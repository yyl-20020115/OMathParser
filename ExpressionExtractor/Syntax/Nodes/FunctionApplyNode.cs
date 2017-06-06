using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes
{
    public class FunctionApplyNode : SyntaxNode
    {
        private String name;
        private int nArguments;
        private ArgumentListNode arguments;

        public delegate double FunctionBody(double[] input);
        private FunctionBody funcDefinition;

        public FunctionApplyNode(ArgumentListNode arguments, FunctionBody funcDefinition, String name)
        {
            this.name = name;
            this.funcDefinition = funcDefinition;
            this.arguments = arguments;
        }

        public override double getValue()
        {
            return funcDefinition(arguments.CalculatedValues);
        }

        public override string simpleRepresentation()
        {
            return String.Format("FuncApply: {0}({1}) ", name, arguments.simpleRepresentation());
        }

        public ArgumentListNode Arguments { get => arguments; }
        public int ArgumentsCount { get => arguments.Count; }
    }
}
