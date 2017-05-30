using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens
{
    public class DelimiterToken : AbstractToken
    {
        private List<IToken> elements;
        private char beginChar;
        private char endChar;
        private char delimiter;

        public DelimiterToken(char beginChar, char endChar, char delimiter)
        {
            this.beginChar = beginChar;
            this.endChar = endChar;
            this.delimiter = delimiter;
            this.elements = new List<IToken>();
        }

        public void AddElement(IToken element)
        {
            this.elements.Add(element);
        }

        public List<IToken> Elements { get => this.elements; }

        public override string simpleRepresentation()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IToken t in elements)
            {
                sb.Append(t.simpleRepresentation());
                sb.Append(this.delimiter);
                sb.Append(' ');
            }

            if (elements.Count > 1)
            {
                sb.Remove(sb.Length - 2, 2);
            }

            return String.Format("Delimiter: {0}{1}{2}", beginChar, sb.ToString(), endChar);
        }
    }
}
