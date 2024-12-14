using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes;

public class UnaryPlusNode(SyntaxNode child) : UnaryNode(child)
{
    public override double Value => child.Value;

    public override string SimpleRepresentation => $"UnaryPlus: {child.SimpleRepresentation} ";

    public override string ToInfixNotation() => base.ToInfixNotation("+");

    public override string ToPostfixNotation() => base.ToPostfixNotation("+");
}
