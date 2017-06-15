using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.OXMLTokens;

namespace OMathParser.Tokens
{
    public class TokenTree
    {
        private TokenList rootTokens;
        private ISet<string> identifiers;

        public TokenTree(TokenList rootTokens, ISet<string> identifiers)
        {
            this.rootTokens = rootTokens;
            this.identifiers = new HashSet<string>(identifiers);
        }

        public TokenList RootTokens { get => rootTokens; }
        public IEnumerable<string> Identifiers { get => identifiers; }
    }
}
