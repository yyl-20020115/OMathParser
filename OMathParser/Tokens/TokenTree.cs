using OMathParser.Tokens.OXMLTokens;
using OMathParser.Lexical;

namespace OMathParser.Tokens;

public class TokenTree(TokenList rootTokens, ISet<Lexeme> identifiers)
{
    private readonly TokenList rootTokens = rootTokens;
    private readonly ISet<Lexeme> identifiers = new HashSet<Lexeme>(identifiers);

    public TokenList RootTokens => rootTokens;
    public IEnumerable<Lexeme> Identifiers => identifiers;
}
