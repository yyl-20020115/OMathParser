using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes;

public class ConstantIdentifierNode(string name, double value) : SyntaxNode
{
    private string name = name;
    private double value = value;

    public override double Value => value;

    public string Name => name;

    public override string SimpleRepresentation => $"ConstantIdentifier: {name}={value.ToString()}";

    public override string ToInfixNotation() => name;

    public override string ToPostfixNotation() => name;

    public override bool Equals(object? obj) => obj is ConstantIdentifierNode other && this.name.Equals(other.name);

    public override int GetHashCode() => name.GetHashCode();
}
