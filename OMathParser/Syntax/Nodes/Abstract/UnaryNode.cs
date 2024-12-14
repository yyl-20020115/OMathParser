namespace OMathParser.Syntax.Nodes.Abstract;

public abstract class UnaryNode(SyntaxNode child) : SyntaxNode()
{
    protected SyntaxNode child = child;

    protected string ToInfixNotation(string operatorString)
    {
        var child = this.child.ToInfixNotation();

        if (!(this.child is ConstantIdentifierNode ||
                this.child is LiteralNode ||
                this.child is VariableIdentifierNode))
        {
            child = $"({child})";
        }

        return $"[u{operatorString}]{child}";
    }

    protected string ToPostfixNotation(string operatorString)
    {
        var child = this.child.ToPostfixNotation();
        return $"{child} [{operatorString}u]";
    }
}
