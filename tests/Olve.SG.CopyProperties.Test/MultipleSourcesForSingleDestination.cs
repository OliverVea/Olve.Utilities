namespace Olve.SG.CopyProperties.Text;

public class SourceA
{
    public int A { get; set; }
}

public class SourceB
{
    public int B { get; set; }
}

[CopyProperties(typeof(SourceA))]
[CopyProperties(typeof(SourceB))]
public partial class Destination;

public static class Test_MultipleSourcesForSingleDestination
{
    public static void Method()
    {
        var destination = new Destination { A = 1, B = 2 };
    }
}
