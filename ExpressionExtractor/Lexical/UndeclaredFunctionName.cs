using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMathParser.Lexical
{
    class UndeclaredFunctionName : UndeclaredIdentifier
    {
        private int nArguments;

        public UndeclaredFunctionName(String name, int nArgs)
            : base(name, Type.FUNCTION_NAME)
        {
            this.nArguments = nArgs;
        }
    }
}
