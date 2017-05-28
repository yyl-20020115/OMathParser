using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Utils;
using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Lexical
{
    public class Lexeme : ISimplifiable, ISyntaxUnit, IToken
    {
        public enum LexemeType {
            IDENTIFIER_VAR,
            IDENTIFIER_CONST,
            FUNCTION_NAME,
            REAL_VALUE,
            LEFT_PAREN,
            RIGHT_PAREN,
            OP_PLUS,
            OP_MINUS,
            OP_MUL,
            OP_DIV,
            EQ_SIGN,
            ARGUMENT_SEPARATOR
        }

        private LexemeType type;
        private String value;
        private IToken parent;

        public Lexeme(LexemeType t, String value)
        {
            this.type = t;
            this.value = value;
        }

        public LexemeType Type { get => type; set => type = value; }
        public String Value { get => value; set => this.value = value; }
        public IToken Parent { get => null; set => this.parent = value; }

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
