using Wallet.DOM.Modelos.GestionWallet;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionUsuario;

namespace Wallet.UnitTest;

public class CommonSettings
{
	// Test line for Gitnuro application
	private const string TestCaseId = "FunctionalTest";
	private static readonly Guid UserId = Guid.NewGuid();
	public readonly List<Cliente> Clientes = [];
	public readonly List<Estado> Estados = [];
	public readonly List<Empresa> Empresas = [];
	public readonly List<Usuario> Usuarios = [];
	public readonly List<Broker> Brokers = [];
	public readonly List<Proveedor> Proveedores = [];
	public readonly List<ServicioFavorito> ServiciosFavoritos = [];
	public readonly List<Producto> Productos = [];
	public readonly List<CuentaWallet> Cuentas = [];
	public readonly List<TarjetaEmitida> TarjetasEmitidas = [];
	public readonly List<TarjetaVinculada> TarjetasVinculadas = [];

	public CommonSettings()
	{
		// Create user data
		CrearEmpresas();
		CrearEstados();
		CrearClientes();
		CrearBrokers();
		CrearProveedores();
	}

	public void CrearCuentas(Cliente cliente)
	{
		var cuenta = new CuentaWallet(
			idCliente: cliente.Id,
			moneda: "MXN",
			cuentaCLABE: "123456789012345678",
			creationUser: UserId
		);
		Cuentas.Add(cuenta);
	}

	public void CrearTarjetasEmitidas(CuentaWallet cuenta)
	{
		var tarjetaVirtual = new TarjetaEmitida(
			idCuentaWallet: cuenta.Id,
			tokenProcesador: Guid.NewGuid().ToString("N"),
			panEnmascarado: "400000******1234",
			tipo: TipoTarjeta.Virtual,
			fechaExpiracion: DateTime.UtcNow.AddYears(3),
			creationUser: UserId
		);
		tarjetaVirtual.ActivarTarjeta(UserId);

		var tarjetaFisica = new TarjetaEmitida(
			idCuentaWallet: cuenta.Id,
			tokenProcesador: Guid.NewGuid().ToString("N"),
			panEnmascarado: "400000******5678",
			tipo: TipoTarjeta.Fisica,
			fechaExpiracion: DateTime.UtcNow.AddYears(5),
			creationUser: UserId,
			nombreImpreso: "Cliente Test"
		);
		// Fisica starts inactive usually

		TarjetasEmitidas.Add(tarjetaVirtual);
		TarjetasEmitidas.Add(tarjetaFisica);
	}

	public void CrearTarjetasVinculadas(CuentaWallet cuenta)
	{
		var tarjeta = new TarjetaVinculada(
			idCuentaWallet: cuenta.Id,
			numeroTarjeta: "4111111111111111",
			alias: "Mi Visa",
			marca: MarcaTarjeta.Visa,
			fechaExpiracion: DateTime.UtcNow.AddYears(2),
			creationUser: UserId
		);
		TarjetasVinculadas.Add(tarjeta);
	}


