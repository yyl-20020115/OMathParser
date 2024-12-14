using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes;

public class PowerNode(SyntaxNode exponentBase, SyntaxNode power) : BinaryNode(left : exponentBase, right : power)
{
    public override double Value => Math.Pow(left.Value, right.Value);

    public override string SimpleRepresentation => $"Pow: {left.SimpleRepresentation} ^ {right.SimpleRepresentation} ";

    public override string ToInfixNotation() => base.ToInfixNotation("^");

    public override string ToPostfixNotation() => base.ToPostfixNotation("^");
}
