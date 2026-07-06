namespace Shed.LateInjection.SourceGenerators.Utility;

internal class Indentation
{
    private int levels = 0;
    
    private string indent = string.Empty;

    public string Indent => indent;

    public void Increment(){
        levels++;
        RecalculateIndent();
    }

    public void Decrement()
    {
        levels--;
        RecalculateIndent();
    }

    private void RecalculateIndent()
    {
        indent = new(' ', levels * 4);
    }
}