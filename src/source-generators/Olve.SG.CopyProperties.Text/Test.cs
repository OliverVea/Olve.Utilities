using System.ComponentModel.DataAnnotations;

namespace Olve.SG.CopyProperties.Text;

internal class Source
{
    /// <summary>
    /// Xml comments :D
    /// </summary>
    [Required]
    public int Id2 { get; init; }
    public string Text { get; set; } = string.Empty;
}

[CopyProperties(typeof(Source))]
public partial class DestinationClass
{
}

[CopyProperties(typeof(Source))]
public partial record DestinationRecord
{
}

[CopyProperties(typeof(Source))]
public partial struct DestinationStruct
{
}

[CopyProperties(typeof(Source))]
public partial record struct DestinationRecordStruct
{
}

[CopyProperties(typeof(Source))]
public partial interface DestinationInterface
{
}


public static class Test
{
    public static void Method()
    {
        var dc = new DestinationClass
        {
            Id2 = 1,
            Text = "Hello, World!"
        };
        
        var ds = new DestinationStruct
        {
            Id2 = 2,
            Text = "Goodbye, World!"
        };
    }
}