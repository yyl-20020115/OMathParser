using OMathParser.Lexical;

namespace OMathParser.Utils;

public class UnexpectedLexemeException : ParseException
{
    private readonly Lexeme unexpectedLexeme;

    public UnexpectedLexemeException(Lexeme unexpected)
        : base("") => this.unexpectedLexeme = unexpected;

    public UnexpectedLexemeException(Lexeme unexpected, string message)
        : base(message) => this.unexpectedLexeme = unexpected;

    public Lexeme UnexpectedLexeme => unexpectedLexeme;
}
