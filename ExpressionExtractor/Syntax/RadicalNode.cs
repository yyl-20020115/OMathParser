using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Abstract;

namespace OMathParser.Syntax
{
    class RadicalNode : BinaryNode
    {
        public RadicalNode(SyntaxNode radicalBase, SyntaxNode degree) 
            : base(left : radicalBase, right : degree)
        {
        }

        public override double getValue()
        {
            return Math.Pow(left.getValue(), 1 / right.getValue());
        }

        public override string simpleRepresentation()
        {
            return String.Format("Root: {0} √ {1} ", right.simpleRepresentation(), left.simpleRepresentation());
        }
    }
}
