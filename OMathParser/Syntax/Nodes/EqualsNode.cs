using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes;

class EqualsNode(SyntaxNode left, SyntaxNode right) : BinaryNode(left, right)
{
    public override double Value => right.Value;

    public override string SimpleRepresentation => $"EqualsNode: {left.SimpleRepresentation} = {right.SimpleRepresentation}";

    public override string ToInfixNotation() => base.ToInfixNotation("=");

    public override string ToPostfixNotation() => base.ToPostfixNotation("=");
}
