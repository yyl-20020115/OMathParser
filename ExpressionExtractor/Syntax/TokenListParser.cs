using System;
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
    class TokenListParser
    {
        private ParseProperties properties;
        private Tokenizer textRunTokenizer;

        private Queue<IToken> input;
        private List<IToken> processed;
        private Queue<ISyntaxUnit> output;
        private Stack<Lexeme> operatorStack;

        private IToken lastProcessedInput;

        public TokenListParser(ParseProperties properties, TokenList tokens)
        {
            this.properties = properties;
            this.textRunTokenizer = new Tokenizer(properties);

            this.input = new Queue<IToken>();
            this.processed = new List<IToken>();
            this.output = new Queue<ISyntaxUnit>();
            this.operatorStack = new Stack<Lexeme>();
            
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

        private IToken peekNextInput()
        {
            return input.Peek();
        }

        private IToken pollNextInput()
        {
            IToken next = input.Dequeue();
            lastProcessedInput = next;
            return next;
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
                        operatorStack.Push(new Lexeme(Lexeme.LexemeType.OP_PLUS_UNARY, "+"));
                    }
                    else
                    {

                    }
                }
            }

        }

        private void pushValueProducerToOutput(IToken t)
        {
            if (t is Lexeme)
            {
                output.Enqueue(processValueProducerLexeme(t as Lexeme));
            }
            else
            {
                if (t is FractionToken)
                {
                    output.Enqueue(processFraction(t as FractionToken));
                }
                else if (t is FunctionApplyToken)
                {
                    output.Enqueue(processFuncApplyToken(t as FunctionApplyToken));
                }
                else if (t is ParenthesesToken)
                {
                    output.Enqueue(processParenthesesToken(t as ParenthesesToken));
                }
                else if (t is SuperscriptToken)
                {
                    output.Enqueue(processSuperscriptToken(t as SuperscriptToken));
                }
                else if (t is RadicalToken)
                {
                    output.Enqueue(processRadicalToken(t as RadicalToken));
                }
            }
        }

        private bool canProduceValue(IToken token)
        {
            if (token is Lexeme)
            {
                Lexeme l = token as Lexeme;
                Lexeme.LexemeType t = l.Type;
                return t == Lexeme.LexemeType.REAL_VALUE ||
                        t == Lexeme.LexemeType.IDENTIFIER_CONST ||
                        t == Lexeme.LexemeType.IDENTIFIER_VAR;
            }
            else
            {
                if (token is FractionToken ||
                    token is FunctionApplyToken ||
                    token is ParenthesesToken ||
                    token is SuperscriptToken ||
                    token is RadicalToken)
                {
                    return true;
                }
            }

            return false;
        }

        private bool canProcessTokenAsUnaryOp()
        {
            if (lastProcessedInput == null)
            {
                return true;
            }
            else if (lastProcessedInput is Lexeme)
            {
                Lexeme previous = lastProcessedInput as Lexeme;
                Lexeme.LexemeType type = previous.Type;
                return type == Lexeme.LexemeType.LEFT_PAREN ||
                        type == Lexeme.LexemeType.EQ_SIGN ||
                        type == Lexeme.LexemeType.OP_DIV ||
                        type == Lexeme.LexemeType.OP_MUL ||
                        type == Lexeme.LexemeType.OP_MINUS ||
                        type == Lexeme.LexemeType.OP_PLUS;
            }

            return false;
        }

        private SyntaxNode processValueProducerLexeme(Lexeme lexeme)
        {
            Lexeme.LexemeType lt = lexeme.Type;
            if (lt == Lexeme.LexemeType.REAL_VALUE)
            {
                double value;
                if (!double.TryParse(lexeme.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    throw new ParseException("Couldn't parse literal value: " + lexeme.Value);
                }
                return new LiteralNode(value);
            }
            else if (lt == Lexeme.LexemeType.IDENTIFIER_VAR)
            {
                return new VariableIdentifierNode(lexeme.Value);
            }
            else if (lt == Lexeme.LexemeType.IDENTIFIER_CONST)
            {
                Double value = properties.getConstantValue(lexeme.Value);
                return new ConstantIdentifierNode(lexeme.Value, value);
            }

            throw new ParseException("Cannot process lexeme " + lexeme.simpleRepresentation() + "as a value producer.");
        }

        private DivisionNode processFraction(FractionToken fraction)
        {
            TokenListParser numeratorParser = new TokenListParser(properties, fraction.Numerator);
            TokenListParser denominatorParser = new TokenListParser(properties, fraction.Denominator);

            SyntaxNode left = numeratorParser.parse();
            SyntaxNode right = numeratorParser.parse();

            return new DivisionNode(left, right);
        }

        private SyntaxNode processFuncApplyToken(FunctionApplyToken func)
        {
            TokenList fNameNode = func.FunctionName;
            if (fNameNode.Count == 1)
            {
                if (fNameNode[0] is TextRunToken)
                {
                    // Samo naziv funkcije u fName, bez ikakvih eksponenata i sl.
                    String functionName = (fNameNode[0] as TextRunToken).Text;
                    if (properties.isFunctionNameDeclared(functionName))
                    {
                        int nArguments = properties.getFunctionArgumentsCount(functionName);
                        ArgumentListNode arguments = parseArgumentList(func.Arguments);

                        if (arguments.Count != nArguments)
                        {
                            throw new ParseException(
                                "Number of arguments given doesn't match function declaration for function: " +
                                func.simpleRepresentation());
                        }

                        FunctionApplyNode.FunctionBody definition = properties.getFunctionDefinition(functionName);
                        return new FunctionApplyNode(arguments, definition, functionName);
                    }
                }
                else if (fNameNode[0] is SuperscriptToken)
                {
                    // eksponent u nazivu funkcije, npr. sin^-1(x)
                    SuperscriptToken sup = fNameNode[0] as SuperscriptToken;
                    if (sup.Base.Count == 1 && sup.Base[0] is TextRunToken)
                    {
                        String functionName = (sup.Base[0] as TextRunToken).Text;
                        if (properties.isFunctionNameDeclared(functionName))
                        {
                            int? nArguments = properties.getFunctionArgumentsCount(functionName);
                            ArgumentListNode arguments = parseArgumentList(func.Arguments);

                            if (arguments.Count != nArguments)
                            {
                                throw new ParseException(
                                    "Number of arguments given doesn't match function declaration for function: " +
                                    func.simpleRepresentation());
                            }

                            TokenListParser exponentParser = new TokenListParser(properties, sup.Argument);
                            SyntaxNode exponentArgument = exponentParser.parse();

                            FunctionApplyNode.FunctionBody definition = properties.getFunctionDefinition(functionName);
                            FunctionApplyNode exponentBase = new FunctionApplyNode(arguments, definition, functionName);

                            return new PowerNode(exponentBase, exponentArgument);
                        }
                    }
                }
            }

            throw new ParseException("Can't process function name: " + func.simpleRepresentation());
        }

        private SyntaxNode processParenthesesToken(ParenthesesToken parentheses)
        {
            TokenListParser listParser = new TokenListParser(properties, parentheses.Elements);
            return listParser.parse();
        }

        private PowerNode processSuperscriptToken(SuperscriptToken superscript)
        {
            TokenListParser baseParser = new TokenListParser(properties, superscript.Base);
            TokenListParser argumentParser = new TokenListParser(properties, superscript.Argument);

            SyntaxNode baseNode = baseParser.parse();
            SyntaxNode argumentNode = argumentParser.parse();

            return new PowerNode(baseNode, argumentNode);
        }

        private RadicalNode processRadicalToken(RadicalToken radical)
        {
            TokenListParser baseParser = new TokenListParser(properties, radical.Base);
            TokenListParser degreeParser = new TokenListParser(properties, radical.Degree);

            SyntaxNode baseNode = baseParser.parse();
            SyntaxNode degreeNode = degreeParser.parse();

            return new RadicalNode(baseNode, degreeNode);
        }

        

        private ArgumentListNode parseArgumentList(TokenList argumentList)
        {
            // TODO:
            throw new NotImplementedException();
        }

        private ArgumentListNode parseArgumentList(ParenthesesToken argumentList)
        {
            // TODO:
            throw new NotImplementedException();
        }

        private void processFunctionNameLexeme(Lexeme fName)
        {
            IToken next = pollNextInput();
            if (next is ParenthesesToken)
            {
                int nArguments = properties.getFunctionArgumentsCount(fName.Value);
                ArgumentListNode arguments = parseArgumentList(next as ParenthesesToken);

                if (arguments.Count != nArguments)
                {
                    throw new ParseException(
                        "Number of arguments given doesn't match function declaration for function: " + fName.Value);
                }

                FunctionApplyNode.FunctionBody funcDefinition = properties.getFunctionDefinition(fName.Value);

                FunctionApplyNode funcApplyNode = new FunctionApplyNode(arguments, funcDefinition, fName.Value);
                output.Enqueue(funcApplyNode);
            }
            else if (next is Lexeme && (next as Lexeme).Type == Lexeme.LexemeType.LEFT_PAREN)
            {
                operatorStack.Push(fName);
                operatorStack.Push(next as Lexeme);
            }
            else
            {
                throw new ParseException("Missing argument list for function call: " 
                    + fName.Value + " " + next.simpleRepresentation());
            }
        }

        private void processRightParenthesisLexeme(Lexeme rightParen)
        {
            while (true)
            {
                IToken popped;
                try
                {
                    popped = operatorStack.Pop();
                }
                catch (InvalidOperationException ex)
                {
                    throw new ParseException("Mismatched parentheses!");
                }

                if (popped is Lexeme && (popped as Lexeme).Type == Lexeme.LexemeType.LEFT_PAREN)
                {
                    try
                    {
                        // Ako je na vrhu stoga ostalo ime funkcije, prebacujemo ga u izlaz
                        IToken stackTop = operatorStack.Peek();
                        if (stackTop is Lexeme && (stackTop as Lexeme).Type == Lexeme.LexemeType.FUNCTION_NAME)
                        {
                            Lexeme funcName = stackTop as Lexeme;
                            output.Enqueue(funcName);
                            operatorStack.Pop();
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        // do nothing, operator stack remains empty
                    }

                    return;
                }
            }
        }

    }
}
