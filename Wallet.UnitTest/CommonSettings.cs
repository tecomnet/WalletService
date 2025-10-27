using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;

namespace Wallet.UnitTest;

public class CommonSettings
{
	// Test line for Gitnuro application
	private const string TestCaseId = "FunctionalTest";
	private static readonly Guid UserId = Guid.NewGuid();
	public readonly List<Cliente> Clientes = [];
	public readonly List<Estado> Estados = [];
	public readonly List<Empresa> Empresas = [];

	public CommonSettings()
	{
        // Create user data
		CrearClientes();
		CrearEstados();
		CrearEmpresas();
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

	private void CrearEstados()
	{
		// Nuevo estado
		var estado = new Estado(
			nombre: "Aguascalientes",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega estado
		Estados.Add(estado);
		// Nuevo estado
		estado = new Estado(
			nombre: "Baja California",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega estado
		Estados.Add(estado);
		// Nuevo estado
		estado = new Estado(
			nombre: "Baja California Sur",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega estado
		Estados.Add(estado);
		// Nuevo estado
		estado = new Estado(
			nombre: "Campeche",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega estado
		Estados.Add(estado);
	}

	private void CrearEmpresas()
	{
		// Nueva empresa
		var empresa = new Empresa(
			nombre: "Tecomnet",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega empresa
		Empresas.Add(empresa);

	}
}