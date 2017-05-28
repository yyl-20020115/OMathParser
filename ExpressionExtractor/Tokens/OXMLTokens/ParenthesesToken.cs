using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens
{
    class ParenthesesToken : AbstractToken
    {
        private TokenList elements;
        private char beginChar;
        private char endChar;

        public ParenthesesToken(char beginChar, char endChar, IEnumerable<IToken> elements)
        {
            this.beginChar = beginChar;
            this.endChar = endChar;
            this.elements = new TokenList(elements);
        }

        public override string simpleRepresentation()
        {
            return String.Format("Parentheses: ({0})", elements.simpleRepresentation());
        }

        public TokenList Elements { get => elements; }
    }
}
