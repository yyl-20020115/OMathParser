using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OMathParser.Syntax.Abstract
{
    abstract class BinaryNode : SyntaxNode
    {
        protected SyntaxNode left;
        protected SyntaxNode right;

        public BinaryNode(SyntaxNode left, SyntaxNode right)
            : base()
        {
            this.left = left;
            this.right = right;
        }
    }
}
