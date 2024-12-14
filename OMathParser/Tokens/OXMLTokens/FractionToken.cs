using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens
{
    public class FractionToken : AbstractToken
    {
        private TokenList numerator;
        private TokenList denominator;

        public FractionToken(TokenList numerator, TokenList denominator) 
            : base()
        {
            this.numerator = numerator;
            this.denominator = denominator;
        }

        public void addNummeratorToken(IToken t)
        {
            numerator.Append(t);
        }

        public void addDenominatorToken(IToken t)
        {
            denominator.Append(t);
        }

        public override string SimpleRepresentation()
        {
            return string.Format("Fraction: num=({0}), den=({1})", numerator.SimpleRepresentation(), denominator.SimpleRepresentation());
        }

        public TokenList Numerator { get => numerator; }
        public TokenList Denominator { get => denominator; }
    }
}
