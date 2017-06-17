using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.OXMLTokens;
using OMathParser.Lexical;

namespace OMathParser.Tokens
{
    public class TokenTree
    {
        private TokenList rootTokens;
        private ISet<Lexeme> identifiers;

        public TokenTree(TokenList rootTokens, ISet<Lexeme> identifiers)
        {
            this.rootTokens = rootTokens;
            this.identifiers = new HashSet<Lexeme>(identifiers);
        }

        public TokenList RootTokens { get => rootTokens; }
        public IEnumerable<Lexeme> Identifiers { get => identifiers; }
    }
}
