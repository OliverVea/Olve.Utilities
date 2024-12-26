namespace Olve.SG.CopyProperties.Text;

internal class Source
{
    public int Id2 { get; init; }
    public string Text { get; set; } = string.Empty;
}

[CopyProperties(typeof(Source))] public partial class DestinationClass;
[CopyProperties(typeof(Source))] public partial record DestinationRecord;
[CopyProperties(typeof(Source))] public partial struct DestinationStruct;
[CopyProperties(typeof(Source))] public partial record struct DestinationRecordStruct;
[CopyProperties(typeof(Source))] public partial interface IDestinationInterface;

public class InterfaceClass : IDestinationInterface
{
    public int Id2 { get; init; }
    public string Text { get; set; } = string.Empty;
}

public static class Test
{
    public static void Method()
    {
        var dc = new DestinationClass { Id2 = 1, Text = "Hello, World!" };
        var dr = new DestinationRecord { Id2 = 2, Text = "Good morning, World!" };
        var ds = new DestinationStruct { Id2 = 3, Text = "Goodbye, World!" };
        var drs = new DestinationRecordStruct { Id2 = 4, Text = "Goodnight, World!" };
        
        IDestinationInterface ic = new InterfaceClass { Id2 = 5, Text = "Good afternoon, World!" };
        
        var icid2 = ic.Id2;
        var icText = ic.Text;
    }
}