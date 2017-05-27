using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens;
using OMathParser.Utils;

namespace OMathParser.Syntax
{
    class SyntaxTreeBuilder
    {
        private ParseProperties parseProperties;

        public SyntaxTreeBuilder(ParseProperties properties)
        {
            parseProperties = properties;
        }

        public SyntaxTree Build(TokenTree tokenTree)
        {
            return null;
        }


    }
}
