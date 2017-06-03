using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes
{
    class EqualsNode : BinaryNode
    {
        public EqualsNode(SyntaxNode left, SyntaxNode right) 
            : base(left, right)
        {
        }

        public override double getValue()
        {
            return right.getValue();
        }

        public override string simpleRepresentation()
        {
            return String.Format("EqualsNode: {0} = {1}", left.simpleRepresentation(), right.simpleRepresentation());
        }
    }
}
