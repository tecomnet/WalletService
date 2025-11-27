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
		CrearEmpresas();
		CrearEstados();
		CrearClientes();
		CrearProveedoresServicios();
	}


	private void CrearClientes()
	{
		var tecomnet = Empresas.First(e => e.Nombre == "Tecomnet");
		// Cliente pre registro
		var cliente = new Cliente(
			codigoPais: "+52",
			telefono: "9812078573",
			creationUser: UserId,
			testCase: TestCaseId);
		cliente.AgregarEmpresa(tecomnet, UserId);
		// Agrega cliente
		Clientes.Add(cliente);
		// Nuevo cliente con datos completos
		cliente = new Cliente(
			codigoPais: "+52",
			telefono: "1234567890",
			creationUser: UserId,
			testCase: TestCaseId);
		cliente.AgregarEmpresa(tecomnet, UserId);
		// Agrega datos personales
		cliente.AgregarDatosPersonales(nombre: "Cliente", primerApellido: "ApellidoPaterno",
			segundoApellido: "ApellidoMaterno", fechaNacimiento: DateOnly.Parse("1990-01-01"),
			genero: Genero.Masculino, modificationUser: UserId);
		// Agrega pre direccion
		cliente.AgregarDireccion(direccion: new Direccion(
			pais: "México",
			estado: "Campeche",
			creationUser: UserId,
			testCase: TestCaseId), creationUser: UserId);
		Clientes.Add(cliente);
		// Nuevo cliente con datos completos
		cliente = new Cliente(
			codigoPais: "+52",
			telefono: "9876543210",
			creationUser: UserId,
			testCase: TestCaseId);
		cliente.AgregarEmpresa(tecomnet, UserId);
		// Agrega datos personales
		cliente.AgregarDatosPersonales(nombre: "Cliente Tecomnet", primerApellido: "Primer Apellido",
			segundoApellido: "Segundo Apellido", fechaNacimiento: DateOnly.Parse("1990-01-01"),
			genero: Genero.Femenino, modificationUser: UserId);
		// Agrega dispositivo movil autorizado
		cliente.AgregarDispositivoMovilAutorizado(dispositivo: new DispositivoMovilAutorizado(
			token: "32414",
			idDispositivo: "32414",
			nombre: "nombre dispositivo",
			caracteristicas: "caracteristicas dispositivo",
			creationUser: UserId,
			testCase: TestCaseId), modificationUser: UserId);
		// Agrega cliente
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
		// Nuevo estado
		estado = new Estado(
			nombre: "EstadoInactivo",
			creationUser: UserId,
			testCase: TestCaseId);
		estado.Deactivate(UserId);
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
		// Nueva empresa
		empresa = new Empresa(
			nombre: "EmpresaInactiva",
			creationUser: UserId,
			testCase: TestCaseId);
		empresa.Deactivate(modificationUser: UserId);
		// Agrega empresa
		Empresas.Add(empresa);
	}

    private void CrearProveedoresServicios()
    {
        // Nuevo proveedor
        var proveedor = new ProveedorServicio(
            nombre: "CFE",
            categoria: ProductoCategoria.Servicios,
            urlIcono: "https://cfe.mx/logo.png",
            creationUser: UserId);
        // Agrega proveedor
        ProveedoresServicios.Add(proveedor);

        proveedor.AgregarProducto(
            sku: "SKU123",
            nombre: "Netflix Premium",
            monto: 15.99m,
            descripcion: "Premium subscription",
            creationUser: UserId);

        // Nuevo proveedor
        proveedor = new ProveedorServicio(
            nombre: "Telmex",
            categoria: ProductoCategoria.Recargas,
            urlIcono: "https://telmex.com/logo.png",
            creationUser: UserId);
        // Agrega proveedor
        ProveedoresServicios.Add(proveedor);
    }

    public void CrearServiciosFavoritos(Cliente primerCliente, ProveedorServicio primerProveedor)
    {
        var servicioFavorito = new ServicioFavorito(
            cliente: primerCliente,
            proveedorServicio: primerProveedor,
            alias: "Mi Luz",
            numeroReferencia: "123456789012",
            creationUser: UserId);
        ServiciosFavoritos.Add(servicioFavorito);
    }


	public readonly List<ProveedorServicio> ProveedoresServicios = [];
	public readonly List<ServicioFavorito> ServiciosFavoritos = [];
	public readonly List<ProductoProveedor> ProductosProveedores = [];
}