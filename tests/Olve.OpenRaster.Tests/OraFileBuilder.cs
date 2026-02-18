using System.IO.Compression;
using System.Text;

namespace Olve.OpenRaster.Tests;

/// <summary>
/// Builds minimal .ora (OpenRaster) files for testing.
/// An .ora file is a zip archive containing at minimum a stack.xml file.
/// </summary>
internal sealed class OraFileBuilder
{
    private string _stackXml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <image version="0.0.5" w="256" h="128">
          <stack>
            <layer name="Background" src="data/layer0.png" />
          </stack>
        </image>
        """;

    private readonly Dictionary<string, byte[]> _entries = new();

    public OraFileBuilder WithStackXml(string stackXml)
    {
        _stackXml = stackXml;
        return this;
    }

    public OraFileBuilder WithEntry(string path, byte[] data)
    {
        _entries[path] = data;
        return this;
    }

    /// <summary>
    /// Writes the .ora file to the given path.
    /// </summary>
    public string Build(string filePath)
    {
        using var stream = File.Create(filePath);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);

        // mimetype entry (per OpenRaster spec, should be first and uncompressed)
        var mimeEntry = zip.CreateEntry("mimetype", CompressionLevel.NoCompression);
        using (var writer = new StreamWriter(mimeEntry.Open(), Encoding.ASCII))
        {
            writer.Write("image/openraster");
        }

        // stack.xml
        var stackEntry = zip.CreateEntry("stack.xml");
        using (var writer = new StreamWriter(stackEntry.Open(), Encoding.UTF8))
        {
            writer.Write(_stackXml);
        }

        // Additional entries
        foreach (var (path, data) in _entries)
        {
            var entry = zip.CreateEntry(path);
            using var entryStream = entry.Open();
            entryStream.Write(data);
        }

        return filePath;
    }

    /// <summary>
    /// Builds the .ora file to a temporary path and returns the path.
    /// </summary>
    public string Build()
    {
        var path = Path.Combine(Path.GetTempPath(), $"olve-openraster-test-{Guid.NewGuid():N}.ora");
        return Build(path);
    }
}
