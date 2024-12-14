using OMathParser.Utils;
using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Lexical;

public class Lexeme : ISimplifiable, ISyntaxUnit, IToken
{
    public enum LexemeType : int
    {
        IDENTIFIER,
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

    private static readonly Dictionary<LexemeType, int> precedenceMap;

    static Lexeme()
    {
        precedenceMap = new Dictionary<LexemeType, int>
        {
            { LexemeType.IDENTIFIER, -1 },
            { LexemeType.REAL_VALUE, -1 },
            { LexemeType.LEFT_PAREN, -1 },
            { LexemeType.RIGHT_PAREN, -1 },
            { LexemeType.OP_PLUS, 2 },
            { LexemeType.OP_MINUS, 2 },
            { LexemeType.OP_MUL, 3 },
            { LexemeType.OP_DIV, 3 },
            { LexemeType.OP_POW, 4 },
            { LexemeType.EQ_SIGN, 1 },
            { LexemeType.ARGUMENT_SEPARATOR, -1 },
            { LexemeType.OP_PLUS_UNARY, 5 },
            { LexemeType.OP_MINUS_UNARY, 5 }
        };
    }

    private LexemeType type;
    private string value;
    private int precedence;
    private IToken? parent;
    private bool rightAssociative;
    private bool isOperator;

    public Lexeme(LexemeType t, string value)
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
    public string Value { get => value; set => this.value = value; }
    public IToken? Parent { get => null; set => this.parent = value; }

    public bool IsOperator => this.isOperator;

    public bool IsHigherPrecedenceThan(Lexeme other) => this.precedence > other.precedence;

    public bool IsEqualPrecedence(Lexeme other) => this.precedence == other.precedence;

    public bool IsLowerPrecedenceThan(Lexeme other) => this.precedence < other.precedence;

    public bool IsRightAssociative() => this.rightAssociative;

    public string SimpleRepresentation => $"[Lexeme {type.ToString()} {value}]";

    public override string ToString() => SimpleRepresentation;
}
