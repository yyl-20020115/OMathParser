﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Utils;
using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Syntax.Nodes;
using OMathParser.Tokens.OXMLTokens;
using OMathParser.Tokens.OXMLTokens.Abstract;
using OMathParser.Lexical;
using System.Globalization;

namespace OMathParser.Syntax
{
    public class TokenListParser : BaseOXMLParser
    {
        public TokenListParser(ParseProperties properties, TokenList tokens)
            : base(properties)
        {
            populateInputQueue(tokens);
        }

        private void populateInputQueue(TokenList tokens)
        {
            foreach (IToken t in tokens)
            {
                if (t is TextRunToken)
                {
                    String run = (t as TextRunToken).Text;
                    foreach (IToken lexeme in textRunTokenizer.Tokenize(run))
                    {
                        input.Enqueue(lexeme);
                    }
                }
                else
                {
                    input.Enqueue(t);
                }
            }
        }

        public SyntaxNode parse()
        {
            IToken current = pollNextInput();
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
                         type == Lexeme.LexemeType.OP_POW ||
                         type == Lexeme.LexemeType.EQ_SIGN)
                {
                    pushOperator(currentLexeme);
                }
                else if (type == Lexeme.LexemeType.ARGUMENT_SEPARATOR)
                {
                    processArgumentSeparator();
                }
                else
                {
                    throw new ParseException("Unknown token type encountered in input.");
                }
            }

        }
    }
}
