using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens
{
    class SuperscriptToken : AbstractToken
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
            supBase.addToken(t);
        }

        public void addSupArgumentToken(IToken t)
        {
            supArgument.addToken(t);
        }

        public override string simpleRepresentation()
        {
            return String.Format("Superscript: base=({0}), arg=({1})", supBase.simpleRepresentation(), supArgument.simpleRepresentation());
        }

        public TokenList Base { get => supBase; }
        public TokenList Argument { get => supArgument; }
    }
}
