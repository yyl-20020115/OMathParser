namespace OMathParser.Utils;

public class LexicalException(string source, int position) : Exception("Cannot tokenize characters from index " + position + " in run: " + source)
{
    public string SourceRun
    {
        get;
    } = source;

    public int Position
    {
        get;
    } = position;

}
