using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMathParser.Tokens.OXMLTokens.Abstract
{
    public abstract class AbstractToken : IToken
    {
        protected IToken parent;

        public IToken Parent { get => parent; set => parent = value; } 

        public abstract string simpleRepresentation();
    }
}
