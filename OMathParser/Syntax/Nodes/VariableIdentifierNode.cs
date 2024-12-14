﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes.Abstract;

namespace OMathParser.Syntax.Nodes
{
    public class VariableIdentifierNode : SyntaxNode
    {
        private double value;
        private String name;

        public VariableIdentifierNode(String name)
        {
            this.name = name;
            this.value = 0;
        }

        public override double getValue()
        {
            return value;
        }

        public void setValue(double value)
        {
            this.value = value;
        }

        public override string SimpleRepresentation()
        {
            return "VariableIdentifier: " + name;
        }

        public override string toInfixNotation()
        {
            return name;
        }

        public override string toPostfixNotation()
        {
            return name;
        }

        public override bool Equals(object obj)
        {
            VariableIdentifierNode other = obj as VariableIdentifierNode;
            if (other != null)
            {
                return this.name.Equals(other.name);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
