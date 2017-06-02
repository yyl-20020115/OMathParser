using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes
{
    public class ConstantIdentifierNode : SyntaxNode
    {
        private String name;
        private double value;

        public ConstantIdentifierNode(string name, double value)
        {
            this.name = name;
            this.value = value;
        }

        public override double getValue()
        {
            return value;
        }

        public string getName()
        {
            return name;
        }

        public override string simpleRepresentation()
        {
            return String.Format("ConstantIdentifier: {0}={1}", name, value.ToString());
        }
    }
}
