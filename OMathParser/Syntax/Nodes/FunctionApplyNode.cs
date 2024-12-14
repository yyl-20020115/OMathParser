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
        private string name;
        private ArgumentListNode arguments;

        public delegate double FunctionBody(double[] input);
        private FunctionBody funcDefinition;

        public FunctionApplyNode(ArgumentListNode arguments, FunctionBody funcDefinition, string name)
        {
            this.name = name;
            this.funcDefinition = funcDefinition;
            this.arguments = arguments;
        }

        public override double getValue()
        {
            return funcDefinition(arguments.CalculatedValues);
        }

        public override string SimpleRepresentation()
        {
            return string.Format("FuncApply: {0}({1}) ", name, arguments.SimpleRepresentation());
        }

        public override string toInfixNotation()
        {
            string arguments = string.Join(", ", this.arguments.Select(arg => arg.toInfixNotation()));
            return string.Format("{0}({1})", name, arguments);
        }

        public override string toPostfixNotation()
        {
            string arguments = string.Join(", ", this.arguments.Select(arg => arg.toPostfixNotation()));
            return string.Format("{0}{1}[nArgs:{2}]", arguments, name, this.arguments.Count);
        }

        public override bool Equals(object obj)
        {
            FunctionApplyNode other = obj as FunctionApplyNode;
            if (other != null)
            {
                return this.name.Equals(other.name);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }


        public ArgumentListNode Arguments { get => arguments; }
        public int ArgumentsCount { get => arguments.Count; }
    }
}
