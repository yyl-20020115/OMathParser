using OMathParser.Syntax.Nodes;

namespace OMathParser.Utils;

public class ParseProperties
{
    private readonly bool addUndeclaredIdentifiers;
    private readonly HashSet<string> identifiers;
    private readonly Dictionary<string, double> constants;
    private readonly Dictionary<string, int> functionDeclarations;
    private readonly Dictionary<string, FunctionApplyNode.FunctionBody> functionDefinitions;
    private readonly HashSet<char> specialCharacters;

    public ParseProperties(bool addUndeclaredIdentifiers = false)
    {
        this.addUndeclaredIdentifiers = addUndeclaredIdentifiers;
        identifiers = [];
        constants = [];
        functionDeclarations = [];
        functionDefinitions = [];
        specialCharacters = [];

        PopulateConstants();
        PopulateBasicFunctions();
        PopulateSpecialCharacters();
    }

    private void PopulateConstants()
    {
        constants.Add("π", Math.PI);
        constants.Add("e", Math.E);
    }

    private void PopulateBasicFunctions()
    {
        functionDeclarations.Add("sin", 1);
        functionDefinitions.Add("sin", arguments => Math.Sin(arguments[0]));

        functionDeclarations.Add("cos", 1);
        functionDefinitions.Add("cos", arguments => Math.Cos(arguments[0]));

        functionDeclarations.Add("tan", 1);
        functionDefinitions.Add("tan", arguments => Math.Tan(arguments[0]));

        functionDeclarations.Add("log", 1);
        functionDefinitions.Add("log", arguments => Math.Log10(arguments[0]));

        functionDeclarations.Add("ln", 1);
        functionDefinitions.Add("ln", arguments => Math.Log(arguments[0]));
    }

    private void PopulateSpecialCharacters()
    {
        SpecialCharacters.Add('+');
        SpecialCharacters.Add('-');
        SpecialCharacters.Add('*');
        SpecialCharacters.Add('/');
        SpecialCharacters.Add('(');
        SpecialCharacters.Add(')');
    }

    public void AddVariableIdentifier(string identifier) => this.identifiers.Add(identifier);

    public void AddConstantIdentifier(string name, double value) => this.constants.Add(name, value);

    public void AddFunction(string functionName, int numArguments, FunctionApplyNode.FunctionBody definition)
    {
        functionDeclarations.Add(functionName.Trim(), numArguments);
        functionDefinitions.Add(functionName.Trim(), definition);
    }

    public void AddSpecialChar(char specialChar)
    {
        this.SpecialCharacters.Add(specialChar);
    }

    public double GetConstantValue(string name) => constants.TryGetValue(name, out var value)
            ? value
            : throw new ParseException("No constant declaration found for constant name: " + name);

    public bool IsConstant(string name) => constants.ContainsKey(name);

    public bool IsVariable(string name) => identifiers.Contains(name);

    public bool IsFunctionName(string name) => functionDeclarations.ContainsKey(name);

    public FunctionApplyNode.FunctionBody GetFunctionDefinition(string fName) 
        => functionDefinitions.TryGetValue(fName, out var definition)
            ? definition
            : throw new ParseException("No function definition found for function name: " + fName);

    public int GetFunctionArgumentsCount(string fName) 
        => functionDeclarations.TryGetValue(fName, out int nArguments)
            ? nArguments
            : throw new ParseException("No function declaration found for function name: " + fName);

    public IEnumerable<string> VariableIdentifiers => identifiers.AsEnumerable();
    public IEnumerable<KeyValuePair<string, double>> ConstantIdentifiers => constants.AsEnumerable();
    public IEnumerable<KeyValuePair<string, int>> Functions => functionDeclarations.AsEnumerable();
    public IEnumerable<char> SpecialChars => SpecialCharacters.AsEnumerable();

    public HashSet<char> SpecialCharacters => specialCharacters;
}
