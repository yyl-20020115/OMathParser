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
using OMathParser.Utils;


namespace OMathParser.Tokens
{
    public class TokenTreeBuilder
    {
        private ParseProperties parseProperties;
        private IDictionary<String, int> functions;

        public TokenTreeBuilder(ParseProperties parseProperties)
        {
            this.parseProperties = parseProperties;

            functions = new Dictionary<String, int>();
            foreach (KeyValuePair<String, int> funcDeclaration in parseProperties.Functions)
            {
                this.functions.Add(funcDeclaration.Key, funcDeclaration.Value);
            }
        }

        public TokenTree build(OfficeMath expression)
        {
            TokenList rootTokens = new TokenList();

            foreach (OpenXmlElement element in expression.ChildElements)
            {
                IToken processedElement = processElement(element);
                rootTokens.addToken(processedElement);
            }

            return new TokenTree(rootTokens);
        }


        private IToken processElement(OpenXmlElement e)
        {
            if (e is Run)
            {
                return processRun(e as Run);
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

        private TextRunToken processRun(Run r)
        {
            StringBuilder innerText = new StringBuilder();
            foreach (var child in r.ChildElements)
            {
                if (child is Text)
                {
                    innerText.Append(((Text)child).Text);
                }
            }

            return new TextRunToken(innerText.ToString());
        }

        private FractionToken processFraction(Fraction f)
        {
            TokenList denominator = new TokenList();
            TokenList numerator = new TokenList();

            foreach (var child in f.Denominator)
            {
                denominator.addToken(processElement(child));
            }

            foreach (var child in f.Numerator)
            {
                numerator.addToken(processElement(child));
            }

            return new FractionToken(numerator, denominator);
        }

        private SubscriptToken processSubScript(Subscript s)
        {
            TokenList subBase = new TokenList();
            TokenList argument = new TokenList();

            foreach (var child in s.Base)
            {
                subBase.addToken(processElement(child));
            }

            foreach (var child in s.SubArgument)
            {
                argument.addToken(processElement(child));
            }

            return new SubscriptToken(subBase, argument);
        }

        private SuperscriptToken processSupScript(Superscript s)
        {
            TokenList supBase = new TokenList();
            TokenList argument = new TokenList();

            foreach (var child in s.Base)
            {
                supBase.addToken(processElement(child));
            }

            foreach (var child in s.SuperArgument)
            {
                argument.addToken(processElement(child));
            }

            return new SuperscriptToken(supBase, argument);
        }

        private RadicalToken processRadical(Radical r)
        {
            TokenList degree = new TokenList();
            TokenList radBase = new TokenList();

            if (!r.Degree.HasChildren)
            {
                degree.addToken(new TextRunToken("2"));
            }
            else
            {
                foreach (var child in r.Degree)
                {
                    degree.addToken(processElement(child));
                }
            }

            foreach (var child in r.Base)
            {
                radBase.addToken(processElement(child));
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
                funcName.addToken(processElement(child));
            }

            foreach (var child in f.Base)
            {
                funcBase.addToken(processElement(child));
            }

            return new FunctionApplyToken(funcBase, funcName);
        }
    }
}
