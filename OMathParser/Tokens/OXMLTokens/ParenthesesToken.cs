using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens;

public class ParenthesesToken(char beginChar, char endChar, IEnumerable<IToken> elements) : AbstractToken
{
    private readonly TokenList elements = new TokenList(elements);
    private readonly char beginChar = beginChar;
    private readonly char endChar = endChar;

    public override string SimpleRepresentation => $"Parentheses: ({elements.SimpleRepresentation})";

    public char BeginChar => this.beginChar;
    public char EndChar => this.endChar;
    public TokenList Elements => elements;
}
