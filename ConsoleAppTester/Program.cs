using DocumentFormat.OpenXml.Packaging;
using OfficeMath = DocumentFormat.OpenXml.Math.OfficeMath;
using OMathParser.Tokens;
using OMathParser.Utils;
using OMathParser.Syntax;


namespace ConsoleAppTester;

public class Program
{
    static void Main(string[] args)
    {
        var docPath = @"proba - Copy.docx";

        var doc = WordprocessingDocument.Open(docPath, false);
        var docBody = doc.MainDocumentPart?.Document?.Body;

        var parseProperties = BuildProperties();
        var tokenTreeBuilder = new TokenTreeBuilder(parseProperties);
        var syntaxTreeBuilder = new SyntaxTreeBuilder(parseProperties);

        List<OfficeMath> mathExpressions = new(docBody.Descendants<OfficeMath>());
        foreach (var expression in mathExpressions)
        {
            if (string.IsNullOrWhiteSpace(expression.InnerText))
            {
                continue;
            }

            Console.WriteLine("OMath paragraph found!");
            Console.WriteLine(System.Xml.Linq.XDocument.Parse(expression.OuterXml).ToString());

            TokenTree tokenTree = tokenTreeBuilder.Build(expression);
            SyntaxTree syntaxTree = syntaxTreeBuilder.Build(tokenTree);

            Console.WriteLine("\nSyntax tree built!");
            Console.WriteLine("Postfix notation: ");
            Console.WriteLine(syntaxTree.ToPostfixNotation());
            Console.WriteLine("Infix notation: ");
            Console.WriteLine(syntaxTree.ToInfixNotation());
            Console.WriteLine("\n====================================================================\n");
        }



        Console.Read();
    }

    public static ParseProperties BuildProperties()
    {
        ParseProperties pp = new();
        pp.AddVariableIdentifier("r");
        pp.AddVariableIdentifier("v");
        pp.AddVariableIdentifier("t");
        pp.AddVariableIdentifier("a");
        pp.AddVariableIdentifier("A");
        pp.AddVariableIdentifier("b");
        pp.AddVariableIdentifier("c");
        pp.AddVariableIdentifier("x");
        pp.AddVariableIdentifier("y");

        pp.AddFunction("f", 1, inputs => inputs[0]);

        return pp;
    }
}
