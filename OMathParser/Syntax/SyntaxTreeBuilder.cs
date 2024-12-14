using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Tokens;
using OMathParser.Utils;

namespace OMathParser.Syntax;

public class SyntaxTreeBuilder(ParseProperties properties)
{
    private readonly ParseProperties parseProperties = properties;

    public SyntaxTree Build(TokenTree tokenTree)
    {
        TokenListParser parser = new (parseProperties);
        SyntaxNode root = parser.Parse(tokenTree.RootTokens);
        return new SyntaxTree(root);
    }
}
