using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Math;

using OMathParser.Syntax;
using OMathParser.Tokens;


namespace OMathParser
{
    public class OMathParser
    {
        private Queue<OpenXmlElement> expressionElements;

        public OMathParser()
        {
            this.expressionElements = new Queue<OpenXmlElement>();
        }

        private void reset()
        {
            this.expressionElements.Clear();
        }

        public Expression extractExpression(OfficeMath expression)
        {
            this.reset();
            return null;
        }

        private TokenTree extractTokens(OfficeMath expression)
        {
            return TokenTree.fromOpenXML(expression);
        }

        private static IEnumerable<Text> splitText(Text text) {
            List<Text> parts = new List<Text>();
            foreach (char c in text.Text.ToCharArray())
            {
                Text newPart = new Text();
                newPart.Text = c.ToString();
                parts.Add(newPart);
            }
            return parts;
        }
    }
}
