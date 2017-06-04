using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Tokens;
using OMathParser.Utils;

namespace OMathParser.Syntax
{
    public class SyntaxTreeBuilder
    {
        private ParseProperties parseProperties;

        public SyntaxTreeBuilder(ParseProperties properties)
        {
            parseProperties = properties;
        }

        public SyntaxTree Build(TokenTree tokenTree)
        {
            TokenListParser parser = new TokenListParser(parseProperties, tokenTree.RootTokens);
            SyntaxNode root = parser.parse();
            return new SyntaxTree(root);
        }
    }
}
