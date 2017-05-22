using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Tokens.Abstract;

namespace OMathParser.Tokens
{
    class TextRunToken : AbstractToken
    {
        private String text;

        public TextRunToken(String text)
            : base()
        {
            this.text = text;
        }

        public String Text { get => text; set => text = value; }

        public override string simpleRepresentation()
        {
            return String.Format("TextRun: ('{0}')", this.text);
        }
    }
}
