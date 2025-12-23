namespace Wallet.Funcionalidad.ServiceClient
{
	/// <summary>
	/// Define los tipos de autorización soportados para las peticiones HTTP.
	/// </summary>
	public enum AuthorizationType
	{
		/// <summary>
		/// Autenticación mediante token Bearer (JWT).
		/// </summary>
		BEARER,

		/// <summary>
		/// Autenticación específica para Postman (si aplica).
		/// </summary>
		POSTMAN,

		/// <summary>
		/// Autenticación mediante API Key.
		/// </summary>
		API_KEY,
	}
}
