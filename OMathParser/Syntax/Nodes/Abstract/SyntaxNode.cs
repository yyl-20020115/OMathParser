using OMathParser.Utils;

namespace OMathParser.Syntax.Nodes.Abstract;

public abstract class SyntaxNode : ISimplifiable, ISyntaxUnit
{
    protected SyntaxNode? parent;

    public abstract double Value { get; }

    public abstract string SimpleRepresentation { get; }
    public SyntaxNode? Parent { get => parent; set => parent = value; }

    public override string ToString() => this.SimpleRepresentation;

    public abstract string ToInfixNotation();
    public abstract string ToPostfixNotation();

}
