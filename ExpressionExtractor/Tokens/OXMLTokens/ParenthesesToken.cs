using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens
{
    public class ParenthesesToken : AbstractToken
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

        public char BeginChar { get => this.beginChar; }
        public char EndChar { get => this.endChar; }
        public TokenList Elements { get => elements; }
    }
}
