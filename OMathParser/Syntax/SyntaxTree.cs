using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax;

public class SyntaxTree(SyntaxNode root)
{
    private readonly SyntaxNode root = root;

    public string ToInfixNotation() => root.ToInfixNotation();

    public string ToPostfixNotation() => root.ToPostfixNotation();

    public SyntaxNode RootNode => root;
}
