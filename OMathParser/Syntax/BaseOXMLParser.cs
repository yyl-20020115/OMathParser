using OMathParser.Utils;
using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Syntax.Nodes;
using OMathParser.Tokens.OXMLTokens;
using OMathParser.Tokens.OXMLTokens.Abstract;
using OMathParser.Lexical;
using System.Globalization;

namespace OMathParser.Syntax;

public abstract class BaseOXMLParser(ParseProperties properties)
{
    protected ParseProperties properties = properties;
    protected Tokenizer textRunTokenizer = new(properties);

    private readonly Queue<IToken> input = new();
    protected Queue<ISyntaxUnit> output = new();
    protected Stack<Lexeme> operatorStack = new();

    protected ISyntaxUnit? lastProcessedElement = null;
    protected int openedArgumentLists = 0;

    protected void Reset()
    {
        input.Clear();
        output.Clear();
        operatorStack.Clear();
        lastProcessedElement = null;
        openedArgumentLists = 0;
    }

    protected void PopulateInputQueue(TokenList tokens)
    {
        foreach (IToken t in tokens)
        {
            if (t is TextRunToken token)
            {
                var run = token.Text;
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

    protected IToken? PeekNextInput()
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

    protected IToken PollNextInput() => input.Dequeue();

    protected int InputCount => input.Count;

    protected int OutputCount => output.Count;

    protected List<ISyntaxUnit> ClearOutput()
    {
        List<ISyntaxUnit> outputCopy = [.. this.output];
        this.output.Clear();
        return outputCopy;
    }

    protected void PushOperator(Lexeme op)
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
                    var stackTop = operatorStack.Peek();
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
                    var stackTop = operatorStack.Peek();
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

    protected void PushValueProducerToOutput(IToken t)
    {
        ISyntaxUnit processed;
        if (t is Lexeme lexeme)
        {
            processed = ProcessValueProducerLexeme(lexeme);
        }
        else
        {
            if (t is FractionToken ft)
            {
                processed = ProcessFraction(ft);
            }
            else if (t is FunctionApplyToken fa)
            {
                processed = ProcessFuncApplyToken(fa);
            }
            else if (t is ParenthesesToken pt)
            {
                processed = ProcessParenthesesToken(pt);
            }
            else if (t is SuperscriptToken st)
            {
                processed = ProcessSuperscriptToken(st);
            }
            else if (t is RadicalToken rt)
            {
                processed = ProcessRadicalToken(rt);
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

    protected bool CanProduceValue(Object? token)
    {
        if (token is SyntaxNode)
        {
            return true;
        }
        else if (token is Lexeme lexme)
        {
            Lexeme.LexemeType t = lexme.Type;
            return t == Lexeme.LexemeType.REAL_VALUE ||
                    properties.IsConstant(lexme.Value) ||
                    properties.IsVariable(lexme.Value);
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
    //        string subBaseText = (subBase[0] as TextRunToken).Text;
    //        if (Regex.Match(subBaseText, )
    //            }
    //    // TODO : implementiraj!!!!
    //    return false;
    //}

    protected bool CanProcessTokenAsUnaryOp()
    {
        if (lastProcessedElement == null)
        {
            return true;
        }
        else if (lastProcessedElement is Lexeme previous)
        {
            return previous.Type is
                Lexeme.LexemeType.LEFT_PAREN or
                Lexeme.LexemeType.EQ_SIGN or
                Lexeme.LexemeType.OP_DIV or
                Lexeme.LexemeType.OP_MUL or
                Lexeme.LexemeType.OP_MINUS or
                Lexeme.LexemeType.OP_PLUS or
                Lexeme.LexemeType.ARGUMENT_SEPARATOR
                ;
        }
        else
        {
            return false;
        }
    }

    protected bool CanAddImplicitMultiplication()
    {
        return CanProduceValue(lastProcessedElement) &&
                CanProduceValue(PeekNextInput());
    }

    protected SyntaxNode ProcessValueProducerLexeme(Lexeme lexeme)
    {
        Lexeme.LexemeType lt = lexeme.Type;
        if (lt == Lexeme.LexemeType.REAL_VALUE)
        {
            if (!double.TryParse(lexeme.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                throw new ParseException($"Couldn't parse literal value: {lexeme.Value}");
            }
            return new LiteralNode(value);
        }
        else if (properties.IsVariable(lexeme.Value))
        {
            return new VariableIdentifierNode(lexeme.Value);
        }
        else if (properties.IsConstant(lexeme.Value))
        {
            var value = properties.GetConstantValue(lexeme.Value);
            return new ConstantIdentifierNode(lexeme.Value, value);
        }

        throw new ParseException("Cannot process lexeme " + lexeme.SimpleRepresentation + "as a value producer.");
    }

    protected DivisionNode ProcessFraction(FractionToken fraction)
    {
        TokenListParser fractionParser = new (properties);

        SyntaxNode left = fractionParser.Parse(fraction.Numerator);
        SyntaxNode right = fractionParser.Parse(fraction.Denominator);
        return new DivisionNode(left, right);
    }

    protected SyntaxNode ProcessFuncApplyToken(FunctionApplyToken func)
    {
        TokenList fNameNode = func.FunctionName;
        if (fNameNode.Count == 1)
        {
            if (fNameNode[0] is Lexeme lexeme)
            {
                // Samo naziv funkcije u fName, bez ikakvih eksponenata i sl.
                var functionName = lexeme.Value;
                if (properties.IsFunctionName(functionName))
                {
                    int nArguments = properties.GetFunctionArgumentsCount(functionName);
                    ArgumentListNode arguments = ParseArgumentList(func.Arguments, nArguments);

                    if (arguments.Count != nArguments)
                    {
                        throw new ParseException(
                            "Number of arguments given doesn't match function declaration for function: " +
                            func.SimpleRepresentation);
                    }

                    FunctionApplyNode.FunctionBody definition = properties.GetFunctionDefinition(functionName);
                    return new FunctionApplyNode(arguments, definition, functionName);
                }
            }
            else if (fNameNode[0] is SuperscriptToken sup)
            {
                // eksponent u nazivu funkcije, npr. sin^-1(x)
                if (sup.Base.Count == 1 && sup.Base[0] is TextRunToken text)
                {
                    string functionName = text.Text;
                    if (properties.IsFunctionName(functionName))
                    {
                        int nArguments = properties.GetFunctionArgumentsCount(functionName);
                        ArgumentListNode arguments = ParseArgumentList(func.Arguments, nArguments);

                        if (arguments.Count != nArguments)
                        {
                            throw new ParseException(
                                "Number of arguments given doesn't match function declaration for function: " +
                                func.SimpleRepresentation);
                        }

                        TokenListParser exponentParser = new(properties);
                        SyntaxNode exponentArgument = exponentParser.Parse(sup.Argument);

                        FunctionApplyNode.FunctionBody definition = properties.GetFunctionDefinition(functionName);
                        FunctionApplyNode exponentBase = new (arguments, definition, functionName);

                        return new PowerNode(exponentBase, exponentArgument);
                    }
                }
            }
        }

        throw new ParseException($"Can't process function name: {func.SimpleRepresentation}");
    }

    protected SyntaxNode ProcessParenthesesToken(ParenthesesToken parentheses)
    {
        TokenListParser listParser = new(properties);
        return listParser.Parse(parentheses.Elements);
    }

    protected PowerNode ProcessSuperscriptToken(SuperscriptToken superscript)
    {
        TokenListParser supParser = new(properties);

        SyntaxNode baseNode = supParser.Parse(superscript.Base);
        SyntaxNode argumentNode = supParser.Parse(superscript.Argument);

        return new PowerNode(baseNode, argumentNode);
    }

    protected RadicalNode ProcessRadicalToken(RadicalToken radical)
    {
        TokenListParser radicalParser = new(properties);

        SyntaxNode baseNode = radicalParser.Parse(radical.Base);
        SyntaxNode degreeNode = radicalParser.Parse(radical.Degree);

        return new RadicalNode(baseNode, degreeNode);
    }

    protected ArgumentListNode ParseArgumentList(TokenList argumentList, int argumentsNeeded)
    {
        ArgumentTokenListParser argumentListParser = new(properties);
        return argumentListParser.Parse(argumentList, argumentsNeeded);
    }

    protected ArgumentListNode ParseArgumentList(ParenthesesToken argumentList, int argumentsNeeded)
    {
        //List<SyntaxNode> processedArguments = new List<SyntaxNode>();
        ArgumentTokenListParser argumentListParser = new(properties);
        return argumentListParser.Parse(argumentList.Elements, argumentsNeeded);
    }

    protected ArgumentListNode ParseArgumentList(DelimiterToken argumentList)
    {
        if (argumentList.BeginChar != '(' || argumentList.EndChar != ')' || argumentList.Delimiter != ',')
        {
            throw new ParseException($"{argumentList.SimpleRepresentation} cannot be used as an argument list for a function call.");
        }

        ArgumentListNode argumentListNode = new();
        foreach (TokenList argument in argumentList.Elements)
        {
            TokenListParser argumentParser = new(properties);
            SyntaxNode argumentRootNode = argumentParser.Parse(argument);
            argumentListNode.AddArgument(argumentRootNode);
        }

        return argumentListNode;
    }

    protected void ProcessFunctionNameLexeme(Lexeme fName)
    {
        IToken next = PollNextInput();
        if (next is ParenthesesToken p)
        {
            int nArguments = properties.GetFunctionArgumentsCount(fName.Value);
            ArgumentListNode arguments = ParseArgumentList(p, nArguments);

            if (arguments.Count != nArguments)
            {
                throw new ParseException(
                    $"Number of arguments given doesn't match function declaration for function: {fName.Value}");
            }

            FunctionApplyNode.FunctionBody funcDefinition = properties.GetFunctionDefinition(fName.Value);

            FunctionApplyNode funcApplyNode = new(arguments, funcDefinition, fName.Value);
            output.Enqueue(funcApplyNode);
            lastProcessedElement = funcApplyNode;
        }
        else if (next is Lexeme lexeme && lexeme.Type == Lexeme.LexemeType.LEFT_PAREN)
        {
            openedArgumentLists++;
            operatorStack.Push(fName);
            operatorStack.Push(lexeme);
            lastProcessedElement = lexeme;
        }
        else
        {
            throw new ParseException($"Missing argument list for function call: {fName.Value} {next.SimpleRepresentation}");
        }
    }

    protected void ProcessRightParenthesisLexeme(Lexeme rightParen)
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
                    if (stackTop is Lexeme funcName && properties.IsFunctionName(funcName.Value))
                    {
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

    protected virtual void ProcessArgumentSeparator()
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

    protected SyntaxNode BuildSyntaxTree(List<ISyntaxUnit> postfixForm)
    {
        Queue<ISyntaxUnit> inputQueue = new(postfixForm);
        Stack<SyntaxNode> operandStack = new();
        while (inputQueue.Count > 0)
        {
            ISyntaxUnit input = inputQueue.Dequeue();
            if (input is Lexeme token)
            {
                Lexeme.LexemeType ttype = token.Type;
                if (properties.IsVariable(token.Value))
                {
                    VariableIdentifierNode variable = new (token.Value);
                    operandStack.Push(variable);
                }
                else if (properties.IsConstant(token.Value))
                {
                    double constantValue = properties.GetConstantValue(token.Value);
                    ConstantIdentifierNode constant = new (token.Value, constantValue);
                    operandStack.Push(constant);
                }
                else if (properties.IsFunctionName(token.Value))
                {
                    int nArguments = properties.GetFunctionArgumentsCount(token.Value);
                    FunctionApplyNode.FunctionBody funcBody = properties.GetFunctionDefinition(token.Value);
                    ArgumentListNode argumentList = new ();
                    try
                    {
                        for (int i = 0; i < nArguments; i++)
                        {
                            argumentList.AddArgument(operandStack.Pop());
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new ParseException("Not enough operands on operand stack for function call.");
                    }

                    FunctionApplyNode functionCall = new (argumentList, funcBody, token.Value);
                    operandStack.Push(functionCall);
                }
                else if (ttype == Lexeme.LexemeType.REAL_VALUE)
                {
                    if (!double.TryParse(token.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        throw new ParseException("Couldn't parse literal value: " + token.Value);
                    }
                    LiteralNode literal = new (value);
                    operandStack.Push(literal);
                }
                else if (ttype == Lexeme.LexemeType.OP_PLUS)
                {
                    try
                    {
                        SyntaxNode right = operandStack.Pop();
                        SyntaxNode left = operandStack.Pop();
                        AdditionNode addition = new (left, right);
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
                        SubtractionNode subtraction = new (left, right);
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
                        MultiplicationNode multiplication = new (left, right);
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
                        DivisionNode division = new (left, right);
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
                        PowerNode power = new (baseNode, exponent);
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
                        EqualsNode eqNode = new (left, right);
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
                        UnaryPlusNode unaryPlus = new (child);
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
                        UnaryMinusNode unaryMinus = new (child);
                        operandStack.Push(unaryMinus);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new ParseException("Missing operand for unary minus.");
                    }
                }
                else
                {
                    throw new ParseException("Unexpected token in postfix expression: " + token.SimpleRepresentation);
                }
            }
            else if (input is SyntaxNode node)
            {
                operandStack.Push(node);
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
