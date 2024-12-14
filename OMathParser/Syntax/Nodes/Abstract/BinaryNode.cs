namespace OMathParser.Syntax.Nodes.Abstract;

public abstract class BinaryNode(SyntaxNode left, SyntaxNode right) : SyntaxNode()
{
    protected SyntaxNode left = left;
    protected SyntaxNode right = right;

    protected string ToPostfixNotation(string operatorString)
    {
        var left = this.left.ToPostfixNotation();
        var right = this.right.ToPostfixNotation();

        return $"{left} {right} {operatorString}";
    }
    protected string ToInfixNotation(string operatorString)
    {
        var left = this.left.ToInfixNotation();
        var right = this.right.ToInfixNotation();

        if (!(this.left is ConstantIdentifierNode ||
                this.left is LiteralNode ||
                this.left is VariableIdentifierNode))
        {
            left = $"({left})";
        }

        if (!(this.right is ConstantIdentifierNode ||
                this.right is LiteralNode ||
                this.right is VariableIdentifierNode))
        {
            right = $"({this.right.ToInfixNotation()})";
        }

        return $"{left} {operatorString} {right}";
    }

}
