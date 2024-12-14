using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens;

public class RadicalToken(TokenList radBase, TokenList degree) : AbstractToken()
{
    private readonly TokenList radBase = radBase;
    private readonly TokenList degree = degree;

    public override string SimpleRepresentation => $"Radical: deg=({degree.SimpleRepresentation}), base=({radBase.SimpleRepresentation})";

    public TokenList Base => radBase;
    public TokenList Degree => degree;
}
