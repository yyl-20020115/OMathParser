﻿using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes;

public class SubtractionNode(SyntaxNode left, SyntaxNode right) : BinaryNode(left, right)
{
    public override double Value => left.Value - right.Value;

    public override string SimpleRepresentation => $"Sub: {left.SimpleRepresentation} - {right.SimpleRepresentation} ";

    public override string ToInfixNotation() => base.ToInfixNotation("-");

    public override string ToPostfixNotation() => base.ToPostfixNotation("-");
}
