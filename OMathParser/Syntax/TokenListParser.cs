using OMathParser.Utils;
using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Tokens.OXMLTokens;
using OMathParser.Tokens.OXMLTokens.Abstract;
using OMathParser.Lexical;

namespace OMathParser.Syntax;

public class TokenListParser(ParseProperties properties) : BaseOXMLParser(properties)
{
    private List<ISyntaxUnit> ConvertToPostfix(TokenList tokens)
    {
        this.Reset();
        PopulateInputQueue(tokens);

        while (true)
        {
            IToken current;

            if (CanAddImplicitMultiplication())
            {
                current = new Lexeme(Lexeme.LexemeType.OP_MUL, "*");
            }
            else
            {
                try
                {
                    current = PollNextInput();
                }
                catch (InvalidOperationException ex)
                {
                    while (operatorStack.Count > 0)
                    {
                        var op = operatorStack.Pop();
                        output.Enqueue(op);
                    }

                    return new List<ISyntaxUnit>(output);
                }
            }

            if (CanProduceValue(current))
            {
                // LexemeTypes: REAL_VALUE, IDENTIFIER_CONST and IDENTIFIER_VAR are processed here
                PushValueProducerToOutput(current);
            }
            else if (current is Lexeme currentLexeme)
            {
                Lexeme.LexemeType type = currentLexeme.Type;
                if (properties.IsFunctionName(currentLexeme.Value))
                {
                    ProcessFunctionNameLexeme(currentLexeme);
                }
                else if (type == Lexeme.LexemeType.LEFT_PAREN)
                {
                    operatorStack.Push(currentLexeme);
                }
                else if (type == Lexeme.LexemeType.RIGHT_PAREN)
                {
                    ProcessRightParenthesisLexeme(currentLexeme);
                }
                else if (type == Lexeme.LexemeType.OP_PLUS)
                {
                    if (CanProcessTokenAsUnaryOp())
                    {
                        PushOperator(new Lexeme(Lexeme.LexemeType.OP_PLUS_UNARY, "+"));
                    }
                    else
                    {
                        PushOperator(currentLexeme);
                    }
                }
                else if (type == Lexeme.LexemeType.OP_MINUS)
                {
                    if (CanProcessTokenAsUnaryOp())
                    {
                        PushOperator(new Lexeme(Lexeme.LexemeType.OP_MINUS_UNARY, "-"));
                    }
                    else
                    {
                        PushOperator(currentLexeme);
                    }
                }
                else if (type == Lexeme.LexemeType.OP_MUL ||
                         type == Lexeme.LexemeType.OP_DIV ||
                         type == Lexeme.LexemeType.OP_POW ||
                         type == Lexeme.LexemeType.EQ_SIGN)
                {
                    PushOperator(currentLexeme);
                }
                else if (type == Lexeme.LexemeType.ARGUMENT_SEPARATOR)
                {
                    ProcessArgumentSeparator();
                }
                else
                {
                    throw new ParseException("Unknown token type encountered in input.");
                }
            }
        }
    }

    public SyntaxNode Parse(TokenList infix)
    {
        var postfixForm = ConvertToPostfix(infix);
        return BuildSyntaxTree(postfixForm);
    }
}
