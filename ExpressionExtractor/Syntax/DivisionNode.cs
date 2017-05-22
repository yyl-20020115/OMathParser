﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Abstract;

namespace OMathParser.Syntax
{
    class DivisionNode : BinaryNode
    {
        public DivisionNode(SyntaxNode left, SyntaxNode right) : base(left, right)
        {
        }

        public override double getValue()
        {
            return left.getValue() / right.getValue();
        }

        public override string simpleRepresentation()
        {
            return String.Format("Div: {0} / {1} ", left.simpleRepresentation(), right.simpleRepresentation());
        }
    }
}
