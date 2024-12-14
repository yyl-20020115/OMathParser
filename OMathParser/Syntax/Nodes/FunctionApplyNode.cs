using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes;

public class FunctionApplyNode(ArgumentListNode arguments, FunctionApplyNode.FunctionBody funcDefinition, string name) : SyntaxNode
{
    private readonly string name = name;
    private readonly ArgumentListNode arguments = arguments;

    public delegate double FunctionBody(double[] input);
    private readonly FunctionBody funcDefinition = funcDefinition;

    public override double Value => funcDefinition(arguments.CalculatedValues);

    public override string SimpleRepresentation => string.Format("FuncApply: {0}({1}) ", name, arguments.SimpleRepresentation);

    public override string ToInfixNotation()
    {
        var arguments = string.Join(", ", this.arguments.Select(arg => arg.ToInfixNotation()));
        return $"{name}({arguments})";
    }

    public override string ToPostfixNotation()
    {
        var arguments = string.Join(", ", this.arguments.Select(arg => arg.ToPostfixNotation()));
        return $"{arguments}{name}[nArgs:{this.arguments.Count}]";
    }

    public override bool Equals(object? obj) => obj is FunctionApplyNode other && this.name.Equals(other.name);

    public override int GetHashCode() => name.GetHashCode();


    public ArgumentListNode Arguments => arguments;
    public int ArgumentsCount => arguments.Count;
}
