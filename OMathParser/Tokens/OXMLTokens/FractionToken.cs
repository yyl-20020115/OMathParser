using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens;

public class FractionToken(TokenList numerator, TokenList denominator) : AbstractToken()
{
    private readonly TokenList numerator = numerator;

    public void AddNummeratorToken(IToken t) => numerator.Append(t);

    public void AddDenominatorToken(IToken t) => Denominator.Append(t);

    public override string SimpleRepresentation => $"Fraction: num=({numerator.SimpleRepresentation}), den=({Denominator.SimpleRepresentation})";

    public TokenList Numerator => numerator;
    public TokenList Denominator { get; } = denominator;
}
