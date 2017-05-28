using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens
{
    class RadicalToken : AbstractToken
    {
        private TokenList radBase;
        private TokenList degree;

        public RadicalToken(TokenList radBase, TokenList degree)
            : base()
        {
            this.radBase = radBase;
            this.degree = degree;
        }

        public override string simpleRepresentation()
        {
            return String.Format("Radical: deg=({0}), base=({1})", degree.simpleRepresentation(), radBase.simpleRepresentation());
        }

        public TokenList Base { get => radBase; }
        public TokenList Degree { get => degree; }
    }
}
