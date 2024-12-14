using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes;

public class LiteralNode(double value) : SyntaxNode
{
    private readonly double value = value;

    public override double Value => value;

    public override string SimpleRepresentation => $"Literal: {value}";

    public override string ToInfixNotation() => value.ToString();

    public override string ToPostfixNotation() => value.ToString();
}
