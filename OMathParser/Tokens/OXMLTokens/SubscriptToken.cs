using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens;

public class SubscriptToken(TokenList subBase, TokenList subArgument) : AbstractToken()
{
    private readonly TokenList subBase = subBase;
    private readonly TokenList subArgument = subArgument;

    public void AddBaseToken(IToken t) => subBase.Append(t);
    public void AddSubArgumentToken(IToken t) => subArgument.Append(t);


    public override string SimpleRepresentation => $"Subscript: base=({subBase.SimpleRepresentation}), arg=({subArgument.SimpleRepresentation})";

    public TokenList Base => subBase;
    public TokenList Subscript => subArgument;
}
