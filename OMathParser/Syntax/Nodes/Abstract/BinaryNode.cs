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

        protected string toInfixNotation(string operatorString)
        {
            string left = this.left.toInfixNotation();
            string right = this.right.toInfixNotation();

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

            return string.Format("{0} {1} {2}", left, operatorString, right);
        }

        protected string toPostfixNotation(string operatorString)
        {
            string left = this.left.toPostfixNotation();
            string right = this.right.toPostfixNotation();

            return string.Format("{0} {1} {2}", left, right, operatorString);
        }
    }
}
