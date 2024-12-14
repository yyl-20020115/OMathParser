using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens;

public class SuperscriptToken(TokenList supBase, TokenList supArgument) : AbstractToken()
{
    private readonly TokenList supBase = supBase;
    private readonly TokenList supArgument = supArgument;

    public void AddBaseToken(IToken t) => supBase.Append(t);

    public void AddSupArgumentToken(IToken t) => supArgument.Append(t);

    public override string SimpleRepresentation => $"Superscript: base=({supBase.SimpleRepresentation}), arg=({supArgument.SimpleRepresentation})";

    public TokenList Base => supBase;
    public TokenList Argument => supArgument;
}
