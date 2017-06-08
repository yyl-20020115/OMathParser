using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes
{
    public class LiteralNode : SyntaxNode
    {
        private double value;

        public LiteralNode(double value)
        {
            this.value = value;
        }

        public override double getValue()
        {
            return value;
        }

        public override string simpleRepresentation()
        {
            return String.Format("Literal: " + value.ToString());
        }

        public override string toInfixNotation()
        {
            return value.ToString();
        }

        public override string toPostfixNotation()
        {
            return value.ToString();
        }
    }
}
