﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Abstract;

namespace OMathParser.Syntax
{
    class SyntaxTree
    {
        private SyntaxNode root;

        public SyntaxTree(SyntaxNode root)
        {
            this.root = root;
        }
    }
}
