using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Utils;

namespace OMathParser.Lexical
{
    public class Lexeme : ISimplifiable
    {
        public enum LexemeType {
            IDENTIFIER,
            FUNCTION_NAME,
            REAL_VALUE,
            LEFT_PAREN,
            RIGHT_PAREN,
            OP_PLUS,
            OP_MINUS,
            OP_MUL,
            OP_DIV,
            EQ_SIGN
        }

        private LexemeType type;
        private String value;

        public Lexeme(LexemeType t, String value)
        {
            this.type = t;
            this.value = value;
        }

        public LexemeType Type { get => type; set => type = value; }
        public String Value { get => value; set => this.value = value; }

        public string simpleRepresentation()
        {
            return String.Format("[Lexeme {0} {1}]", type.ToString(), value);
        }

        public override string ToString()
        {
            return simpleRepresentation();
        }
    }
}
