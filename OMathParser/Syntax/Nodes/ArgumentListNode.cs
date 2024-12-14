using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Utils;

namespace OMathParser.Syntax.Nodes
{
    public class ArgumentListNode : ISimplifiable, IEnumerable<SyntaxNode>
    {
        private List<SyntaxNode> arguments;

        public ArgumentListNode()
        {
            arguments = new List<SyntaxNode>();
        }

        public ArgumentListNode(IEnumerable<SyntaxNode> arguments)
        {
            this.arguments = new List<SyntaxNode>(arguments);
        }

        public void addArgument(SyntaxNode argument)
        {
            arguments.Add(argument);
        }

        public SyntaxNode getArgument(int index)
        {
            return arguments[index];
        }

        public string SimpleRepresentation()
        {
            StringBuilder sb = new StringBuilder();
            foreach(ISimplifiable arg in arguments)
            {
                sb.Append(arg);
                sb.Append(", ");
            }

            if (arguments.Count > 1)
            {
                sb.Remove(sb.Length - 2, 2);
            }
            
            return "ArgumentList: [" + sb.ToString() + "]";
        }

        public IEnumerator<SyntaxNode> GetEnumerator()
        {
            return arguments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return arguments.GetEnumerator();
        }

        public int Count { get => arguments.Count; }

        public double[] CalculatedValues
        {
            get => arguments.Select<SyntaxNode, double>(arg => arg.getValue()).ToArray<double>();
        }
    }
}
