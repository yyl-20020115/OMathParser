using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMathParser.Utils
{
    public class LexicalException : Exception
    {
        public LexicalException(String source, int position)
            : base("Cannot tokenize characters from index " + position + " in run: " + source)
        {
            SourceRun = source;
            Position = position;
        }

        public String SourceRun
        {
            get;
        }

        public int Position
        {
            get;
        }

    }
}
