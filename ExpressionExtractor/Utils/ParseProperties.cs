using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax.Nodes;

namespace OMathParser.Utils
{
    public class ParseProperties
    {
        private bool addUndeclaredIdentifiers;
        private ISet<String> identifiers;
        private IDictionary<String, double> constants;
        private IDictionary<String, int> functionDeclarations;
        private IDictionary<String, FunctionApplyNode.FunctionBody> functionDefinitions;
        private ISet<Char> specialCharacters;

        public ParseProperties(bool addUndeclaredIdentifiers = false)
        {
            this.addUndeclaredIdentifiers = addUndeclaredIdentifiers;
            identifiers = new HashSet<String>();
            constants = new Dictionary<String, double>();
            functionDeclarations = new Dictionary<String, int>();
            functionDefinitions = new Dictionary<String, FunctionApplyNode.FunctionBody>();
            specialCharacters = new HashSet<Char>();

            populateConstants();
            populateBasicFunctions();
            populateSpecialCharacters();
        }

        private void populateConstants()
        {
            constants.Add("π", Math.PI);
            constants.Add("e", Math.E);
        }

        private void populateBasicFunctions()
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

        private void populateSpecialCharacters()
        {
            specialCharacters.Add('+');
            specialCharacters.Add('-');
            specialCharacters.Add('*');
            specialCharacters.Add('/');
            specialCharacters.Add('(');
            specialCharacters.Add(')');
        }

        public void addVariableIdentifier(String identifier)
        {
            this.identifiers.Add(identifier);
        }

        public void addConstantIdentifier(String name, double value)
        {
            this.constants.Add(name, value);
        }

        public void AddFunction(String functionName, int numArguments, FunctionApplyNode.FunctionBody definition)
        {
            functionDeclarations.Add(functionName.Trim(), numArguments);
            functionDefinitions.Add(functionName.Trim(), definition);
        }

        public void addSpecialChar(Char specialChar)
        {
            this.specialCharacters.Add(specialChar);
        }

        public Double getConstantValue(String name)
        {
            double value;
            if (constants.TryGetValue(name, out value))
            {
                return value;
            }
            else
            {
                throw new ParseException("No constant declaration found for constant name: " + name);
            }
            
        }

        public bool isConstantIdentifierDeclared(String name)
        {
            return constants.ContainsKey(name);
        }

        public bool isVariableIdentifierDeclared(String name)
        {
            return identifiers.Contains(name);
        }

        public bool isFunctionNameDeclared(String name)
        {
            return functionDeclarations.ContainsKey(name);
        }

        public FunctionApplyNode.FunctionBody getFunctionDefinition(String fName)
        {
            FunctionApplyNode.FunctionBody definition;
            if (functionDefinitions.TryGetValue(fName, out definition))
            {
                return definition;
            }
            else
            {
                throw new ParseException("No function definition found for function name: " + fName);
            }

        }

        public int getFunctionArgumentsCount(String fName)
        {
            int nArguments;
            if (functionDeclarations.TryGetValue(fName, out nArguments))
            {
                return nArguments;
            }
            else
            {
                throw new ParseException("No function declaration found for function name: " + fName);
            }
        }

        public IEnumerable<String> VariableIdentifiers => identifiers.AsEnumerable();
        public IEnumerable<KeyValuePair<String, double>> ConstantIdentifiers => constants.AsEnumerable();
        public IEnumerable<KeyValuePair<String, int>> Functions => functionDeclarations.AsEnumerable();
        public IEnumerable<Char> SpecialChars => specialCharacters.AsEnumerable();
    }
}
