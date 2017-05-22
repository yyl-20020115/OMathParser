using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Math;
using OfficeMath = DocumentFormat.OpenXml.Math.OfficeMath;

using OMathParser.Tokens.Abstract;

namespace OMathParser.Tokens
{
    class TokenTreeBuilder
    {
        public TokenTreeBuilder()
        {

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


        private static IToken processElement(OpenXmlElement e)
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
            else
            {
                throw new NotImplementedException("No handler implemented for " + e.GetType().FullName);
            }
        }

        private static TextRunToken processRun(Run r)
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

        private static FractionToken processFraction(Fraction f)
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

            return new FractionToken(mergeTextRunTokens(numerator), mergeTextRunTokens(denominator));
        }

        private static SubscriptToken processSubScript(Subscript s)
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

            return new SubscriptToken(mergeTextRunTokens(subBase), mergeTextRunTokens(argument));
        }

        private static SuperscriptToken processSupScript(Superscript s)
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

            return new SuperscriptToken(mergeTextRunTokens(supBase), mergeTextRunTokens(argument));
        }

        private static RadicalToken processRadical(Radical r)
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

            return new RadicalToken(mergeTextRunTokens(radBase), mergeTextRunTokens(degree));
        }

        private static IToken processDelimiter(Delimiter d)
        {
            DelimiterProperties dp = d.DelimiterProperties;
            char beginChar = dp.BeginChar == null ? '(' : dp.BeginChar.Val.ToString().Trim().ElementAt(0);
            char endChar = dp.EndChar == null ? ')' : dp.EndChar.Val.ToString().Trim().ElementAt(0);

            var delimiterElements = d.Elements<Base>();
            if (delimiterElements.Count() > 1)
            {
                throw new NotImplementedException("No handler implemented for Delimiter with multiple bases.");
            }
            else
            {
                var children = from child in delimiterElements.First() select processElement(child);

                return new ParenthesesToken(beginChar, endChar, mergeTextRunTokens(new TokenList(children)));
            }
        }

        private static IToken processMathFunction(MathFunction f)
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

            return new FunctionApplyToken(mergeTextRunTokens(funcBase), mergeTextRunTokens(funcName));
        }

        private static TokenList mergeTextRunTokens(TokenList tokens)
        {
            TokenList consecutiveTextRuns = new TokenList();
            TokenList processed = new TokenList();
            

            int i = 0; 
            while (i < tokens.Count)
            {
                if (tokens[i] is TextRunToken)
                {
                    consecutiveTextRuns.addToken(tokens[i]);
                }
                else
                {
                    if (consecutiveTextRuns.Count != 0)
                    {
                        StringBuilder textRun = new StringBuilder();
                        foreach (var text in consecutiveTextRuns)
                        {
                            textRun.Append((text as TextRunToken).Text);
                        }
                        processed.addToken(new TextRunToken(textRun.ToString()));
                        consecutiveTextRuns.Clear();
                    }

                    processed.Add(tokens[i]);
                }

                i++;
            }

            processed.Parent = tokens.Parent;
            return processed;
        }
    }
}
