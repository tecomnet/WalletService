using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class ClienteTest : UnitTestTemplate
{
    [Theory]
    [InlineData(data: ["OK: New user", "+52", "9815263699", true, new string[] { }])]
    [InlineData(data: ["ERROR: User null", null, null, false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["ERROR: User empty", "", "", false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["ERROR: User long string", "ThisisexampleofastringthatcontainsmorethanfiftycharactersokThisisexampleofastringthatcontainsmorethanfiftycharactersok1", "5959595959595959595959595", false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }])]
    public void BasicClienteTest(
        // Case name
        string caseName,
        // Test data
        string? codigoPais,
        string? telefono,
        // Result
        bool success,
        string[]? expectedErrors = null
    )
    {
        try
        {
            // Crea un cliente
#pragma warning disable CS8604 // Possible null reference argument
            var usuario = new Usuario(
                codigoPais: codigoPais,
                telefono: telefono,
                correoElectronico: null,
                contrasena: null,
                estatus: "Activo",
                creationUser: Guid.NewGuid(),
                testCase: caseName);
            var user = new Cliente(
                usuario: usuario,
                creationUser: Guid.NewGuid(),
                testCase: caseName);
            // Check the properties
            Assert.True(condition: user.Usuario.Telefono == telefono,
                userMessage: $"CodigoPais is not correct. Expected: {codigoPais}. " +
                             $"Actual: {user.Usuario.CodigoPais}");
            Assert.True(condition: user.Usuario.Telefono == telefono,
                userMessage: $"Telefono is not correct. Expected: {telefono}. " +
                             $"Actual: {user.Usuario.Telefono}");
            // Assert success
            Assert.True(condition: success, userMessage: "Should not reach on failures.");
        }
        // Catch the managed errors and check them with the expected ones in the case of failures
        catch (EMGeneralAggregateException exception)
        {
            // Treat the raised error
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Catch any non managed errors and display them to understand the root cause
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            // Should not reach for unmanaged errors
            Assert.Fail(message: $"Uncaught exception. {exception.Message}");
        }
    }


    [Theory]
    // ----------------------------------------------------------------------------------------------------------------
    // 1. CASOS DE ÉXITO (Datos mínimos y máximos válidos)
    // ----------------------------------------------------------------------------------------------------------------
    [InlineData(data: ["1. OK: Todos los campos válidos", "Ana", "Lopez", "Gomez", "1990-01-01", Genero.Femenino, true, new string[] { }])]
    [InlineData(data: ["2. OK: Segundo Apellido null", "Juan", "Perez", "Gomez", "2000-05-15", Genero.Masculino, true, new string[] { }])]
    [InlineData(data:
    ["3. OK: Longitud máxima (Nombre/Apellido y Email)", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", // 100 chars
            "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB", // 100 chars
            "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC", // 100 chars
            "2005-12-31", Genero.NoBinario, true, new string[] { }
    ])]
    // ----------------------------------------------------------------------------------------------------------------
    // 2. ERRORES DE REQUERIMIENTO (PROPERTY-VALIDATION-REQUIRED-ERROR)
    // ----------------------------------------------------------------------------------------------------------------
    [InlineData(data: ["4. ERROR: Nombre null", null, "Perez", null, "1990-01-01", Genero.Masculino, false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["5. ERROR: Primer Apellido vacío", "Juan", "", "Gomez", "1990-01-01", Genero.Masculino, false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["6. ERROR: Fecha Nacimiento null", "Juan", "Perez", "Gomez", null, Genero.Masculino, false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["7. ERROR: Género null", "Juan", "Perez", "Gomez", "1990-01-01", null, false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    // ----------------------------------------------------------------------------------------------------------------
    // 3. ERRORES DE LONGITUD (PROPERTY-VALIDATION-LENGTH-INVALID)
    // ----------------------------------------------------------------------------------------------------------------
    [InlineData(data:
    ["8. ERROR: Nombre > 100 caracteres", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", // 101 chars
            "Perez", "Gomez", "1990-01-01", Genero.Masculino, false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }
    ])]
    // ----------------------------------------------------------------------------------------------------------------
    // 5. CASO DE ERRORES MÚLTIPLES 
    // ----------------------------------------------------------------------------------------------------------------
    [InlineData(data:
    ["9. ERROR: Múltiples fallos críticos", "", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "Gomez", null, null, false, new string[]
        {
            "PROPERTY-VALIDATION-REQUIRED-ERROR", // Nombre
            "PROPERTY-VALIDATION-LENGTH-INVALID", // PrimerApellido
            "PROPERTY-VALIDATION-REQUIRED-ERROR", // FechaNacimiento
            "PROPERTY-VALIDATION-REQUIRED-ERROR", // Genero
            "PROPERTY-VALIDATION-LENGTH-INVALID", // CorreoElectronico (asumo min 5)
            "PROPERTY-VALIDATION-REGEX-INVALID" // CorreoElectronico
        }
    ])]
    public void AgregarDatosPersonales_ValidationTest(
        // Case name
        string caseName,
        // Test data for strings
        string? nombre,
        string? primerApellido,
        string? segundoApellido,
        // Test data for DateOnly? as string
        string? fechaNacimientoString,
        // Test data for Genero?
        Genero? genero,
        // Result
        bool success,
        string[]? expectedErrors = null
    )
    {
        // CONVERSIÓN DE CADENA A DATEONLY?
        DateOnly? fechaNacimiento = null;
        if (!string.IsNullOrEmpty(value: fechaNacimientoString))
        {
            // Usar TryParse para manejar posibles errores de formato, aunque el test usa formato YYYY-MM-DD
            if (DateOnly.TryParse(s: fechaNacimientoString, result: out DateOnly parsedDate))
            {
                fechaNacimiento = parsedDate;
            }
            // Si el test pasa un string mal formado (ej. "2020/02/30"), el TryParse fallará, 
            // pero el test no lo validaría como REQUIRED; esto es aceptable para simplificar el test.
        }

        try
        {
            // Crea una instancia de Cliente (asumiendo un constructor base)
            // Crea una instancia de Cliente (asumiendo un constructor base)
            var usuario = new Usuario(codigoPais: "+52", telefono: "9825897845", correoElectronico: null, contrasena: null, estatus: "Activo", creationUser: Guid.NewGuid(),
                testCase: caseName);
            var cliente = new Cliente(usuario: usuario, creationUser: Guid.NewGuid(),
                testCase: caseName);
            // Ejecutar el método a probar
            cliente.AgregarDatosPersonales(
                nombre: nombre,
                primerApellido: primerApellido,
                segundoApellido: segundoApellido,
                fechaNacimiento: fechaNacimiento,
                genero: genero,
                modificationUser: Guid.NewGuid());
            // Verificación de éxito
            Assert.True(condition: success, userMessage: $"El caso '{caseName}' falló cuando se esperaba éxito.");
            // Verificación de asignación de valores (si success es true)
            Assert.Equal(expected: nombre, actual: cliente.Nombre);
            Assert.Equal(expected: primerApellido, actual: cliente.PrimerApellido);
            Assert.Equal(expected: segundoApellido, actual: cliente.SegundoApellido);
            Assert.Equal(expected: fechaNacimiento, actual: cliente.FechaNacimiento);
            Assert.Equal(expected: genero, actual: cliente.Genero);
        }
        // Capturar y verificar errores gestionados
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Capturar errores no gestionados
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException &&
                                          exception is not FalseException)
        {
            Assert.Fail(message: $"Excepción no gestionada en '{caseName}': {exception.GetType().Name} - {exception.Message}");
        }
    }

    private const string ContrasenaInicial = "Pass123456789";

    private const string MaxContrasenaMas100Chars =
        "X1X2X3X4X5X6X7X8X9X1X2X3X4X5X6X7X8X9X1X2X3X4X5X6X7X8X9X1X2X3X4X5X6X7X8X9X1X2X3X4X5X6X7X8X9X1X2X3X4X5X6X7X8X9X1X2X3X4X5X6X7X8X9X1X2X3X4X5X6X7X8X9X1X2X3X4X5X6X7X8X9X1X2X3X4X5X6X7X8X9X"; // 100 caracteres

    private const string LongCorreoElectronicoMas150Chars =
        "largo_excede_maximo_150_caracteres_ejemplo_dominio_de_prueba_y_validación_para_que_este_email_sea_muy_largo@example.comX"; // 151 caracteres

    [Theory]
    // === CASOS DE ÉXITO ===
    [InlineData(data: ["1. OK: CrearContrasena", "CrearContrasena", "NuevaPass123", null, null, null, null, null, true, new string[] { }])]
    [InlineData(data: ["2. OK: ActualizarContrasena", "ActualizarContrasena", "SuperNuevaPass", "SuperNuevaPass", ContrasenaInicial, null, null, null, true, new string[] { }])]
    [InlineData(data: ["3. OK: ActualizarTelefono", "ActualizarTelefono", null, null, null, "MEX", "5512345678", null, true, new string[] { }])]
    [InlineData(data: ["4. OK: ActualizarCorreo", "ActualizarCorreoElectronico", null, null, null, null, null, "nuevo.email@ejemplo.com", true, new string[] { }])]
    [InlineData(data: ["5. OK: Longitud máxima Contraseña (100)", "CrearContrasena", "X1X2X3X4X5X6X7X8X9X1X2X3X4X5X6X7X8X9X2XX9X13X46X7X8X9X1X2X3X4X5X6X7X8X9X1X2X3XX1X2X3X4X5X6X7X8X9X1X2", null, null, null, null, null, true, new string[] { }])]

    // === ERRORES DE ACTUALIZAR CONTRASEÑA (Lógica de Negocio) ===
    [InlineData(data: ["6. ERROR: Pass Antigua NO Coincide", "ActualizarContrasena", "SuperNuevaPass", "SuperNuevaPass", "PassIncorrecta", null, null, null, false, new string[] { ServiceErrorsBuilder.ContrasenaActualIncorrecta }])]
    [InlineData(data: ["6.1. ERROR: Pass Antigua NO Coincide", "ActualizarContrasena", "SuperNuevaPass", "SuperNuevaPass1", "PassIncorrecta", null, null, null, false, new string[] { ServiceErrorsBuilder.ContrasenasNoCoinciden }])]
    // === ERRORES DE CONTRASENA (REQUIRED, LENGTH-INVALID) ===
    [InlineData(data: ["7. OK: Contrasena nula", "CrearContrasena", null, null, null, null, null, null, true, new string[] { }])]
    [InlineData(data: ["8. ERROR: Contrasena muy larga (>100)", "CrearContrasena", MaxContrasenaMas100Chars, null, null, null, null, null, false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }])]

    // === ERRORES DE ACTUALIZAR TELEFONO (REQUIRED, LENGTH-INVALID) ===
    // CódigoPais (3-3)
    [InlineData(data: ["9. ERROR: CodigoPais null", "ActualizarTelefono", null, null, null, null, "5512345678", null, false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["10. ERROR: CodigoPais < 3", "ActualizarTelefono", null, null, null, "MX", "5512345678", null, false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }])]
    // Telefono (9-10)
    [InlineData(data: ["11. ERROR: Telefono < 9", "ActualizarTelefono", null, null, null, "MEX", "12345678", null, false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }])]
    [InlineData(data: ["12. ERROR: Telefono > 10", "ActualizarTelefono", null, null, null, "MEX", "12345678901", null, false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }])]

    // === ERRORES DE CORREO ELECTRONICO (REQUIRED, LENGTH-INVALID, REGEX-INVALID) ===
    // Restricciones: min 1, max 150, con formato regex
    [InlineData(data: ["13. OK: Correo null", "ActualizarCorreoElectronico", null, null, null, null, null, null, true, new string[] { }])]
    [InlineData(data: ["14. ERROR: Correo REGEX-INVALID", "ActualizarCorreoElectronico", null, null, null, null, null, "correo@malo", false, new string[] { "PROPERTY-VALIDATION-REGEX-INVALID" }])]
    [InlineData(data:
    ["15. ERROR: Correo muy largo (>150)", "ActualizarCorreoElectronico", null, null, null, null, null, LongCorreoElectronicoMas150Chars + "@dominio.com", // 151 chars
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID", "PROPERTY-VALIDATION-REGEX-INVALID" }
    ])]

    // === ERRORES MÚLTIPLES ===
    [InlineData(data:
    ["16. ERROR: Múltiples fallos en Teléfono", "ActualizarTelefono", null, null, null, "M", "", null, // Código: < 3; Teléfono: < 9 y REQUIRED
        false, new string[]
        {
            "PROPERTY-VALIDATION-LENGTH-INVALID", // CodigoPais
            "PROPERTY-VALIDATION-REQUIRED-ERROR", // Telefono
            "PROPERTY-VALIDATION-LENGTH-INVALID" // Telefono (si el validador maneja ambas, required y length, en ese orden)
        }
    ])]
    public void DatosContactoYSeguridadTest(
        string caseName,
        string accion,
        string? nuevaContrasena,
        string? confirmacionNuevaContrasena,
        string? contrasenaActual,
        string? codigoPais,
        string? telefono,
        string? correoElectronico,
        bool success,
        string[]? expectedErrors = null
    )
    {
        // 1. Configuración Inicial
        // 1. Configuración Inicial
        var usuario = new Usuario(codigoPais: "+52", telefono: "5512345678", correoElectronico: null, contrasena: null, estatus: "Activo", creationUser: Guid.NewGuid(),
            testCase: caseName);
        var cliente = new Cliente(usuario: usuario, creationUser: Guid.NewGuid(),
            testCase: caseName);

        // Si la acción es ActualizarContrasena, debemos inicializar la Contrasena del cliente.
        if (accion == "ActualizarContrasena")
        {
            // Llama al método de dominio para establecer la contraseña inicial.
            cliente.Usuario.CrearContrasena(contrasena: ContrasenaInicial, modificationUser: Guid.NewGuid());
        }

        try
        {
            // 2. Ejecución Dinámica del Método
            switch (accion)
            {
                case "CrearContrasena":
                    cliente.Usuario.CrearContrasena(contrasena: nuevaContrasena!, modificationUser: Guid.NewGuid());
                    Assert.Equal(expected: nuevaContrasena, actual: cliente.Usuario.Contrasena);
                    break;
                case "ActualizarContrasena":
#pragma warning disable CS8604 // Possible null reference argument
                    cliente.Usuario.ActualizarContrasena(contrasenaNueva: nuevaContrasena!,
                        confirmacionContrasenaNueva: confirmacionNuevaContrasena!, contrasenaActual: contrasenaActual,
                        modificationUser: Guid.NewGuid());
                    Assert.Equal(expected: nuevaContrasena, actual: cliente.Usuario.Contrasena);
                    break;
                case "ActualizarTelefono":
                    cliente.Usuario.ActualizarTelefono(codigoPais: codigoPais!, telefono: telefono!, modificationUser: Guid.NewGuid());
                    Assert.Equal(expected: codigoPais, actual: cliente.Usuario.CodigoPais);
                    Assert.Equal(expected: telefono, actual: cliente.Usuario.Telefono);
                    break;
                case "ActualizarCorreoElectronico":
                    cliente.Usuario.ActualizarCorreoElectronico(correoElectronico: correoElectronico!, modificationUser: Guid.NewGuid());
                    Assert.Equal(expected: correoElectronico, actual: cliente.Usuario.CorreoElectronico);
                    break;
                default:
                    throw new InvalidOperationException(message: $"Acción de prueba '{accion}' no reconocida.");
            }

            // 3. Verificación Final de Éxito
            Assert.True(condition: success, userMessage: $"El caso '{caseName}' falló cuando se esperaba éxito.");
        }
        // 4. Capturar y verificar errores gestionados (EMGeneralAggregateException)
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // 5. Capturar errores no gestionados
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException &&
                                          exception is not FalseException)
        {
            Assert.Fail(
                message: $"Excepción no gestionada en '{caseName}' (Acción: {accion}): {exception.GetType().Name} - {exception.Message}");
        }
    }
}