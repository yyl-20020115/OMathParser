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
            OP_POW,
            EQ_SIGN,
            ARGUMENT_SEPARATOR,
            OP_PLUS_UNARY,
            OP_MINUS_UNARY
        }

        private static Dictionary<LexemeType, int> precedenceMap;

        static Lexeme()
        {
            precedenceMap = new Dictionary<LexemeType, int>();
            precedenceMap.Add(LexemeType.IDENTIFIER_VAR, -1);
            precedenceMap.Add(LexemeType.IDENTIFIER_CONST, -1);
            precedenceMap.Add(LexemeType.FUNCTION_NAME, -1);
            precedenceMap.Add(LexemeType.REAL_VALUE, -1);
            precedenceMap.Add(LexemeType.LEFT_PAREN, -1);
            precedenceMap.Add(LexemeType.RIGHT_PAREN, -1);
            precedenceMap.Add(LexemeType.OP_PLUS, 2);
            precedenceMap.Add(LexemeType.OP_MINUS, 2);
            precedenceMap.Add(LexemeType.OP_MUL, 3);
            precedenceMap.Add(LexemeType.OP_DIV, 3);
            precedenceMap.Add(LexemeType.OP_POW, 4);
            precedenceMap.Add(LexemeType.EQ_SIGN, 1);
            precedenceMap.Add(LexemeType.ARGUMENT_SEPARATOR, -1);
            precedenceMap.Add(LexemeType.OP_PLUS_UNARY, 5);
            precedenceMap.Add(LexemeType.OP_MINUS_UNARY, 5);
        }

        private LexemeType type;
        private String value;
        private int precedence;
        private IToken parent;
        private bool rightAssociative;
        private bool isOperator;

        public Lexeme(LexemeType t, String value)
        {
            this.type = t;
            this.value = value;
            this.precedence = precedenceMap[t];
            this.rightAssociative = false;
            this.isOperator = false;

            if (t == LexemeType.OP_POW || t == LexemeType.OP_PLUS_UNARY || t == LexemeType.OP_MINUS_UNARY)
            {
                this.rightAssociative = true;
            }

            if (t == LexemeType.EQ_SIGN ||
                t == LexemeType.OP_DIV ||
                t == LexemeType.OP_MINUS ||
                t == LexemeType.OP_MINUS_UNARY ||
                t == LexemeType.OP_MUL ||
                t == LexemeType.OP_PLUS ||
                t == LexemeType.OP_PLUS_UNARY ||
                t == LexemeType.OP_POW)
            {
                this.isOperator = true;
            }
        }

        public LexemeType Type { get => type; set => type = value; }
        public String Value { get => value; set => this.value = value; }
        public IToken Parent { get => null; set => this.parent = value; }

        public bool IsOperator()
        {
            return this.isOperator;
        }
         
        public bool IsHigherPrecedence(Lexeme other)
        {
            return this.precedence > other.precedence;
        }

        public bool IsEqualPrecedence(Lexeme other)
        {
            return this.precedence == other.precedence;
        }

        public bool IsLowerPrecedence(Lexeme other)
        {
            return this.precedence < other.precedence;
        }

        public bool IsRightAssociative()
        {
            return this.rightAssociative;
        }

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
