using OMathParser.Lexical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMathParser.Utils
{
    public class UnexpectedLexemeException : ParseException
    {
        private Lexeme unexpectedLexeme;

        public UnexpectedLexemeException(Lexeme unexpected)
            : base("")
        {
            this.unexpectedLexeme = unexpected;
        }

        public UnexpectedLexemeException(Lexeme unexpected, string message)
            : base(message)
        {
            this.unexpectedLexeme = unexpected;
        }

        public Lexeme UnexpectedLexeme { get => unexpectedLexeme; }
    }
}
