using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


namespace OMathParser.Tokens
{
    public class TokenTreeBuilder
    {
        private ParseProperties parseProperties;
        private Tokenizer textRunTokenizer;

        private ISet<Lexeme> foundIdentifiers;

        public TokenTreeBuilder(ParseProperties parseProperties)
        {
            this.parseProperties = parseProperties;
            this.textRunTokenizer = new Tokenizer(parseProperties);
            this.foundIdentifiers = new HashSet<Lexeme>();
        }

        public TokenTree build(OfficeMath expression)
        {
            foundIdentifiers.Clear();

            TokenList rootTokens = new TokenList();

            foreach (OpenXmlElement element in expression.ChildElements)
            {
                IToken processedElement = processElement(element);
                rootTokens.Append(processedElement);
            }

            return new TokenTree(rootTokens, foundIdentifiers);
        }

        private IToken processElement(OpenXmlElement e)
        {
            if (e is Run)
            {
                TokenList lexemes = processRun(e as Run);
                foreach (IToken l in lexemes)
                {
                    Lexeme lex = l as Lexeme;
                    if (lex != null && lex.Type == Lexeme.LexemeType.IDENTIFIER)
                    {
                        foundIdentifiers.Add(lex);
                    }
                }
                return lexemes;
            }
            else if (e is Fraction)
            {
                return processFraction(e as Fraction);
            }
            else if (e is Subscript)
            {
                return processSubScript(e as Subscript);
            }
            else if (e is Superscript)
            {
                return processSupScript(e as Superscript);
            }
            else if (e is Radical)
            {
                return processRadical(e as Radical);
            }
            else if (e is Delimiter)
            {
                return processDelimiter(e as Delimiter);
            }
            else if (e is MathFunction)
            {
                return processMathFunction(e as MathFunction);
            }
            else if (e is BookmarkStart || e is BookmarkEnd)
            {
                return null;
            }
            else
            {
                throw new NotImplementedException("No handler implemented for " + e.GetType().FullName);
            }
        }

        private TokenList processRun(Run r)
        {
            StringBuilder innerText = new StringBuilder();
            foreach (var child in r.ChildElements)
            {
                if (child is Text)
                {
                    innerText.Append(((Text)child).Text);
                }
            }

            return new TokenList(textRunTokenizer.Tokenize(innerText.ToString(), true));
        }

        private FractionToken processFraction(Fraction f)
        {
            TokenList denominator = new TokenList();
            TokenList numerator = new TokenList();

            foreach (var child in f.Denominator)
            {
                denominator.Append(processElement(child));
            }

            foreach (var child in f.Numerator)
            {
                numerator.Append(processElement(child));
            }

            return new FractionToken(numerator, denominator);
        }

        private SubscriptToken processSubScript(Subscript s)
        {
            TokenList subBase = new TokenList();
            TokenList argument = new TokenList();

            foreach (var child in s.Base)
            {
                subBase.Append(processElement(child));
            }

            foreach (var child in s.SubArgument)
            {
                argument.Append(processElement(child));
            }

            return new SubscriptToken(subBase, argument);
        }

        private SuperscriptToken processSupScript(Superscript s)
        {
            TokenList supBase = new TokenList();
            TokenList argument = new TokenList();

            foreach (var child in s.Base)
            {
                supBase.Append(processElement(child));
            }

            foreach (var child in s.SuperArgument)
            {
                argument.Append(processElement(child));
            }

            return new SuperscriptToken(supBase, argument);
        }

        private RadicalToken processRadical(Radical r)
        {
            TokenList degree = new TokenList();
            TokenList radBase = new TokenList();

            if (!r.Degree.HasChildren)
            {
                degree.Append(new TextRunToken("2"));
            }
            else
            {
                foreach (var child in r.Degree)
                {
                    degree.Append(processElement(child));
                }
            }

            foreach (var child in r.Base)
            {
                radBase.Append(processElement(child));
            }

            return new RadicalToken(radBase, degree);
        }

        private IToken processDelimiter(Delimiter d)
        {
            DelimiterProperties dp = d.DelimiterProperties;
            char beginChar = dp.BeginChar == null ? '(' : dp.BeginChar.Val.ToString().Trim().ElementAt(0);
            char endChar = dp.EndChar == null ? ')' : dp.EndChar.Val.ToString().Trim().ElementAt(0);

            var delimiterElements = d.Elements<Base>();
            if (delimiterElements.Count() > 1)
            {
                char separator = dp.SeparatorChar == null ? '|' : dp.SeparatorChar.Val.ToString().Trim().ElementAt(0);
                DelimiterToken delimiterToken = new DelimiterToken(beginChar, endChar, separator);

                foreach (var element in delimiterElements)
                {
                    var processedElement = processElement(element);
                    delimiterToken.AddElement(processedElement);
                }

                return delimiterToken;
            }
            else
            {
                var children = from child in delimiterElements.First() select processElement(child);

                return new ParenthesesToken(beginChar, endChar, new TokenList(children));
            }
        }

        private IToken processMathFunction(MathFunction f)
        {
            TokenList funcName = new TokenList();
            TokenList funcBase = new TokenList();

            foreach (var child in f.FunctionName)
            {
                funcName.Append(processElement(child));
            }

            foreach (var child in f.Base)
            {
                funcBase.Append(processElement(child));
            }

            return new FunctionApplyToken(funcBase, funcName);
        }
    }
}
