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

        public List<Lexeme> Tokenize(String run)
        {
            List<Lexeme> lexemes = new List<Lexeme>();

            int i = 0;

            while (i < run.Length)
            {
                char current = run[i];
                if (Char.IsWhiteSpace(current))
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
                    Lexeme matched = matchFunctionName(run, i);
                    if (matched != null)
                    {
                        lexemes.Add(matched);
                        i += matched.Value.Length;
                        continue;
                    }

                    matched = matchVariableIdentifier(run, i);
                    if (matched != null)
                    {
                        lexemes.Add(matched);
                        i += matched.Value.Length;
                        continue;
                    }

                    matched = matchConstantIdentifier(run, i);
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

        private Lexeme matchFunctionName(string input, int startPos)
        {
            HashSet<String> matches = new HashSet<string>();
            foreach (KeyValuePair<String, int> functionDeclaration in properties.Functions)
            {
                if (matchesFromPosition(input, startPos, functionDeclaration.Key))
                {
                    matches.Add(functionDeclaration.Key);
                }
            }

            if (matches.Count == 0)
            {
                return null;
            }
            else
            {
                return new Lexeme(Lexeme.LexemeType.FUNCTION_NAME, matches.OrderBy(s => s.Length).First());
            }
        }

        private Lexeme matchConstantIdentifier(string input, int startPos)
        {
            HashSet<String> matches = new HashSet<string>();
            foreach (KeyValuePair<String, double> constant in properties.ConstantIdentifiers)
            {
                if (matchesFromPosition(input, startPos, constant.Key))
                {
                    matches.Add(constant.Key);
                }
            }

            if (matches.Count == 0)
            {
                return null;
            }
            else
            {
                return new Lexeme(Lexeme.LexemeType.IDENTIFIER_CONST, matches.OrderBy(s => s.Length).First());
            }
        }

        private Lexeme matchVariableIdentifier(string input, int startPos)
        {
            HashSet<String> matches = new HashSet<string>();
            foreach (String name in properties.VariableIdentifiers)
            {
                if (matchesFromPosition(input, startPos, name))
                {
                    matches.Add(name);
                }
            }

            if (matches.Count == 0)
            {
                return null;
            }
            else
            {
                return new Lexeme(Lexeme.LexemeType.IDENTIFIER_VAR, matches.OrderBy(s => s.Length).First());
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
