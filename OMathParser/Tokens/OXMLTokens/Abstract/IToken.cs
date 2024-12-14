using OMathParser.Utils;

namespace OMathParser.Tokens.OXMLTokens.Abstract;

public interface IToken : ISimplifiable
{
    IToken? Parent { get ; set; }
}
