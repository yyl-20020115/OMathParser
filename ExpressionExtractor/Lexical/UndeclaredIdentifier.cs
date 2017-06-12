using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMathParser.Lexical
{
    class UndeclaredIdentifier : Lexeme
    {
        public enum Type
        {
            VARIABLE_NAME,
            FUNCTION_NAME,
            VAR_OR_SINGLE_ARG_FUNC
        }

        private Type type;

        public UndeclaredIdentifier(String value, Type t)
            : base(LexemeType.UNDECLARED_IDENTIFIER, value)
        {
            this.type = t;
        }

        public Type UndeclaredIdentifierType { get => type; }
    }
}
