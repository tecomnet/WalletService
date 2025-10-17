namespace Template.Funcionalidad.Helper;

/// <summary>
/// 
/// </summary>
public static class NumberHelper
{
    /// <summary>
    /// Valida el valor minimo de un numero
    /// </summary>
    /// <param name="valor">Valor a validar</param>
    /// <param name="valorMinimo">El minimo que debe aceptar el valor</param>
    /// <returns></returns>
    public static bool ValidarValorMinimo(decimal valor, decimal valorMinimo)
    {
        return valor >= valorMinimo;
    }
    
    /// <summary>
    /// Valida si un numero tiene decimales
    /// </summary>
    /// <param name="valor">Valor decimal</param>
    /// <returns></returns>
    public static bool TieneDecimales(decimal valor)
    {
        return valor % 1 != 0;
    }
}