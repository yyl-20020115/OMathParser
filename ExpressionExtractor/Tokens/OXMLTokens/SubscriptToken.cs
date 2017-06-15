using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens
{
    public class SubscriptToken : AbstractToken
    {
        private TokenList subBase;
        private TokenList subArgument;

        public SubscriptToken(TokenList subBase, TokenList subArgument)
            : base()
        {
            this.subBase = subBase;
            this.subArgument = subArgument;
        }

        public void addBaseToken(IToken t)
        {
            subBase.Append(t);
        }

        public void addSubArgumentToken(IToken t)
        {
            subArgument.Append(t);
        }

        public override string simpleRepresentation()
        {
            return String.Format("Subscript: base=({0}), arg=({1})", subBase.simpleRepresentation(), subArgument.simpleRepresentation());
        }
    }
}
