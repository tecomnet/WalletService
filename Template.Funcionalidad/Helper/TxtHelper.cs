namespace Template.Funcionalidad.Helper;

public static class TxtHelper
{
    /// <summary>
    /// Crea un byte a partir de un string para ser guardado en bd
    /// </summary>
    /// <param name="fileContent"></param>
    /// <returns></returns>
    public static async Task<byte[]> ConvertStringToByteArrayAsync(string fileContent)
    {
        // Byte to return
        byte[] byteArray;
        // Create memoryStream
        using (var memoryStream = new MemoryStream())
        {
            using (var sw = new StreamWriter(memoryStream))
            {
                await sw.WriteLineAsync(fileContent);
                await sw.FlushAsync();
                byteArray = memoryStream.ToArray();
            }
        }
        // Return the byte array
        return byteArray;
    }
}