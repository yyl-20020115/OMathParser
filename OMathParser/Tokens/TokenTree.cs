using OMathParser.Tokens.OXMLTokens;
using OMathParser.Lexical;

namespace OMathParser.Tokens;

public class TokenTree(TokenList rootTokens, HashSet<Lexeme> identifiers)
{
    private readonly TokenList rootTokens = rootTokens;
    private readonly HashSet<Lexeme> identifiers = new (identifiers);

    public TokenList RootTokens => rootTokens;
    public IEnumerable<Lexeme> Identifiers => identifiers;
}
