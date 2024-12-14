using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes;

public class RadicalNode(SyntaxNode radicalBase, SyntaxNode degree) : BinaryNode(left : radicalBase, right : degree)
{
    public override double Value => Math.Pow(left.Value, 1 / right.Value);

    public override string SimpleRepresentation => $"Root: {right.SimpleRepresentation} √ {left.SimpleRepresentation} ";

    public override string ToInfixNotation() => base.ToInfixNotation("√");

    public override string ToPostfixNotation() => base.ToPostfixNotation("√");
}
