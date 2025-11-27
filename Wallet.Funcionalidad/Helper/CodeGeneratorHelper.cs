using System.Text;

namespace Wallet.Funcionalidad.Helper;

public class CodeGeneratorHelper
{
    private static readonly Random Random = new Random();

    /// <summary>
    /// Genera un código de 4 dígitos donde cada dígito es aleatorio (1-9).
    /// Esto asegura que no hay ceros en el código (ej. 1111 a 9999).
    /// </summary>
    /// <returns>Una cadena de 4 caracteres que representa el código.</returns>
    public static string GenerateFourDigitCode()
    {
        // Utilizamos un StringBuilder para construir el código de manera eficiente.
        StringBuilder code = new StringBuilder(4);
        
        // El dígito cero (0) está excluido, por lo que el rango es [1, 10), 
        // lo que genera números enteros del 1 al 9.
        const int minDigit = 1;
        const int maxDigitExclusive = 10; 

        // Generar 4 dígitos
        for (int i = 0; i < 4; i++)
        {
            // Genera un número entre 1 y 9 (ambos inclusive)
            int digit = Random.Next(minDigit, maxDigitExclusive); 
            
            // Concatena el dígito como un carácter
            code.Append(digit);
        }

        return code.ToString();
    }
}
