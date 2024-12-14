using OMathParser.Utils;

namespace OMathParser.Lexical;

public class Tokenizer(ParseProperties properties)
{
    private readonly ParseProperties properties = properties;
    private readonly NumericLiteralMatcher literalMatcher = new(properties);

    public List<Lexeme> Tokenize(string run, bool matchPredefinedIdentifiers)
    {
        List<Lexeme> lexemes = [];

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
                Lexeme matched = MatchIdentifierName(run, i);
                if (matchPredefinedIdentifiers)
                {
                    // TODO :  
                    matched = MatchDefinedIdentifier(run, i);
                }

                if (matched != null)
                {
                    lexemes.Add(matched);
                    i += matched.Value.Length;
                    continue;
                }

                matched = MatchNumericLiteral(run, i);
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

    private Lexeme MatchDefinedIdentifier(string input, int startPos)
    {
        var inputStart = input.Substring(startPos);
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

    private Lexeme MatchIdentifierName(string input, int startPos) => char.IsLetter(input[startPos]) ? new Lexeme(Lexeme.LexemeType.IDENTIFIER, input.Substring(startPos, 1)) : null;

    private Lexeme MatchNumericLiteral(string input, int startPos) => literalMatcher.TryMatch(input, startPos, out string match) ? new Lexeme(Lexeme.LexemeType.REAL_VALUE, match) : null;

    private bool MatchesFromPosition(string input, int startPos, string value)
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

    public override bool Equals(object? obj) => obj is Tokenizer tokenizer &&
               EqualityComparer<ParseProperties>.Default.Equals(properties, tokenizer.properties);

    public override int GetHashCode() => base.GetHashCode();
}
