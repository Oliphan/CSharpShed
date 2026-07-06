using System.Text;
using Shed.ResourceManagement.Disposal;

namespace Shed.LateInjection.SourceGenerators.Utility;

internal class ScopeManagingStringBuilder
{
    private readonly StringBuilder stringBuilder;
    private readonly Indentation indent;

    public ScopeManagingStringBuilder(
        StringBuilder stringBuilder,
        Indentation indent)
    {
        this.stringBuilder = stringBuilder;
        this.indent = indent;
    }
    
    public IDisposable CurlyScope()
    {
        AppendIndentedLine("{");
        indent.Increment();

        return new DisposeHandler(() =>
        {
            indent.Decrement();
            AppendIndentedLine("}");
        });
    }

    public IDisposable IndentScope()
    {
        indent.Increment();

        return new DisposeHandler(() =>
        {
            indent.Decrement();
        });
    }

    public void AppendIndented(string text)
    {
        stringBuilder.Append(indent.Indent);
        stringBuilder.Append(text);
    }

    public void Append(string text)
        => stringBuilder.Append(text);

    public void AppendLine(string text)
        => stringBuilder.AppendLine(text);

    public void AppendIndentedLine(string text)
    {
        stringBuilder.Append(indent.Indent);
        stringBuilder.AppendLine(text);
    }

    public override string ToString()
        => stringBuilder.ToString();
}

