using OMathParser.Utils;
using OMathParser.Tokens.OXMLTokens;
using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Syntax.Nodes;
using OMathParser.Tokens.OXMLTokens.Abstract;
using OMathParser.Lexical;

namespace OMathParser.Syntax;

public class ArgumentTokenListParser(ParseProperties properties) : BaseOXMLParser(properties)
{
    private readonly List<SyntaxNode> processedArguments = [];
    private int argumentsNeeded;

    private void Reset(int argumentsNeeded)
    {
        base.Reset();
        processedArguments.Clear();
        this.argumentsNeeded = argumentsNeeded;
    }

    public ArgumentListNode Parse(TokenList arguments, int nArguments)
    {
        this.Reset(nArguments);
        PopulateInputQueue(arguments);

        ConstructArguments();

        if (processedArguments.Count() < argumentsNeeded)
        {
            throw new ParseException("Too few arguments given for function call.");
        }
        else if (processedArguments.Count() > argumentsNeeded)
        {
            throw new ParseException("Too many arguments given for function call.");
        }
        else
        {
            return new ArgumentListNode(processedArguments);
        }
    }


    private void ConstructArguments()
    {
        while (true)
        {
            IToken current;
            try
            {
                current = PollNextInput();
            }
            catch (InvalidOperationException ex)
            {
                if (OutputCount > 0)
                {
                    constructSingleArgument();
                }

                if (processedArguments.Count() < argumentsNeeded)
                {
                    throw new ParseException("Too few arguments successfully parsed.");
                }

                return;
            }

            if (CanProduceValue(current))
            {
                // LexemeTypes: REAL_VALUE, IDENTIFIER_CONST and IDENTIFIER_VAR are processed here
                PushValueProducerToOutput(current);
            }
            else if (current is Lexeme)
            {
                Lexeme currentLexeme = current as Lexeme;
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
                         type == Lexeme.LexemeType.OP_POW)
                {
                    PushOperator(currentLexeme);
                }
                else if (type == Lexeme.LexemeType.ARGUMENT_SEPARATOR)
                {
                    ProcessArgumentSeparator();
                }
                else
                {
                    //Lexeme.LexemeType.EQ_SIGN not allow inside argument list
                    throw new ParseException("Unknown token type encountered in input.");
                }
            }
        }
    }

    protected void ProcessArgumentSeparator()
    {
        if (openedArgumentLists == 0)
        {
            // The eparator separates main argument list elements (not in a subexpression).
            // That means we've reached the end of an argument expression.
            // In that case the operator stack is emptied and the resulting postfix output
            // represents a single argument which needs to be converted to a syntax tree
            // and added to the processed arguments list.
            constructSingleArgument();
        }
        else
        {
            while (true)
            {
                Lexeme popped;
                try
                {
                    popped = operatorStack.Pop();
                }
                catch (InvalidOperationException ex)
                {
                    throw new ParseException("Unexpected function argument separator (',') found.");
                }

                if (popped.Type == Lexeme.LexemeType.LEFT_PAREN)
                {
                    if (!properties.IsFunctionName(operatorStack.Peek().Value))
                    {
                        throw new ParseException("Unexpected function argument separator (',') found.");
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    output.Enqueue(popped);
                }
            }
        }
    }

    private void constructSingleArgument()
    {
        while (true)
        {
            Lexeme popped;
            try
            {
                popped = operatorStack.Pop();
                output.Enqueue(popped);
            }
            catch (InvalidOperationException ex)
            {
                // do nothing, the stack has been emptied, moving on
                break;
            }
        }

        List<ISyntaxUnit> argumentPostfix = ClearOutput();
        lastProcessedElement = null;
        SyntaxNode argumentNode = BuildSyntaxTree(argumentPostfix);
        processedArguments.Add(argumentNode);

        if (processedArguments.Count == argumentsNeeded && InputCount > 0)
        {
            throw new ParseException("Extra tokens in input after processing all function call arguments.");
        }
    }
}
