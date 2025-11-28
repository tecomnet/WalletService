namespace Wallet.DOM.Comun
{
    /// <summary>
    /// Representa un error HTTP que ocurre durante una llamada a una API.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Obtiene el código de estado HTTP de la respuesta.
        /// </summary>
        public int StatusCode { get; private set; }

        /// <summary>
        /// Obtiene el cuerpo de la respuesta HTTP.
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Obtiene los encabezados de la respuesta HTTP.
        /// </summary>
        public System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ApiException"/>.
        /// </summary>
        /// <param name="message">El mensaje de error que explica la razón de la excepción.</param>
        /// <param name="statusCode">El código de estado HTTP.</param>
        /// <param name="response">El cuerpo de la respuesta HTTP.</param>
        /// <param name="headers">Los encabezados de la respuesta HTTP.</param>
        /// <param name="innerException">La excepción subyacente que causó esta excepción.</param>
        public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Exception innerException)
            : base(message: message + "\n\nEstado: " + statusCode + "\nRespuesta: \n" + ((response == null) ? "(nulo)" : response.Substring(startIndex: 0, length: response.Length >= 512 ? 512 : response.Length)), innerException: innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        /// <summary>
        /// Devuelve una representación de cadena del objeto <see cref="ApiException"/> actual.
        /// </summary>
        /// <returns>Una cadena que representa el objeto <see cref="ApiException"/> actual.</returns>
        public override string ToString()
        {
            return string.Format(format: "Respuesta HTTP: \n\n{0}\n\n{1}", arg0: Response, arg1: base.ToString());
        }
    }

    /// <summary>
    /// Representa un error HTTP con un resultado tipado que ocurre durante una llamada a una API.
    /// </summary>
    /// <typeparam name="TResult">El tipo del resultado contenido en la respuesta de error.</typeparam>
    public class ApiException<TResult> : ApiException
    {
        /// <summary>
        /// Obtiene el resultado tipado de la respuesta de error.
        /// </summary>
        public TResult Result { get; private set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ApiException{TResult}"/>.
        /// </summary>
        /// <param name="message">El mensaje de error que explica la razón de la excepción.</param>
        /// <param name="statusCode">El código de estado HTTP.</param>
        /// <param name="response">El cuerpo de la respuesta HTTP.</param>
        /// <param name="headers">Los encabezados de la respuesta HTTP.</param>
        /// <param name="result">El resultado tipado de la respuesta de error.</param>
        /// <param name="innerException">La excepción subyacente que causó esta excepción.</param>
        public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException)
            : base(message: message, statusCode: statusCode, response: response, headers: headers, innerException: innerException)
        {
            Result = result;
        }
    }
}
