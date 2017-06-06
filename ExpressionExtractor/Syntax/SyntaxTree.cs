using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax
{
    public class SyntaxTree
    {
        private SyntaxNode root;

        public SyntaxTree(SyntaxNode root)
        {
            this.root = root;
        }

        public SyntaxNode RootNode { get => root; }
    }
}
