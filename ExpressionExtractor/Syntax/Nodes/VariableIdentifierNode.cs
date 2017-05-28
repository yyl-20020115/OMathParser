using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes
{
    class VariableIdentifierNode : SyntaxNode
    {
        private double value;
        private String name;

        public VariableIdentifierNode(String name)
        {
            this.name = name;
            this.value = 0;
        }

        public override double getValue()
        {
            return value;
        }

        public void setValue(double value)
        {
            this.value = value;
        }

        public override string simpleRepresentation()
        {
            return "VariableIdentifier: " + name;
        }
    }
}
