using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;

namespace Wallet.UnitTest.IntegrationTest;

/// <summary>
/// Clase de prueba de integración para verificar el flujo completo de la API.
/// Simula la creación de usuarios, empresas, brokers, proveedores, productos y servicios favoritos.
/// </summary>
public class FullFlowApiTest : DatabaseTestFixture
{
    /// <summary>
    /// Interfaz para escribir la salida de las pruebas, útil para depuración.
    /// </summary>
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Versión de la API utilizada en las solicitudes.
    /// </summary>
    private const string ApiVersion = "0.1";

    /// <summary>
    /// Configuración de serialización JSON para convertir objetos a formato camelCase.
    /// </summary>
    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    /// <summary>
    /// Constructor de la clase de prueba.
    /// </summary>
    /// <param name="output">Proveedor de salida para las pruebas.</param>
    public FullFlowApiTest(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Prueba de integración que simula un flujo completo exitoso a través de la API.
    /// Incluye autenticación, creación de entidades relacionadas (empresas, brokers, proveedores, productos)
    /// y la creación de servicios favoritos.
    /// </summary>
    [Fact]
    public async Task Test_Full_Flow_Ok()
    {
        // 1. Autenticación y Configuración Inicial
        // Crea un usuario autenticado y obtiene su token de acceso.
        var (user, token) = await CreateAuthenticatedUserAsync();
        // Crea un cliente HTTP para interactuar con la API.
        var client = Factory.CreateClient();
        // Establece el encabezado de autorización con el token Bearer.
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // Siembra datos iniciales necesarios para el Cliente (Empresa) y crea un Cliente para el usuario.
        int clienteId = await SeedClientForUser(user: user);

        // 2. Registro de 3 Empresas
        // Itera para crear múltiples empresas.
        var empresaIds = new List<int>();
        for (int i = 1; i <= 3; i++)
        {
            var request = new EmpresaRequest { Nombre = $"Empresa Test {i}" };
            // Envía la solicitud POST para crear una empresa.
            var response = await client.PostAsync(requestUri: $"/{ApiVersion}/empresa",
                content: CreateContent(body: request));
            var content = await response.Content.ReadAsStringAsync();
            // Verifica que la respuesta sea Created (201) u OK (200).
            Assert.True(condition: response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK,
                userMessage: $"Expected Created or OK for Empresa but got {response.StatusCode}. Content: {content}");
            // Deserializa la respuesta para obtener el resultado de la empresa creada.
            var result = JsonConvert.DeserializeObject<EmpresaResult>(value: content, settings: _jsonSettings);
            Assert.NotNull(@object: result);
            // Almacena el ID de la empresa creada.
            if (result.Id.HasValue) empresaIds.Add(item: result.Id.Value);
            _output.WriteLine(message: $"Empresa creada: {result.Nombre} (ID: {result.Id})");
        }

        // 3. Registro de 2 Brokers
        // Itera para crear múltiples brokers.
        var brokerIds = new List<int>();
        for (int i = 1; i <= 2; i++)
        {
            var request = new BrokerRequest { Nombre = $"Broker Test {i}" };
            // Envía la solicitud POST para crear un broker.
            var response = await client.PostAsync(requestUri: $"/{ApiVersion}/broker",
                content: CreateContent(body: request));
            var content = await response.Content.ReadAsStringAsync();
            // Verifica que la respuesta sea Created (201) u OK (200).
            Assert.True(condition: response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK,
                userMessage: $"Expected Created or OK for Broker but got {response.StatusCode}. Content: {content}");
            // Deserializa la respuesta para obtener el resultado del broker creado.
            var result = JsonConvert.DeserializeObject<BrokerResult>(value: content, settings: _jsonSettings);
            Assert.NotNull(@object: result);
            // Almacena el ID del broker creado.
            brokerIds.Add(item: result.Id.Value);
            _output.WriteLine(message: $"Broker creado: {result.Nombre} (ID: {result.Id})");
        }

        // 4. Registro de 6 Proveedores (Distribuidos entre Brokers)
        // Itera para crear múltiples proveedores, asignándolos cíclicamente a los brokers existentes.
        var proveedorIds = new List<int>();
        for (int i = 1; i <= 6; i++)
        {
            // Asigna el proveedor a un broker de forma rotativa.
            var brokerId = brokerIds[index: (i - 1) % brokerIds.Count];
            var request = new ProveedorRequest
            {
                Nombre = $"Proveedor Test {i}",
                UrlIcono = $"https://example.com/icon_{i}.png",
                BrokerId = brokerId
            };
            // Envía la solicitud POST para crear un proveedor.
            var response = await client.PostAsync(requestUri: $"/{ApiVersion}/proveedor",
                content: CreateContent(body: request));
            var content = await response.Content.ReadAsStringAsync();
            // Verifica que la respuesta sea Created (201) u OK (200).
            Assert.True(condition: response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK,
                userMessage: $"Expected Created or OK for Proveedor but got {response.StatusCode}. Content: {content}");
            // Deserializa la respuesta para obtener el resultado del proveedor creado.
            var result = JsonConvert.DeserializeObject<ProveedorResult>(value: content, settings: _jsonSettings);
            Assert.NotNull(@object: result);
            // Almacena el ID del proveedor creado.
            if (result.Id.HasValue) proveedorIds.Add(item: result.Id.Value);
            _output.WriteLine(
                message: $"Proveedor creado: {result.Nombre} (ID: {result.Id}) vinculado al Broker {brokerId}");
        }

        // 5. Registro de 10 Productos (Distribuidos entre Proveedores)
        // Itera para crear múltiples productos, asignándolos cíclicamente a los proveedores existentes.
        var productoIds = new List<int>();
        for (int i = 1; i <= 10; i++)
        {
            // Asigna el producto a un proveedor de forma rotativa.
            var proveedorId = proveedorIds[index: (i - 1) % proveedorIds.Count];
            var request = new ProductoRequest
            {
                Sku = $"SKU-{i}",
                Nombre = $"Producto Test {i}",
                Precio = 100 + i,
                UrlIcono = $"icon_{i}.png",
                Categoria = CategoriaEnum.MovilidadEnum
            };
            // Envía la solicitud POST para crear un producto asociado a un proveedor.
            var response = await client.PostAsync(requestUri: $"/{ApiVersion}/proveedor/{proveedorId}/producto",
                content: CreateContent(body: request));
            var content = await response.Content.ReadAsStringAsync();
            // Verifica que la respuesta sea Created (201) u OK (200).
            Assert.True(condition: response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK,
                userMessage: $"Expected Created or OK for Producto but got {response.StatusCode}. Content: {content}");
            // Deserializa la respuesta para obtener el resultado del producto creado.
            var result = JsonConvert.DeserializeObject<ProductoResult>(value: content, settings: _jsonSettings);
            Assert.NotNull(@object: result);
            // Almacena el ID del producto creado.
            if (result.Id.HasValue) productoIds.Add(item: result.Id.Value);
            _output.WriteLine(
                message: $"Producto creado: {result.Nombre} (ID: {result.Id}) vinculado al Proveedor {proveedorId}");
        }

        // 6. Registro de 2 Servicios Favoritos
        // Itera para crear múltiples servicios favoritos para el cliente.
        for (int i = 1; i <= 2; i++)
        {
            // Utiliza los primeros proveedores para los servicios favoritos.
            var proveedorId = proveedorIds[index: i % proveedorIds.Count];
            var request = new ServicioFavoritoRequest
            {
                ClienteId = clienteId,
                ProveedorId = proveedorId,
                Alias = $"Mi Favorito {i}",
                NumeroReferencia = $"REF-{i}"
            };
            // Envía la solicitud POST para crear un servicio favorito.
            var response = await client.PostAsync(requestUri: $"/{ApiVersion}/servicioFavorito",
                content: CreateContent(body: request));
            var content = await response.Content.ReadAsStringAsync();
            // Verifica que la respuesta sea Created (201) u OK (200).
            Assert.True(condition: response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK,
                userMessage:
                $"Expected Created or OK for Servicio Favorito but got {response.StatusCode}. Content: {content}");
            // Deserializa la respuesta para obtener el resultado del servicio favorito creado.
            var result = JsonConvert.DeserializeObject<ServicioFavoritoResult>(value: content, settings: _jsonSettings);
            Assert.NotNull(@object: result);
            _output.WriteLine(message: $"Servicio Favorito creado: {result.Alias} (ID: {result.Id})");
        }
    }

    /// <summary>
    /// Siembra un cliente en la base de datos para un usuario dado.
    /// Incluye la creación de una empresa por defecto para el cliente.
    /// </summary>
    /// <param name="user">El usuario para el cual se creará el cliente.</param>
    /// <returns>El ID del cliente creado.</returns>
    private async Task<int> SeedClientForUser(Usuario user)
    {
        await using var context = CreateContext();

        // Siembra una Empresa por defecto para el cliente.
        var empresa = new Empresa(nombre: "Tecomnet Default", creationUser: Guid.NewGuid());
        await context.Empresa.AddAsync(entity: empresa);
        await context.SaveChangesAsync();

        // Asumiendo que el usuario ya está adjunto o que su ID coincide.
        // CreateAuthenticatedUserAsync crea el usuario en la DB.
        // Necesitamos buscarlo o adjuntarlo? No, solo usar el ID.
        var userDb = await context.Usuario.FindAsync(keyValues: user.Id);

        // Crea una nueva instancia de Cliente vinculada al usuario y la empresa.
        var cliente = new Cliente(usuario: userDb!, empresa: empresa, creationUser: Guid.NewGuid());
        // Agrega datos personales al cliente.
        cliente.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "FullFlow",
            fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Genero.Masculino,
            modificationUser: Guid.NewGuid());
        // Agrega una dirección al cliente.
        cliente.AgregarDireccion(direccion: new Direccion(pais: "MX", estado: "CDMX", creationUser: Guid.NewGuid()),
            creationUser: Guid.NewGuid());

        // Agrega el cliente al contexto y guarda los cambios en la base de datos.
        await context.Cliente.AddAsync(entity: cliente);
        await context.SaveChangesAsync();

        return cliente.Id;
    }

    /// <summary>
    /// Crea un objeto <see cref="StringContent"/> a partir de un objeto, serializándolo a JSON.
    /// </summary>
    /// <param name="body">El objeto a serializar y encapsular en el contenido HTTP.</param>
    /// <returns>Un <see cref="StringContent"/> listo para ser usado en solicitudes HTTP.</returns>
    private StringContent CreateContent(object body)
    {
        // Serializa el objeto a una cadena JSON utilizando la configuración predefinida.
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        // Crea un StringContent con el JSON, codificación UTF8 y tipo de contenido application/json.
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }
}
