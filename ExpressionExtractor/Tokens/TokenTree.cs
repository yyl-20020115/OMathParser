using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMathParser.Tokens
{
    public class TokenTree
    {
        private TokenList rootTokens;

        public TokenTree(TokenList rootTokens)
        {
            this.rootTokens = rootTokens;
        }

        public TokenList RootTokens { get => rootTokens; }
    }
}
