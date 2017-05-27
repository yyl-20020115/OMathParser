using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMathParser.Utils
{
    public class ParseProperties
    {
        private ISet<String> identifiers;
        private IDictionary<String, int> functions;
        private ISet<Char> specialCharacters;

        public ParseProperties()
        {
            identifiers = new HashSet<String>();
            functions = new Dictionary<String, int>();
            specialCharacters = new HashSet<Char>();

            populateBasicFunctions();
            populateSpecialCharacters();
        }

        private void populateBasicFunctions()
        {
            functions.Add("sin", 1);
            functions.Add("cos", 1);
            functions.Add("tan", 1);
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

        public void addIdentifier(String identifier)
        {
            this.identifiers.Add(identifier);
        }

        public void addFunction(String functionName, int numArguments)
        {
            functions.Add(functionName.Trim(), numArguments);
        }

        public void addSpecialChar(Char specialChar)
        {
            this.specialCharacters.Add(specialChar);
        }

        public IEnumerable<String> Identifiers => identifiers.AsEnumerable();
        public IEnumerable<KeyValuePair<String, int>> Functions => functions.AsEnumerable();
        public IEnumerable<Char> SpecialChars => specialCharacters.AsEnumerable();
    }
}
