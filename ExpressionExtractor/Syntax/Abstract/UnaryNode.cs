using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMathParser.Syntax.Abstract
{
    abstract class UnaryNode : SyntaxNode
    {
        protected SyntaxNode child;

        public UnaryNode(SyntaxNode child)
            : base()
        {
            this.child = child;
        }
    }
}
