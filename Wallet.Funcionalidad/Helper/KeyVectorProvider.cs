namespace Wallet.Funcionalidad.Helper
{
    /// <summary>
    /// Proporciona los valores de clave y vector de inicialización para la encriptación AES256CBC.
    /// Esta clase es utilizada para almacenar y acceder a los componentes necesarios para operaciones criptográficas.
    /// </summary>
    public class KeyVectorProvider
    {
        #region Propiedades
        /// <summary>
        /// Obtiene o establece el valor de la clave para el algoritmo de encriptación AES256CBC.
        /// Esta clave es fundamental para el proceso de encriptación y desencriptación.
        /// </summary>
        public string AES256CBCKey { get; set; }

        /// <summary>
        /// Obtiene o establece el valor del vector de inicialización (IV) para el algoritmo de encriptación AES256CBC.
        /// El IV es crucial para asegurar que la encriptación de datos idénticos resulte en textos cifrados diferentes.
        /// </summary>
        public string AES256CBCIV { get; set; }
        #endregion

    }
}