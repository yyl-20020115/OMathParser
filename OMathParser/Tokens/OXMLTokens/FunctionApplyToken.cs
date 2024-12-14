using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens;

public class FunctionApplyToken(TokenList funcBase, TokenList funcName) : AbstractToken()
{
    private readonly TokenList funcBase = funcBase;
    private readonly TokenList funcName = funcName;

    public override string SimpleRepresentation => $"Func: name=({funcName.SimpleRepresentation}), base=({funcBase.SimpleRepresentation})";

    public TokenList FunctionName => funcName;
    public TokenList Arguments => funcBase;
}
