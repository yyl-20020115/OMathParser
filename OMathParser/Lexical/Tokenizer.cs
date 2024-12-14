using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Utils;
using System.Text.RegularExpressions;

namespace OMathParser.Lexical
{
    public class Tokenizer
    {
        private ParseProperties properties;
        private NumericLiteralMatcher literalMatcher;

        public Tokenizer(ParseProperties properties)
        {
            this.properties = properties;
            this.literalMatcher = new NumericLiteralMatcher(properties);
        }

        public List<Lexeme> Tokenize(String run, bool matchPredefinedIdentifiers)
        {
            List<Lexeme> lexemes = new List<Lexeme>();

            int i = 0;

            while (i < run.Length)
            {
                char current = run[i];
                if (Char.IsWhiteSpace(current) ||
                    Char.GetUnicodeCategory(current) == System.Globalization.UnicodeCategory.Format)
                {
                    i++;
                }
                else if (current == '=')
                {
                    i++;
                    lexemes.Add(new Lexeme(Lexeme.LexemeType.EQ_SIGN, current.ToString()));
                }
                else if (current == ',')
                {
                    i++;
                    lexemes.Add(new Lexeme(Lexeme.LexemeType.ARGUMENT_SEPARATOR, current.ToString()));
                }
                else if (current == '+')
                {
                    i++;
                    lexemes.Add(new Lexeme(Lexeme.LexemeType.OP_PLUS, current.ToString()));
                }
                else if (current == '-')
                {
                    i++;
                    lexemes.Add(new Lexeme(Lexeme.LexemeType.OP_MINUS, current.ToString()));
                }
                else if (current == '*')
                {
                    i++;
                    lexemes.Add(new Lexeme(Lexeme.LexemeType.OP_MUL, current.ToString()));
                }
                else if (current == '/' || current == '÷')
                {
                    i++;
                    lexemes.Add(new Lexeme(Lexeme.LexemeType.OP_DIV, current.ToString()));
                }
                else if (current == '^')
                {
                    i++;
                    lexemes.Add(new Lexeme(Lexeme.LexemeType.OP_POW, current.ToString()));
                }
                else if (current == '(')
                {
                    i++;
                    lexemes.Add(new Lexeme(Lexeme.LexemeType.LEFT_PAREN, current.ToString()));
                }
                else if (current == ')')
                {
                    i++;
                    lexemes.Add(new Lexeme(Lexeme.LexemeType.RIGHT_PAREN, current.ToString()));
                }
                else
                {
                    Lexeme matched = matchIdentifierName(run, i);
                    if (matchPredefinedIdentifiers)
                    {
                        // TODO :  
                        matched = matchDefinedIdentifier(run, i);
                    }

                    if (matched != null)
                    {
                        lexemes.Add(matched);
                        i += matched.Value.Length;
                        continue;
                    }

                    matched = matchNumericLiteral(run, i);
                    if (matched != null)
                    {
                        lexemes.Add(matched);
                        i += matched.Value.Length;
                        continue;
                    }

                    throw new LexicalException(run, i);
                }
            }

            return lexemes;
        }

        private Lexeme matchDefinedIdentifier(string input, int startPos)
        {
            string inputStart = input.Substring(startPos);
            foreach (var func in properties.Functions)
            {
                if (inputStart.StartsWith(func.Key))
                {
                    return new Lexeme(Lexeme.LexemeType.IDENTIFIER, func.Key);
                }
            }

            foreach (var variable in properties.VariableIdentifiers)
            {
                if (inputStart.StartsWith(variable))
                {
                    return new Lexeme(Lexeme.LexemeType.IDENTIFIER, variable);
                }
            }

            foreach (var constant in properties.ConstantIdentifiers)
            {
                if (inputStart.StartsWith(constant.Key))
                {
                    return new Lexeme(Lexeme.LexemeType.IDENTIFIER, constant.Key);
                }
            }

            return null;
        }

        private Lexeme matchIdentifierName(string input, int startPos)
        {
            if (Char.IsLetter(input[startPos]))
            {
                return new Lexeme(Lexeme.LexemeType.IDENTIFIER, input.Substring(startPos, 1));
            }
            else
            {
                return null;
            }
        }

        private Lexeme matchNumericLiteral(string input, int startPos)
        {
            String match;
            if (literalMatcher.tryMatch(input, startPos, out match))
            {
                return new Lexeme(Lexeme.LexemeType.REAL_VALUE, match);
            }
            else
            {
                return null;
            }
        }

        private bool matchesFromPosition(string input, int startPos, String value)
        {
            try
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (input[startPos + i] != value[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (IndexOutOfRangeException ex)
            {
                return false;
            }
            
        }
    }
}
