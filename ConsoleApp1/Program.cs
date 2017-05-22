using System;
using System.Collections.Generic;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Validation;
using OfficeMath = DocumentFormat.OpenXml.Math.OfficeMath;

using OMathParser;
using OMathParser.Tokens;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            String docPath = @"D:\Documents\Skola\TVZ\2. sem\OpenXML\proba_clean.docx";

            WordprocessingDocument doc = WordprocessingDocument.Open(docPath, false);
            Body docBody = doc.MainDocumentPart.Document.Body;

            List<OfficeMath> mathExpressions = new List<OfficeMath>(docBody.Descendants<OfficeMath>());
            Console.WriteLine(mathExpressions.Count + " expressions found:");
            foreach (var expression in mathExpressions)
            {
                TokenTree tree = TokenTree.fromOpenXML(expression);
                Console.WriteLine("Tree processed");
            }

            OpenXmlValidator validator = new OpenXmlValidator();
            List<ValidationErrorInfo> errors = new List<ValidationErrorInfo>(validator.Validate(docBody));
            Console.WriteLine("Validator found " + errors.Count + " errors.");
            
            Console.Read();
        }
    }
}
