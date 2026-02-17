using System.IO.Compression;
using Olve.Results;

namespace Olve.OpenRaster.Parsing;

internal static class MimeTypeReader
{
    public static Result<string> ReadMimetype(ZipArchive zipArchive)
    {
        var mimeType = zipArchive.GetEntry("mimetype");

        if (mimeType == null)
        {
            return new ResultProblem("mimetype file was not found in the zip file");
        }

        using var stream = mimeType.Open();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}