using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Utils;
using OMathParser.Tokens.OXMLTokens;
using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Syntax.Nodes;
using OMathParser.Tokens.OXMLTokens.Abstract;
using OMathParser.Lexical;

namespace OMathParser.Syntax
{
    public class ArgumentTokenListParser : BaseOXMLParser
    {
        private List<SyntaxNode> processedArguments;
        private readonly int argumentsNeeded;

        public ArgumentTokenListParser(ParseProperties properties, TokenList arguments, int nArguments) 
            : base(properties)
        {
            this.processedArguments = new List<SyntaxNode>();
            this.argumentsNeeded = nArguments;
            populateInputQueue(arguments);
        }

        public ArgumentListNode Parse()
        {
            constructArguments();
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


        private void constructArguments()
        {
            while (true)
            {
                IToken current;
                try
                {
                    current = pollNextInput();
                }
                catch (InvalidOperationException ex)
                {
                    if (outputCount() > 0)
                    {
                        constructSingleArgument();
                    }

                    if (processedArguments.Count() < argumentsNeeded)
                    {
                        throw new ParseException("Too few arguments successfully parsed.");
                    }

                    return;
                }

                if (canProduceValue(current))
                {
                    // LexemeTypes: REAL_VALUE, IDENTIFIER_CONST and IDENTIFIER_VAR are processed here
                    pushValueProducerToOutput(current);
                }
                else if (current is Lexeme)
                {
                    Lexeme currentLexeme = current as Lexeme;
                    Lexeme.LexemeType type = currentLexeme.Type;
                    if (type == Lexeme.LexemeType.FUNCTION_NAME)
                    {
                        processFunctionNameLexeme(currentLexeme);
                    }
                    else if (type == Lexeme.LexemeType.LEFT_PAREN)
                    {
                        operatorStack.Push(currentLexeme);
                    }
                    else if (type == Lexeme.LexemeType.RIGHT_PAREN)
                    {
                        processRightParenthesisLexeme(currentLexeme);
                    }
                    else if (type == Lexeme.LexemeType.OP_PLUS)
                    {
                        if (canProcessTokenAsUnaryOp())
                        {
                            pushOperator(new Lexeme(Lexeme.LexemeType.OP_PLUS_UNARY, "+"));
                        }
                        else
                        {
                            pushOperator(currentLexeme);
                        }
                    }
                    else if (type == Lexeme.LexemeType.OP_MINUS)
                    {
                        if (canProcessTokenAsUnaryOp())
                        {
                            pushOperator(new Lexeme(Lexeme.LexemeType.OP_MINUS_UNARY, "-"));
                        }
                        else
                        {
                            pushOperator(currentLexeme);
                        }
                    }
                    else if (type == Lexeme.LexemeType.OP_MUL ||
                             type == Lexeme.LexemeType.OP_DIV ||
                             type == Lexeme.LexemeType.OP_POW)
                    {
                        pushOperator(currentLexeme);
                    }
                    else if (type == Lexeme.LexemeType.ARGUMENT_SEPARATOR)
                    {
                        processArgumentSeparator();
                    }
                    else
                    {
                        //Lexeme.LexemeType.EQ_SIGN not allow inside argument list
                        throw new ParseException("Unknown token type encountered in input.");
                    }
                }
            }
        }

        protected void processArgumentSeparator()
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
                        if (operatorStack.Peek().Type != Lexeme.LexemeType.FUNCTION_NAME)
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

            List<ISyntaxUnit> argumentPostfix = clearOutput();
            lastProcessedElement = null;
            SyntaxNode argumentNode = buildSyntaxTree(argumentPostfix);
            processedArguments.Add(argumentNode);

            if (processedArguments.Count == argumentsNeeded && inputCount() > 0)
            {
                throw new ParseException("Extra tokens in input after processing all function call arguments.");
            }
        }
    }
}
