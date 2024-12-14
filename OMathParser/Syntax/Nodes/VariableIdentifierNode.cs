using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes;

public class VariableIdentifierNode(string name) : SyntaxNode
{
    private double value = 0;
    private readonly string name = name;

    public override double Value => value;

    public void SetValue(double value) => this.value = value;

    public override string SimpleRepresentation => "VariableIdentifier: " + name;

    public override string ToInfixNotation() => name;

    public override string ToPostfixNotation() => name;

    public override bool Equals(object? obj) 
        => obj is VariableIdentifierNode other && this.name.Equals(other.name);

    public override int GetHashCode() => name.GetHashCode();
}
