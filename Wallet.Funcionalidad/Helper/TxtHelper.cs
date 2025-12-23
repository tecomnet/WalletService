namespace Wallet.Funcionalidad.Helper;

public static class TxtHelper
{
    /// <summary>
    /// Convierte el contenido de un string a un array de bytes de forma asíncrona.
    /// Este método es útil para preparar datos de texto para ser almacenados en una base de datos u otro medio binario.
    /// </summary>
    /// <param name="fileContent">El contenido del string a convertir en un array de bytes.</param>
    /// <returns>Un <see cref="Task{TResult}"/> que representa la operación asíncrona, conteniendo el array de bytes resultante.</returns>
    public static async Task<byte[]> ConvertStringToByteArrayAsync(string fileContent)
    {
        // Array de bytes que contendrá el resultado final.
        byte[] byteArray;

        // Se utiliza un MemoryStream para escribir el contenido del string y luego obtener sus bytes.
        using (var memoryStream = new MemoryStream())
        {
            // Se utiliza un StreamWriter para escribir el string en el MemoryStream.
            // Se especifica el stream para asegurar que se escribe correctamente.
            using (var sw = new StreamWriter(stream: memoryStream))
            {
                // Escribe el string en el stream de forma asíncrona, añadiendo un salto de línea al final.
                await sw.WriteLineAsync(value: fileContent);
                // Asegura que todos los datos buffered se escriban en el stream subyacente.
                await sw.FlushAsync();
                // Convierte el contenido del MemoryStream a un array de bytes.
                byteArray = memoryStream.ToArray();
            }
        }
        // Retorna el array de bytes generado.
        return byteArray;
    }
}