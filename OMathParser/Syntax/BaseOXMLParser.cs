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
using System.Text.RegularExpressions;

namespace OMathParser.Syntax
{
    public abstract class BaseOXMLParser
    {
        protected ParseProperties properties;
        protected Tokenizer textRunTokenizer;

        private Queue<IToken> input;
        protected Queue<ISyntaxUnit> output;
        protected Stack<Lexeme> operatorStack;

        protected ISyntaxUnit lastProcessedElement;
        protected int openedArgumentLists;

        public BaseOXMLParser(ParseProperties properties)
        {
            this.properties = properties;
            this.textRunTokenizer = new Tokenizer(properties);

            this.input = new Queue<IToken>();
            this.output = new Queue<ISyntaxUnit>();
            this.operatorStack = new Stack<Lexeme>();

            this.lastProcessedElement = null;
            this.openedArgumentLists = 0;
        }

        protected void reset()
        {
            input.Clear();
            output.Clear();
            operatorStack.Clear();
            lastProcessedElement = null;
            openedArgumentLists = 0;
        }

        protected void populateInputQueue(TokenList tokens)
        {
            foreach (IToken t in tokens)
            {
                if (t is TextRunToken)
                {
                    string run = (t as TextRunToken).Text;
                    foreach (IToken lexeme in textRunTokenizer.Tokenize(run, true))
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

        protected IToken peekNextInput()
        {
            try
            {
                return input.Peek();
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
            
        }
        
        protected IToken pollNextInput()
        {
            return input.Dequeue();
        }

        protected int inputCount()
        {
            return input.Count;
        }

        protected int outputCount()
        {
            return output.Count;
        }

        protected List<ISyntaxUnit> clearOutput()
        {
            List<ISyntaxUnit> outputCopy = this.output.ToList();
            this.output.Clear();
            return outputCopy;
        }

        protected void pushOperator(Lexeme op)
        {
            if (!op.IsOperator)
            {
                throw new ParseException("Cannot push a non-operator token onto the operator stack!");
            }

            if (op.IsRightAssociative())
            {
                // pushing a right-associative operator
                // pop the top of the stack into the output queue as long as it isn't 
                // an opening parenthesis or its precedence is lower or equal to that of
                // the operator being pushed onto the stack
                try
                {
                    while (true)
                    {
                        Lexeme stackTop = operatorStack.Peek();
                        if (stackTop.Type == Lexeme.LexemeType.LEFT_PAREN)
                        {
                            break;
                        }
                        else if (stackTop.IsHigherPrecedenceThan(op))
                        {
                            output.Enqueue(stackTop);
                            operatorStack.Pop();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    // operator stack is empty, continue with pushing operator
                }

                lastProcessedElement = op;
                operatorStack.Push(op);
            }
            else
            {
                // pushing a left-associative operator
                // pop the top of the stack into the output queue as long as it isn't 
                // an opening parenthesis or its precedence is lower to that of
                // the operator being pushed onto the stack
                try
                {
                    while (true)
                    {
                        Lexeme stackTop = operatorStack.Peek();
                        if (stackTop.Type == Lexeme.LexemeType.LEFT_PAREN)
                        {
                            break;
                        }
                        else if (!stackTop.IsLowerPrecedenceThan(op))
                        {
                            output.Enqueue(stackTop);
                            operatorStack.Pop();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    // operator stack is empty, continue with pushing operator
                }

                lastProcessedElement = op;
                operatorStack.Push(op);
            }
        }

        protected void pushValueProducerToOutput(IToken t)
        {
            ISyntaxUnit processed;
            if (t is Lexeme)
            {
                processed = processValueProducerLexeme(t as Lexeme);
            }
            else
            {
                if (t is FractionToken)
                {
                    processed = processFraction(t as FractionToken);
                }
                else if (t is FunctionApplyToken)
                {
                    processed = processFuncApplyToken(t as FunctionApplyToken);
                }
                else if (t is ParenthesesToken)
                {
                    processed = processParenthesesToken(t as ParenthesesToken);
                }
                else if (t is SuperscriptToken)
                {
                    processed = processSuperscriptToken(t as SuperscriptToken);
                }
                else if (t is RadicalToken)
                {
                    processed = processRadicalToken(t as RadicalToken);
                }
                else if (t is SubscriptToken)
                {
                    // TODO : implementiraj za SubscriptedIdentifierLexeme
                    throw new ParseException("Given token cannot be pushed into the output queue as a value producer.");
                }
                else
                {
                    throw new ParseException("Given token cannot be pushed into the output queue as a value producer.");
                }
            }

            output.Enqueue(processed);
            lastProcessedElement = processed;
        }

        protected bool canProduceValue(Object token)
        {
            if (token is SyntaxNode)
            {
                return true;
            }
            else if (token is Lexeme)
            {
                Lexeme l = token as Lexeme;
                Lexeme.LexemeType t = l.Type;
                return t == Lexeme.LexemeType.REAL_VALUE ||
                        properties.IsConstant(l.Value) ||
                        properties.IsVariable(l.Value);
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
                //else if (token is SubscriptToken)
                //{
                //    return isSubscriptedIdentifier(token as SubscriptToken);
                //}
            }

            return false;
        }

        //private bool isSubscriptedIdentifier(SubscriptToken t)
        //{
        //    TokenList subBase = (token as SubscriptToken).Base;
        //    TokenList subscript = (token as SubscriptToken).Subscript;

        //    if (subBase.Count == 1 && subBase[0] is TextRunToken)
        //    {
        //        String subBaseText = (subBase[0] as TextRunToken).Text;
        //        if (Regex.Match(subBaseText, )
        //            }
        //    // TODO : implementiraj!!!!
        //    return false;
        //}

        protected bool canProcessTokenAsUnaryOp()
        {
            if (lastProcessedElement == null)
            {
                return true;
            }
            else if (lastProcessedElement is Lexeme)
            {
                Lexeme previous = lastProcessedElement as Lexeme;
                Lexeme.LexemeType type = previous.Type;
                return type == Lexeme.LexemeType.LEFT_PAREN ||
                        type == Lexeme.LexemeType.EQ_SIGN ||
                        type == Lexeme.LexemeType.OP_DIV ||
                        type == Lexeme.LexemeType.OP_MUL ||
                        type == Lexeme.LexemeType.OP_MINUS ||
                        type == Lexeme.LexemeType.OP_PLUS ||
                        type == Lexeme.LexemeType.ARGUMENT_SEPARATOR;
            }
            else
            {
                return false;
            }
        }

        protected bool canAddImplicitMultiplication()
        {
            return canProduceValue(lastProcessedElement) &&
                    canProduceValue(peekNextInput());
        }

        protected SyntaxNode processValueProducerLexeme(Lexeme lexeme)
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
            else if (properties.IsVariable(lexeme.Value))
            {
                return new VariableIdentifierNode(lexeme.Value);
            }
            else if (properties.IsConstant(lexeme.Value))
            {
                Double value = properties.getConstantValue(lexeme.Value);
                return new ConstantIdentifierNode(lexeme.Value, value);
            }

            throw new ParseException("Cannot process lexeme " + lexeme.SimpleRepresentation() + "as a value producer.");
        }

        protected DivisionNode processFraction(FractionToken fraction)
        {
            TokenListParser fractionParser = new TokenListParser(properties);
            
            SyntaxNode left = fractionParser.parse(fraction.Numerator);
            SyntaxNode right = fractionParser.parse(fraction.Denominator);
            return new DivisionNode(left, right);
        }

        protected SyntaxNode processFuncApplyToken(FunctionApplyToken func)
        {
            TokenList fNameNode = func.FunctionName;
            if (fNameNode.Count == 1)
            {
                if (fNameNode[0] is Lexeme)
                {
                    // Samo naziv funkcije u fName, bez ikakvih eksponenata i sl.
                    String functionName = (fNameNode[0] as Lexeme).Value;
                    if (properties.IsFunctionName(functionName))
                    {
                        int nArguments = properties.getFunctionArgumentsCount(functionName);
                        ArgumentListNode arguments = parseArgumentList(func.Arguments, nArguments);

                        if (arguments.Count != nArguments)
                        {
                            throw new ParseException(
                                "Number of arguments given doesn't match function declaration for function: " +
                                func.SimpleRepresentation());
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
                        if (properties.IsFunctionName(functionName))
                        {
                            int nArguments = properties.getFunctionArgumentsCount(functionName);
                            ArgumentListNode arguments = parseArgumentList(func.Arguments, nArguments);

                            if (arguments.Count != nArguments)
                            {
                                throw new ParseException(
                                    "Number of arguments given doesn't match function declaration for function: " +
                                    func.SimpleRepresentation());
                            }

                            TokenListParser exponentParser = new TokenListParser(properties);
                            SyntaxNode exponentArgument = exponentParser.parse(sup.Argument);

                            FunctionApplyNode.FunctionBody definition = properties.getFunctionDefinition(functionName);
                            FunctionApplyNode exponentBase = new FunctionApplyNode(arguments, definition, functionName);

                            return new PowerNode(exponentBase, exponentArgument);
                        }
                    }
                }
            }

            throw new ParseException("Can't process function name: " + func.SimpleRepresentation());
        }

        protected SyntaxNode processParenthesesToken(ParenthesesToken parentheses)
        {
            TokenListParser listParser = new TokenListParser(properties);
            return listParser.parse(parentheses.Elements);
        }

        protected PowerNode processSuperscriptToken(SuperscriptToken superscript)
        {
            TokenListParser supParser = new TokenListParser(properties);
            
            SyntaxNode baseNode = supParser.parse(superscript.Base);
            SyntaxNode argumentNode = supParser.parse(superscript.Argument);

            return new PowerNode(baseNode, argumentNode);
        }

        protected RadicalNode processRadicalToken(RadicalToken radical)
        {
            TokenListParser radicalParser = new TokenListParser(properties);

            SyntaxNode baseNode = radicalParser.parse(radical.Base);
            SyntaxNode degreeNode = radicalParser.parse(radical.Degree);

            return new RadicalNode(baseNode, degreeNode);
        }

        protected ArgumentListNode parseArgumentList(TokenList argumentList, int argumentsNeeded)
        {
            ArgumentTokenListParser argumentListParser = new ArgumentTokenListParser(properties);
            return argumentListParser.Parse(argumentList, argumentsNeeded);
        }

        protected ArgumentListNode parseArgumentList(ParenthesesToken argumentList, int argumentsNeeded)
        {
            //List<SyntaxNode> processedArguments = new List<SyntaxNode>();
            ArgumentTokenListParser argumentListParser = new ArgumentTokenListParser(properties);
            return argumentListParser.Parse(argumentList.Elements, argumentsNeeded);
        }

        protected ArgumentListNode parseArgumentList(DelimiterToken argumentList)
        {
            if (argumentList.BeginChar != '(' || argumentList.EndChar != ')' || argumentList.Delimiter != ',')
            {
                throw new ParseException(argumentList.SimpleRepresentation() +
                    " cannot be used as an argument list for a function call.");
            }

            ArgumentListNode argumentListNode = new ArgumentListNode();
            foreach (TokenList argument in argumentList.Elements)
            {
                TokenListParser argumentParser = new TokenListParser(properties);
                SyntaxNode argumentRootNode = argumentParser.parse(argument);
                argumentListNode.addArgument(argumentRootNode);
            }

            return argumentListNode;
        }

        protected void processFunctionNameLexeme(Lexeme fName)
        {
            IToken next = pollNextInput();
            if (next is ParenthesesToken)
            {
                int nArguments = properties.getFunctionArgumentsCount(fName.Value);
                ArgumentListNode arguments = parseArgumentList(next as ParenthesesToken, nArguments);

                if (arguments.Count != nArguments)
                {
                    throw new ParseException(
                        "Number of arguments given doesn't match function declaration for function: " + fName.Value);
                }

                FunctionApplyNode.FunctionBody funcDefinition = properties.getFunctionDefinition(fName.Value);

                FunctionApplyNode funcApplyNode = new FunctionApplyNode(arguments, funcDefinition, fName.Value);
                output.Enqueue(funcApplyNode);
                lastProcessedElement = funcApplyNode;
            }
            else if (next is Lexeme && (next as Lexeme).Type == Lexeme.LexemeType.LEFT_PAREN)
            {
                openedArgumentLists++;
                operatorStack.Push(fName);
                operatorStack.Push(next as Lexeme);
                lastProcessedElement = next as Lexeme;
            }
            else
            {
                throw new ParseException("Missing argument list for function call: "
                    + fName.Value + " " + next.SimpleRepresentation());
            }
        }

        protected void processRightParenthesisLexeme(Lexeme rightParen)
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
                    throw new ParseException("Mismatched parentheses!");
                }

                if (popped.Type == Lexeme.LexemeType.LEFT_PAREN)
                {
                    try
                    {
                        // Ako je na vrhu stoga ostalo ime funkcije, prebacujemo ga u izlaz
                        IToken stackTop = operatorStack.Peek();
                        if (stackTop is Lexeme && properties.IsFunctionName((stackTop as Lexeme).Value))
                        {
                            Lexeme funcName = stackTop as Lexeme;
                            output.Enqueue(funcName);
                            operatorStack.Pop();
                            openedArgumentLists--;
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        // do nothing, operator stack remains empty
                    }

                    return;
                }
                else
                {
                    output.Enqueue(popped);
                }
            }
        }

        protected void processArgumentSeparator()
        {
            if (openedArgumentLists < 1)
            {
                throw new ParseException("Unexpected function argument separator (',') found.");
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

        protected SyntaxNode buildSyntaxTree(List<ISyntaxUnit> postfixForm)
        {
            Queue<ISyntaxUnit> inputQueue = new Queue<ISyntaxUnit>(postfixForm);
            Stack<SyntaxNode> operandStack = new Stack<SyntaxNode>();
            while (inputQueue.Count > 0)
            {
                ISyntaxUnit input = inputQueue.Dequeue();
                if (input is Lexeme)
                {
                    Lexeme token = input as Lexeme;
                    Lexeme.LexemeType ttype = token.Type;
                    if (properties.IsVariable(token.Value))
                    {
                        VariableIdentifierNode variable = new VariableIdentifierNode(token.Value);
                        operandStack.Push(variable);
                    }
                    else if (properties.IsConstant(token.Value))
                    {
                        double constantValue = properties.getConstantValue(token.Value);
                        ConstantIdentifierNode constant = new ConstantIdentifierNode(token.Value, constantValue);
                        operandStack.Push(constant);
                    }
                    else if (properties.IsFunctionName(token.Value))
                    {
                        int nArguments = properties.getFunctionArgumentsCount(token.Value);
                        FunctionApplyNode.FunctionBody funcBody = properties.getFunctionDefinition(token.Value);
                        ArgumentListNode argumentList = new ArgumentListNode();
                        try
                        {
                            for (int i = 0; i < nArguments; i++)
                            {
                                argumentList.addArgument(operandStack.Pop());
                            }
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new ParseException("Not enough operands on operand stack for function call.");
                        }

                        FunctionApplyNode functionCall = new FunctionApplyNode(argumentList, funcBody, token.Value);
                        operandStack.Push(functionCall);
                    }
                    else if (ttype == Lexeme.LexemeType.REAL_VALUE)
                    {
                        double value;
                        if (!double.TryParse(token.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                        {
                            throw new ParseException("Couldn't parse literal value: " + token.Value);
                        }
                        LiteralNode literal = new LiteralNode(value);
                        operandStack.Push(literal);
                    }
                    else if (ttype == Lexeme.LexemeType.OP_PLUS)
                    {
                        try
                        {
                            SyntaxNode right = operandStack.Pop();
                            SyntaxNode left = operandStack.Pop();
                            AdditionNode addition = new AdditionNode(left, right);
                            operandStack.Push(addition);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new ParseException("Missing operand(s) for addition.");
                        }
                    }
                    else if (ttype == Lexeme.LexemeType.OP_MINUS)
                    {
                        try
                        {
                            SyntaxNode right = operandStack.Pop();
                            SyntaxNode left = operandStack.Pop();
                            SubtractionNode subtraction = new SubtractionNode(left, right);
                            operandStack.Push(subtraction);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new ParseException("Missing operand(s) for subtraction.");
                        }
                    }
                    else if (ttype == Lexeme.LexemeType.OP_MUL)
                    {
                        try
                        {
                            SyntaxNode right = operandStack.Pop();
                            SyntaxNode left = operandStack.Pop();
                            MultiplicationNode multiplication = new MultiplicationNode(left, right);
                            operandStack.Push(multiplication);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new ParseException("Missing operand(s) for multiplication.");
                        }
                    }
                    else if (ttype == Lexeme.LexemeType.OP_DIV)
                    {
                        try
                        {
                            SyntaxNode right = operandStack.Pop();
                            SyntaxNode left = operandStack.Pop();
                            DivisionNode division = new DivisionNode(left, right);
                            operandStack.Push(division);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new ParseException("Missing operand(s) for division.");
                        }
                    }
                    else if (ttype == Lexeme.LexemeType.OP_POW)
                    {
                        try
                        {
                            SyntaxNode exponent = operandStack.Pop();
                            SyntaxNode baseNode = operandStack.Pop();
                            PowerNode power = new PowerNode(baseNode, exponent);
                            operandStack.Push(power);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new ParseException("Missing operand(s) for exponentiation.");
                        }
                    }
                    else if (ttype == Lexeme.LexemeType.EQ_SIGN)
                    {
                        try
                        {
                            SyntaxNode right = operandStack.Pop();
                            SyntaxNode left = operandStack.Pop();
                            EqualsNode eqNode = new EqualsNode(left, right);
                            operandStack.Push(eqNode);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new ParseException("Missing operand(s) for assignment.");
                        }
                    }
                    else if (ttype == Lexeme.LexemeType.OP_PLUS_UNARY)
                    {
                        try
                        {
                            SyntaxNode child = operandStack.Pop();
                            UnaryPlusNode unaryPlus = new UnaryPlusNode(child);
                            operandStack.Push(unaryPlus);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new ParseException("Missing operand for unary plus.");
                        }
                    }
                    else if (ttype == Lexeme.LexemeType.OP_MINUS_UNARY)
                    {
                        try
                        {
                            SyntaxNode child = operandStack.Pop();
                            UnaryMinusNode unaryMinus = new UnaryMinusNode(child);
                            operandStack.Push(unaryMinus);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new ParseException("Missing operand for unary minus.");
                        }
                    }
                    else
                    {
                        throw new ParseException("Unexpected token in postfix expression: " + token.SimpleRepresentation());
                    }
                }
                else if (input is SyntaxNode)
                {
                    operandStack.Push(input as SyntaxNode);
                }
                else
                {
                    throw new ParseException("Unexpected object type in postfix expression.");
                }
            }

            if (operandStack.Count == 1)
            {
                return operandStack.Pop();
            }
            else
            {
                throw new ParseException("Too many operands in postfix expression.");
            }
        }
    }
}
