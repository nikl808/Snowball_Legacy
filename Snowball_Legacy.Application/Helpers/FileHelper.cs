using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;

namespace Snowball_Legacy.Application.Helpers;

public static class FileHelper
{
    public static byte[] FileToBytes(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
    public static List<IFormFile> BytesDictToFormFiles(Dictionary<string, byte[]> input)
    {
        var files = new List<IFormFile>();
        foreach (var kvp in input)
        {
            var stream = new MemoryStream(kvp.Value);
            var formFile = new FormFile(stream, 0, kvp.Value.Length, "file", kvp.Key);
            files.Add(formFile);
        }
        return files;
    }
}
