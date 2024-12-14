using DocumentFormat.OpenXml.Packaging;
using OfficeMath = DocumentFormat.OpenXml.Math.OfficeMath;
using OMathParser.Tokens;
using OMathParser.Utils;
using OMathParser.Syntax;
using DocumentFormat.OpenXml;

namespace ConsoleAppTester;

public class Program
{
    static void Main(string[] args)
    {
        var sup = typeof(OpenXmlCompositeElement);
        var root = typeof(OfficeMath);
        var types = new List<Type>();
        foreach(var vt in root.Assembly.GetTypes())
        {
            if (vt.Namespace == "DocumentFormat.OpenXml.Math" && vt.IsSubclassOf(sup))
            {
                Console.WriteLine(vt);
                types.Add(vt);
            }
        }
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        using var writer = new StreamWriter("result.txt");
        foreach (var vt in types)
        {
            writer.WriteLine($"public XElement? Convert({vt.Name}? element)");
            writer.WriteLine("{");
            writer.WriteLine("  return null;");
            writer.WriteLine("}");
        }
        writer.Close();

        var docPath = @"proba - Copy.docx";

        var doc = WordprocessingDocument.Open(docPath, false);
        var docBody = doc.MainDocumentPart?.Document?.Body;

        var parseProperties = BuildProperties();
        var tokenTreeBuilder = new TokenTreeBuilder(parseProperties);
        var syntaxTreeBuilder = new SyntaxTreeBuilder(parseProperties);

        List<OfficeMath> mathExpressions = new(docBody?.Descendants<OfficeMath>() ?? []);
        foreach (var expression in mathExpressions)
        {
            if (string.IsNullOrWhiteSpace(expression.InnerText)) continue;

            Console.WriteLine("OMath paragraph found!");
            Console.WriteLine(System.Xml.Linq.XDocument.Parse(expression.OuterXml).ToString());

            var tokenTree = tokenTreeBuilder.Build(expression);
            var syntaxTree = syntaxTreeBuilder.Build(tokenTree);

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
