using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMathParser.Utils;
using OMathParser.Tokens.OXMLTokens;

namespace OMathParser.Syntax
{
    class ArgumentListParser : BaseOXMLParser
    {
        public ArgumentListParser(ParseProperties properties, TokenList arguments) : base(properties)
        {
            this.openedArgumentLists = 1;
        }


    }
}
