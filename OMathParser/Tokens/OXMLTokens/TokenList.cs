using System.Text;

using OMathParser.Utils;
using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens;

public class TokenList : List<IToken>, IToken, ISimplifiable
{
    private IToken? parent;

    public IToken? Parent { get => parent; set => parent = value; }

    public TokenList() : base() { }
    public TokenList(IEnumerable<IToken> collection) 
        : base()
    {
        foreach (var item in collection)
        {
            this.Append(item);
        }
    }
    public TokenList(int capacity) : base(capacity) { }

    public void Append(IToken token)
    {
        if (token != null)
        {
            if (token is TokenList list)
            {
                this.AddRange(list);
            }
            else
            {
                this.Add(token);
            }
        }
    }

    public string SimpleRepresentation
    {
        get
        {
            var builder = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                builder.Append(this.ElementAt(i).SimpleRepresentation);
                if (i < this.Count - 1)
                {
                    builder.Append(", ");
                }
            }

            return builder.ToString();
        }
    }

    public override string ToString() => SimpleRepresentation;
}
