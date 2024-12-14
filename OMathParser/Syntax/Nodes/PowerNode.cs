﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes
{
    public class PowerNode : BinaryNode
    {
        public PowerNode(SyntaxNode exponentBase, SyntaxNode power) 
            : base(left : exponentBase, right : power)
        {
        }

        public override double getValue()
        {
            return Math.Pow(left.getValue(), right.getValue());
        }

        public override string SimpleRepresentation()
        {
            return String.Format("Pow: {0} ^ {1} ", left.SimpleRepresentation(), right.SimpleRepresentation());
        }

        public override string toInfixNotation()
        {
            return base.toInfixNotation("^");
        }

        public override string toPostfixNotation()
        {
            return base.toPostfixNotation("^");
        }
    }
}
