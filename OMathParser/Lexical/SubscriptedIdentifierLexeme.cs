namespace OMathParser.Lexical;

public class SubscriptedIdentifierLexeme(string name, string subscript) : Lexeme(LexemeType.IDENTIFIER, name + "_" + subscript)
{
    private readonly string name = name;
    private readonly string subscript = subscript;

    public string Name => name;
    public string Subscript => subscript;
}
