using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMathParser.Syntax.Nodes.Abstract
{
    public abstract class UnaryNode : SyntaxNode
    {
        protected SyntaxNode child;

        public UnaryNode(SyntaxNode child)
            : base()
        {
            this.child = child;
        }

        protected string toInfixNotation(String operatorString)
        {
            String child = this.child.toInfixNotation();

            if (!(this.child is ConstantIdentifierNode ||
                    this.child is LiteralNode ||
                    this.child is VariableIdentifierNode))
            {
                child = "(" + child + ")";
            }

            return String.Format("{0}[u{1}]", child, operatorString);
        }

        protected string toPostfixNotation(String operatorString)
        {
            String child = this.child.toPostfixNotation();
            return String.Format("{0}[{1}u]", child, operatorString);
        }
    }
}
