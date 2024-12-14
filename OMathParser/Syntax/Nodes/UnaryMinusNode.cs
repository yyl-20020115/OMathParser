using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes;

public class UnaryMinusNode(SyntaxNode child) : UnaryNode(child)
{
    public override double Value => -child.Value;

    public override string SimpleRepresentation => $"UnaryMinus: {child.SimpleRepresentation} ";

    public override string ToInfixNotation() => base.ToInfixNotation("-");

    public override string ToPostfixNotation() => base.ToPostfixNotation("-");
}
