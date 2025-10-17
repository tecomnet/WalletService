using System.IO.Compression;
using System.Text;

namespace Template.Funcionalidad.Helper;

public static class ZipHelper
{
    public static async Task<byte[]> CreateZipWithJsonFilesAsync(Dictionary<string, string> jsonFiles)
    {
        // Memory stream to hold the zip file in memory
        using (var zipStream = new MemoryStream())
        {
            // Create a new zip archive to write to the memory stream
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var jsonFile in jsonFiles)
                {
                    // Create a new entry in the zip for each JSON file
                    var zipEntry = archive.CreateEntry(jsonFile.Key + ".json", CompressionLevel.Fastest);

                    // Write the JSON content to the entry
                    using (var entryStream = zipEntry.Open())
                    using (var streamWriter = new StreamWriter(entryStream, Encoding.UTF8))
                    {
                        await streamWriter.WriteAsync(jsonFile.Value);
                    }
                }
            }

            // Return the byte array of the zip file
            return zipStream.ToArray();
        }
    }
}