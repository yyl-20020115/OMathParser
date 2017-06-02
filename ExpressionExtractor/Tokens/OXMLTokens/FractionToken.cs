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
            numerator.addToken(t);
        }

        public void addDenominatorToken(IToken t)
        {
            denominator.addToken(t);
        }

        public override string simpleRepresentation()
        {
            return String.Format("Fraction: num=({0}), den=({1})", numerator.simpleRepresentation(), denominator.simpleRepresentation());
        }

        public TokenList Numerator { get => numerator; }
        public TokenList Denominator { get => denominator; }
    }
}
