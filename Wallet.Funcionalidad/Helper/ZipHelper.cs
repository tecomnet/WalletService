using System.IO.Compression;
using System.Text;

namespace Wallet.Funcionalidad.Helper;

/// <summary>
/// Proporciona métodos de utilidad para trabajar con archivos ZIP.
/// </summary>
public static class ZipHelper
{
    /// <summary>
    /// Crea un archivo ZIP en memoria que contiene múltiples archivos JSON.
    /// </summary>
    /// <param name="jsonFiles">Un diccionario donde la clave es el nombre del archivo (sin extensión) y el valor es el contenido JSON.</param>
    /// <returns>Un array de bytes que representa el archivo ZIP creado.</returns>
    public static async Task<byte[]> CreateZipWithJsonFilesAsync(Dictionary<string, string> jsonFiles)
    {
        // Stream en memoria para almacenar el archivo ZIP.
        using (var zipStream = new MemoryStream())
        {
            // Crea un nuevo archivo ZIP y lo escribe en el stream de memoria.
            // 'leaveOpen: true' asegura que el zipStream no se cierre cuando el archivo ZIP se deseche,
            // permitiendo que se lea su contenido posteriormente.
            using (var archive = new ZipArchive(stream: zipStream, mode: ZipArchiveMode.Create, leaveOpen: true))
            {
                // Itera sobre cada archivo JSON proporcionado en el diccionario.
                foreach (var jsonFile in jsonFiles)
                {
                    // Crea una nueva entrada en el archivo ZIP para cada archivo JSON.
                    // El nombre de la entrada incluye la clave del diccionario como nombre base y la extensión ".json".
                    // Se utiliza CompressionLevel.Fastest para una compresión rápida.
                    var zipEntry = archive.CreateEntry(entryName: jsonFile.Key + ".json", compressionLevel: CompressionLevel.Fastest);

                    // Abre el stream de la entrada del ZIP para escribir el contenido JSON.
                    using (var entryStream = zipEntry.Open())
                    // Utiliza un StreamWriter para escribir el contenido JSON en el stream de la entrada, asegurando codificación UTF-8.
                    using (var streamWriter = new StreamWriter(stream: entryStream, encoding: Encoding.UTF8))
                    {
                        // Escribe el contenido JSON de forma asíncrona en la entrada del ZIP.
                        await streamWriter.WriteAsync(value: jsonFile.Value);
                    }
                }
            }

            // Una vez que todas las entradas se han escrito y el archivo ZIP está completo,
            // convierte el contenido del stream de memoria a un array de bytes.
            // Este array de bytes es el archivo ZIP completo.
            return zipStream.ToArray();
        }
    }
}