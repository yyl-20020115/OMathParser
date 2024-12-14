using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeMath = DocumentFormat.OpenXml.Math.OfficeMath;
using Run = DocumentFormat.OpenXml.Math.Run;
using Text = DocumentFormat.OpenXml.Math.Text;

using OMathParser.Tokens.OXMLTokens;
using OMathParser.Tokens.OXMLTokens.Abstract;
using OMathParser.Lexical;
using OMathParser.Utils;


namespace OMathParser.Tokens;

public class TokenTreeBuilder(ParseProperties parseProperties)
{
    private readonly ParseProperties parseProperties = parseProperties;
    private readonly Tokenizer textRunTokenizer = new (parseProperties);

    private readonly HashSet<Lexeme> foundIdentifiers = [];

    public TokenTree Build(OfficeMath expression)
    {
        foundIdentifiers.Clear();

        TokenList rootTokens = [];

        foreach (OpenXmlElement element in expression.ChildElements)
        {
            IToken processedElement = ProcessElement(element);
            rootTokens.Append(processedElement);
        }

        return new TokenTree(rootTokens, foundIdentifiers);
    }

    private IToken ProcessElement(OpenXmlElement e)
    {
        if (e is Run run)
        {
            TokenList lexemes = ProcessRun(run);
            foreach (IToken l in lexemes)
            {
                if (l is Lexeme lex && lex.Type == Lexeme.LexemeType.IDENTIFIER)
                {
                    foundIdentifiers.Add(lex);
                }
            }
            return lexemes;
        }
        else if (e is Fraction f)
        {
            return ProcessFraction(f);
        }
        else if (e is Subscript s)
        {
            return ProcessSubScript(s);
        }
        else if (e is Superscript u)
        {
            return ProcessSupScript(u);
        }
        else if (e is Radical r)
        {
            return ProcessRadical(r);
        }
        else if (e is Delimiter d)
        {
            return ProcessDelimiter(d);
        }
        else if (e is MathFunction m)
        {
            return ProcessMathFunction(m);
        }
        else if (e is BookmarkStart || e is BookmarkEnd)
        {
            return null;
        }
        else
        {
            throw new NotImplementedException($"No handler implemented for {e.GetType().FullName}");
        }
    }

    private TokenList ProcessRun(Run r)
    {
        var innerText = new StringBuilder();
        foreach (var child in r.ChildElements)
        {
            if (child is Text text)
            {
                innerText.Append(text.Text);
            }
        }

        return new TokenList(textRunTokenizer.Tokenize(innerText.ToString(), true));
    }

    private FractionToken ProcessFraction(Fraction f)
    {
        TokenList denominator = [];
        TokenList numerator = [];

        foreach (var child in f.Denominator ?? [])
        {
            denominator.Append(ProcessElement(child));
        }

        foreach (var child in f.Numerator ?? [])
        {
            numerator.Append(ProcessElement(child));
        }

        return new FractionToken(numerator, denominator);
    }

    private SubscriptToken ProcessSubScript(Subscript s)
    {
        TokenList subBase = [];
        TokenList argument = [];

        foreach (var child in s.Base ?? [])
        {
            subBase.Append(ProcessElement(child));
        }

        foreach (var child in s.SubArgument ?? [])
        {
            argument.Append(ProcessElement(child));
        }

        return new SubscriptToken(subBase, argument);
    }

    private SuperscriptToken ProcessSupScript(Superscript s)
    {
        TokenList supBase = [];
        TokenList argument = [];

        foreach (var child in s.Base ?? [])
        {
            supBase.Append(ProcessElement(child));
        }

        foreach (var child in s.SuperArgument ?? [])
        {
            argument.Append(ProcessElement(child));
        }

        return new SuperscriptToken(supBase, argument);
    }

    private RadicalToken ProcessRadical(Radical r)
    {
        TokenList degree = [];
        TokenList radBase = [];

        if (!r.Degree.HasChildren)
        {
            degree.Append(new TextRunToken("2"));
        }
        else
        {
            foreach (var child in r.Degree ?? [])
            {
                degree.Append(ProcessElement(child));
            }
        }

        foreach (var child in r.Base ?? [])
        {
            radBase.Append(ProcessElement(child));
        }

        return new RadicalToken(radBase, degree);
    }

    private IToken ProcessDelimiter(Delimiter d)
    {
        DelimiterProperties dp = d.DelimiterProperties;
        char beginChar = dp.BeginChar == null ? '(' : dp.BeginChar.Val.ToString().Trim().ElementAt(0);
        char endChar = dp.EndChar == null ? ')' : dp.EndChar.Val.ToString().Trim().ElementAt(0);

        var delimiterElements = d.Elements<Base>();
        if (delimiterElements.Count() > 1)
        {
            char separator = dp.SeparatorChar == null ? '|' : dp.SeparatorChar.Val.ToString().Trim().ElementAt(0);
            var delimiterToken = new DelimiterToken(beginChar, endChar, separator);

            foreach (var element in delimiterElements)
            {
                var processedElement = ProcessElement(element);
                delimiterToken.AddElement(processedElement);
            }

            return delimiterToken;
        }
        else
        {
            var children = from child in delimiterElements.First() select ProcessElement(child);

            return new ParenthesesToken(beginChar, endChar, new TokenList(children));
        }
    }

    private IToken ProcessMathFunction(MathFunction f)
    {
        TokenList funcName = [];
        TokenList funcBase = [];

        foreach (var child in f.FunctionName ?? [])
        {
            funcName.Append(ProcessElement(child));
        }

        foreach (var child in f.Base ?? [])
        {
            funcBase.Append(ProcessElement(child));
        }

        return new FunctionApplyToken(funcBase, funcName);
    }
}
