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
            String docPath = @"..\..\proba - Copy.docx";

            WordprocessingDocument doc = WordprocessingDocument.Open(docPath, false);
            Body docBody = doc.MainDocumentPart.Document.Body;

            ParseProperties parseProperties = buildProperties();
            TokenTreeBuilder tokenTreeBuilder = new TokenTreeBuilder(parseProperties);
            SyntaxTreeBuilder syntaxTreeBuilder = new SyntaxTreeBuilder(parseProperties);

            List<OfficeMath> mathExpressions = new List<OfficeMath>(docBody.Descendants<OfficeMath>());
            foreach (var expression in mathExpressions)
            {
                if (String.IsNullOrWhiteSpace(expression.InnerText))
                {
                    continue;
                }

                Console.WriteLine("OMath paragraph found!");
                Console.WriteLine(System.Xml.Linq.XDocument.Parse(expression.OuterXml).ToString());

                TokenTree tokenTree = tokenTreeBuilder.build(expression);
                SyntaxTree syntaxTree = syntaxTreeBuilder.Build(tokenTree);

                Console.WriteLine("\nSyntax tree built!");
                Console.WriteLine("Postfix notation: ");
                Console.WriteLine(syntaxTree.toPostfixNotation());
                Console.WriteLine("Infix notation: ");
                Console.WriteLine(syntaxTree.toInfixNotation());
                Console.WriteLine("\n====================================================================\n");
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
