namespace OMathParser.Tokens.OXMLTokens.Abstract;

public abstract class AbstractToken : IToken
{
    protected IToken? parent;

    public IToken? Parent { get => parent; set => parent = value; }

    public abstract string SimpleRepresentation { get; }
}
