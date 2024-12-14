using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens
{
    public class SuperscriptToken : AbstractToken
    {
        private TokenList supBase;
        private TokenList supArgument;

        public SuperscriptToken(TokenList supBase, TokenList supArgument)
            : base()
        {
            this.supBase = supBase;
            this.supArgument = supArgument;
        }

        public void addBaseToken(IToken t)
        {
            supBase.Append(t);
        }

        public void addSupArgumentToken(IToken t)
        {
            supArgument.Append(t);
        }

        public override string SimpleRepresentation()
        {
            return string.Format("Superscript: base=({0}), arg=({1})", supBase.SimpleRepresentation(), supArgument.SimpleRepresentation());
        }

        public TokenList Base { get => supBase; }
        public TokenList Argument { get => supArgument; }
    }
}
