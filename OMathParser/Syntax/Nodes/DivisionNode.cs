using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes
{
    public class DivisionNode : BinaryNode
    {
        public DivisionNode(SyntaxNode left, SyntaxNode right) : base(left, right)
        {
        }

        public override double getValue()
        {
            return left.getValue() / right.getValue();
        }

        public override string SimpleRepresentation()
        {
            return string.Format("Div: {0} / {1} ", left.SimpleRepresentation(), right.SimpleRepresentation());
        }

        public override string toInfixNotation()
        {
            return base.toInfixNotation("÷");
        }

        public override string toPostfixNotation()
        {
            return base.toPostfixNotation("÷");
        }
    }
}
