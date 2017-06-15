using System;
using System.Collections.Generic;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Validation;
using OfficeMath = DocumentFormat.OpenXml.Math.OfficeMath;

using OMathParser;
using OMathParser.Tokens;
using OMathParser.Utils;
using OMathParser.Syntax;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            String docPath = @"D:\Documents\Skola\TVZ\2. sem\OpenXML\proba - Copy.docx";

            WordprocessingDocument doc = WordprocessingDocument.Open(docPath, false);
            Body docBody = doc.MainDocumentPart.Document.Body;

            ParseProperties parseProperties = buildProperties();
            TokenTreeBuilder tokenTreeBuilder = new TokenTreeBuilder(parseProperties);
            SyntaxTreeBuilder syntaxTreeBuilder = new SyntaxTreeBuilder(parseProperties);

            List<OfficeMath> mathExpressions = new List<OfficeMath>(docBody.Descendants<OfficeMath>());
            Console.WriteLine(mathExpressions.Count + " expressions found:");

            foreach (var expression in mathExpressions)
            {
                if (String.IsNullOrWhiteSpace(expression.InnerText))
                {
                    continue;
                }

                TokenTree tokenTree = tokenTreeBuilder.build(expression);
                SyntaxTree syntaxTree = syntaxTreeBuilder.Build(tokenTree);
                Console.WriteLine("Tree built");
            }



            Console.Read();
        }

        public static ParseProperties buildProperties()
        {
            ParseProperties pp = new ParseProperties();
            pp.addVariableIdentifier("r");
            pp.addVariableIdentifier("v");
            pp.addVariableIdentifier("t");
            pp.addVariableIdentifier("a");
            pp.addVariableIdentifier("A");
            pp.addVariableIdentifier("b");
            pp.addVariableIdentifier("c");
            pp.addVariableIdentifier("x");
            pp.addVariableIdentifier("y");

            pp.AddFunction("f", 1, inputs => inputs[0]);

            return pp;
        }
    }
}
