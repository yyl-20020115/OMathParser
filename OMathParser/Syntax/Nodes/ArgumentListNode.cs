using System.Collections;
using System.Text;

using OMathParser.Syntax.Nodes.Abstract;
using OMathParser.Utils;

namespace OMathParser.Syntax.Nodes;

public class ArgumentListNode : ISimplifiable, IEnumerable<SyntaxNode>
{
    private readonly List<SyntaxNode> arguments = [];

    public ArgumentListNode() { }

    public ArgumentListNode(IEnumerable<SyntaxNode> arguments)
    {
        this.arguments = new List<SyntaxNode>(arguments);
    }

    public void AddArgument(SyntaxNode argument) => arguments.Add(argument);

    public SyntaxNode this[int index] => arguments[index];

    public string SimpleRepresentation
    {
        get
        {
            var builder = new StringBuilder();
            foreach (ISimplifiable arg in arguments)
            {
                builder.Append(arg);
                builder.Append(", ");
            }

            if (arguments.Count > 1)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            return $"ArgumentList: [{builder}]";
        }
    }

    public IEnumerator<SyntaxNode> GetEnumerator() => arguments.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => arguments.GetEnumerator();

    public int Count => arguments.Count;

    public double[] CalculatedValues => arguments.Select(arg => arg.Value).ToArray();
}
