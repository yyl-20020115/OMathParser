using System.Text;

using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens;

public class DelimiterToken(char beginChar, char endChar, char delimiter) : AbstractToken
{
    private readonly List<IToken> elements = [];
    private readonly char beginChar = beginChar;
    private readonly char endChar = endChar;
    private readonly char delimiter = delimiter;

    public void AddElement(IToken element) => this.elements.Add(element);

    public char BeginChar => this.beginChar;
    public char EndChar => this.endChar;
    public char Delimiter => this.delimiter;
    public List<IToken> Elements => this.elements;

    public override string SimpleRepresentation
    {
        get
        {
            var builder = new StringBuilder();
            foreach (IToken t in elements)
            {
                builder.Append(t.SimpleRepresentation);
                builder.Append(this.delimiter);
                builder.Append(' ');
            }

            if (elements.Count > 1)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            return $"Delimiter: {beginChar}{builder}{endChar}";
        }
    }
}
