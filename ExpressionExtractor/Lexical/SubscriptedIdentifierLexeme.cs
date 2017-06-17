using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMathParser.Lexical
{
    public class SubscriptedIdentifierLexeme : Lexeme
    {
        private string name;
        private string subscript;

        public SubscriptedIdentifierLexeme(string name, string subscript) 
            : base(LexemeType.IDENTIFIER, name + "_" + subscript)
        {
            this.name = name;
            this.subscript = subscript;
        }

        public string Name => name;
        public string Subscript => subscript;
    }
}
