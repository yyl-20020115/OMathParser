using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes
{
    class MultiplicationNode : BinaryNode
    {
        public MultiplicationNode(SyntaxNode left, SyntaxNode right) : base(left, right)
        {
        }

        public override double getValue()
        {
            return left.getValue() * right.getValue();
        }

        public override string simpleRepresentation()
        {
            return String.Format("Mul: {0} * {1} ", left.simpleRepresentation(), right.simpleRepresentation());
        }
    }
}
