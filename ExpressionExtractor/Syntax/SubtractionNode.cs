using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Abstract;

namespace OMathParser.Syntax
{
    class SubtractionNode : BinaryNode
    {
        public SubtractionNode(SyntaxNode left, SyntaxNode right) : base(left, right)
        {
        }

        public override double getValue()
        {
            return left.getValue() - right.getValue();
        }

        public override string simpleRepresentation()
        {
            return String.Format("Sub: {0} - {1} ", left.simpleRepresentation(), right.simpleRepresentation());
        }
    }
}
