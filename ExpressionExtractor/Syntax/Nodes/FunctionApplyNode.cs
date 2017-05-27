using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes
{
    class FunctionApplyNode : UnaryNode
    {
        private String name;

        public delegate double function(double input);
        private function funcDefinition;

        public FunctionApplyNode(SyntaxNode argument, function funcDefinition, String name) : base(argument)
        {
            this.name = name;
            this.funcDefinition = funcDefinition;
        }

        public override double getValue()
        {
            return funcDefinition(child.getValue());
        }

        public override string simpleRepresentation()
        {
            return String.Format("FuncApply: {0}({1}) ", name, child.simpleRepresentation());
        }
    }
}
