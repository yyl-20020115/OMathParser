using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Utils;

namespace OMathParser.Syntax.Nodes.Abstract
{
    public abstract class SyntaxNode : ISimplifiable, ISyntaxUnit
    {
        protected SyntaxNode parent;

        public abstract double getValue();
        public abstract string simpleRepresentation();

        public SyntaxNode Parent { get => parent; set => parent = value; }

        public override string ToString()
        {
            return this.simpleRepresentation();
        }
    }
}
