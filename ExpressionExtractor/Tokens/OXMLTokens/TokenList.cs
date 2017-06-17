using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Utils;
using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens
{
    public class TokenList : List<IToken>, IToken, ISimplifiable
    {
        private IToken parent;

        public IToken Parent { get => parent; set => parent = value; }

        public TokenList() : base() { }
        public TokenList(IEnumerable<IToken> collection) 
            : base()
        {
            foreach (var item in collection)
            {
                this.Append(item);
            }
        }
        public TokenList(Int32 capacity) : base(capacity) { }

        

        public void Append(IToken token)
        {
            if (token != null)
            {
                if (token is TokenList)
                {
                    this.AddRange(token as TokenList);
                }
                else
                {
                    this.Add(token);
                }
            }
        }

        public string simpleRepresentation()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                sb.Append(this.ElementAt(i).simpleRepresentation());
                if (i < this.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return simpleRepresentation();
        }
    }
}
