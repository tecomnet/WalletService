using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;

namespace Wallet.UnitTest;

public class CommonSettings
{
	// Test line for Gitnuro application
	private const string TestCaseId = "FunctionalTest";
	private static readonly Guid UserId = Guid.NewGuid();
	public readonly List<Cliente> Clientes = [];

	public CommonSettings()
	{
        // Create user data
		CrearClientes();
	}



	private void CrearClientes()
	{
		// Cliente pre registro
		var cliente = new Cliente(
			codigoPais: "+52",
			telefono: "9812078573",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega cliente
		Clientes.Add(cliente);
		// Nuevo cliente con datos completos
		cliente = new Cliente(
			codigoPais: "+52",
			telefono: "1234567890",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega datos personales
		cliente.AgregarDatosPersonales(nombre: "Cliente", primerApellido: "ApellidoPaterno", segundoApellido: "ApellidoMaterno", fechaNacimiento: DateOnly.Parse("1990-01-01"),
			genero: Genero.Masculino, correoElectronico: "cliente@cliente.com", modificationUser: UserId);
		Clientes.Add(cliente);
	}


}