	private void CrearClientes()
	{
		var tecomnet = Empresas.First(predicate: e => e.Nombre == "Tecomnet");
		// Cliente pre registro
		var usuario = new Usuario(
			codigoPais: "+52",
			telefono: "9812078573",
			correoElectronico: null,
			contrasena: null,
			estatus: EstatusRegistroEnum.RegistroCompletado,
			creationUser: UserId,
			testCase: TestCaseId);
		Usuarios.Add(item: usuario);

		var cliente = new Cliente(
			usuario: usuario,
			empresa: tecomnet,
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega cliente
		Clientes.Add(item: cliente);
		// Nuevo cliente con datos completos
		usuario = new Usuario(
			codigoPais: "+52",
			telefono: "1234567890",
			correoElectronico: null,
			contrasena: null,
			estatus: EstatusRegistroEnum.RegistroCompletado,
			creationUser: UserId,
			testCase: TestCaseId);
		Usuarios.Add(item: usuario);

		cliente = new Cliente(
			usuario: usuario,
			empresa: tecomnet,
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega datos personales
		cliente.AgregarDatosPersonales(nombre: "Cliente", primerApellido: "ApellidoPaterno",
			segundoApellido: "ApellidoMaterno", fechaNacimiento: DateOnly.Parse(s: "1990-01-01"),
			genero: Genero.Masculino, modificationUser: UserId);
		// Agrega pre direccion
		cliente.AgregarDireccion(direccion: new Direccion(
			pais: "México",
			estado: "Campeche",
			creationUser: UserId,
			testCase: TestCaseId), creationUser: UserId);
		Clientes.Add(item: cliente);
		// Nuevo cliente con datos completos
		usuario = new Usuario(
			codigoPais: "+52",
			telefono: "9876543210",
			correoElectronico: null,
			contrasena: null,
			estatus: EstatusRegistroEnum.RegistroCompletado,
			creationUser: UserId,
			testCase: TestCaseId);
		// Removed invalid AddBusiness call
		Usuarios.Add(item: usuario);

		cliente = new Cliente(
			usuario: usuario,
			empresa: tecomnet,
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega datos personales
		cliente.AgregarDatosPersonales(nombre: "Cliente Tecomnet", primerApellido: "Primer Apellido",
			segundoApellido: "Segundo Apellido", fechaNacimiento: DateOnly.Parse(s: "1990-01-01"),
			genero: Genero.Femenino, modificationUser: UserId);
		// Agrega dispositivo movil autorizado
		cliente.Usuario.AgregarDispositivoMovilAutorizado(dispositivo: new DispositivoMovilAutorizado(
			token: "32414",
			idDispositivo: "32414",
			nombre: "nombre dispositivo",
			caracteristicas: "caracteristicas dispositivo",
			creationUser: UserId,
			testCase: TestCaseId), modificationUser: UserId);
		// Agrega cliente
		Clientes.Add(item: cliente);
	}

	private void CrearEstados()
	{
		// Nuevo estado
		var estado = new Estado(
			nombre: "Aguascalientes",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega estado
		Estados.Add(item: estado);
		// Nuevo estado
		estado = new Estado(
			nombre: "Baja California",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega estado
		Estados.Add(item: estado);
		// Nuevo estado
		estado = new Estado(
			nombre: "Baja California Sur",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega estado
		Estados.Add(item: estado);
		// Nuevo estado
		estado = new Estado(
			nombre: "Campeche",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega estado
		Estados.Add(item: estado);
		// Nuevo estado
		estado = new Estado(
			nombre: "EstadoInactivo",
			creationUser: UserId,
			testCase: TestCaseId);
		estado.Deactivate(modificationUser: UserId);
		// Agrega estado
		Estados.Add(item: estado);
	}

	private void CrearEmpresas()
	{
		// Nueva empresa
		var empresa = new Empresa(
			nombre: "Tecomnet",
			creationUser: UserId,
			testCase: TestCaseId);
		// Agrega empresa
		Empresas.Add(item: empresa);
		// Nueva empresa
		empresa = new Empresa(
			nombre: "EmpresaInactiva",
			creationUser: UserId,
			testCase: TestCaseId);
		empresa.Deactivate(modificationUser: UserId);
		// Agrega empresa
		Empresas.Add(item: empresa);
	}

	private void CrearBrokers()
	{
		var broker = new Broker(
			nombre: "Broker Principal",
			creationUser: UserId);
		Brokers.Add(item: broker);
	}

	private void CrearProveedores()
	{
		var broker = Brokers.First();

		// Nuevo proveedor
		var proveedor = new Proveedor(
			nombre: "CFE",
			urlIcono: "https://cfe.mx/logo.png",
			categoria: Categoria.Servicios,
			broker: broker,
			creationUser: UserId);
		// Agrega proveedor
		Proveedores.Add(item: proveedor);

		var producto = proveedor.AgregarProducto(
			sku: "SKU123",
			nombre: "Netflix Premium",
			precio: 15.99m,
			icono: "https://cfe.mx/logo.png",
			categoria: "Servicios",
			creationUser: UserId);
		Productos.Add(item: producto);

		// Producto tipo Recargas
		producto = proveedor.AgregarProducto(
			sku: "TELCEL100",
			nombre: "Recarga Telcel $100",
			precio: 100m,
			icono: "https://telcel.com/logo.png",
			categoria: nameof(Categoria.Recargas),
			creationUser: UserId);
		Productos.Add(item: producto);

		// Nuevo proveedor
		proveedor = new Proveedor(
			nombre: "Telmex",
			urlIcono: "https://telmex.com/logo.png",
			categoria: Categoria.Servicios,
			broker: broker,
			creationUser: UserId);
		// Agrega proveedor
		Proveedores.Add(item: proveedor);
	}

	public void CrearServiciosFavoritos(Cliente primerCliente, Proveedor primerProveedor)
	{
		var servicioFavorito = new ServicioFavorito(
			cliente: primerCliente,
			proveedor: primerProveedor,
			alias: "Mi Luz",
			numeroReferencia: "123456789012",
			creationUser: UserId);
		ServiciosFavoritos.Add(item: servicioFavorito);
	}
}