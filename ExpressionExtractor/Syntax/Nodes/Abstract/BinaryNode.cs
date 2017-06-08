using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OMathParser.Syntax.Nodes.Abstract
{
    public abstract class BinaryNode : SyntaxNode
    {
        protected SyntaxNode left;
        protected SyntaxNode right;

        public BinaryNode(SyntaxNode left, SyntaxNode right)
            : base()
        {
            this.left = left;
            this.right = right;
        }

        protected string toInfixNotation(String operatorString)
        {
            String left = this.left.toInfixNotation();
            String right = this.right.toInfixNotation();

            if (!(this.left is ConstantIdentifierNode || 
                    this.left is LiteralNode || 
                    this.left is VariableIdentifierNode))
            {
                left = "(" + left + ")";
            }

            if (!(this.right is ConstantIdentifierNode ||
                    this.right is LiteralNode ||
                    this.right is VariableIdentifierNode))
            {
                right = "(" + this.right.toInfixNotation() + ")";
            }

            return String.Format("{0} {1} {2}", left, operatorString, right);
        }

        protected string toPostfixNotation(String operatorString)
        {
            String left = this.left.toPostfixNotation();
            String right = this.right.toPostfixNotation();

            return String.Format("{0} {1} {2}", left, right, operatorString);
        }
    }
}
