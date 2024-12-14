using OMathParser.Tokens.OXMLTokens.Abstract;

namespace OMathParser.Tokens.OXMLTokens;

public class TextRunToken(string text) : AbstractToken()
{
    public string Text { get; set; } = text;

    public override string SimpleRepresentation => $"TextRun: ('{this.Text}')";
}
