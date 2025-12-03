# WalletService

Este proyecto implementa una API RESTful para un servicio de billetera. Proporciona funcionalidades básicas para la gestión de usuarios y la monitorización del estado del servicio.

## Funcionalidades

La API expone las siguientes funcionalidades:

### Proceso de Registro de Usuario

El registro de un nuevo usuario sigue un flujo secuencial para garantizar la seguridad y la integridad de los datos:

1.  **Pre-registro**:
    - **Endpoint**: `POST /{version}/registro/preRegistro`
    - **Descripción**: El usuario proporciona su número de teléfono y código de país. El sistema crea un registro inicial de `Usuario` (con estatus `PreRegistrado`) y un `Cliente` asociado, y envía un código de verificación (OTP) por SMS.
    - **Body**: `{ "codigoPais": "52", "telefono": "5512345678" }`

2.  **Confirmar Número**:
    - **Endpoint**: `POST /{version}/registro/confirmarNumero`
    - **Descripción**: El usuario ingresa el código recibido por SMS para verificar su número.
    - **Body**: `{ "idUsuario": 1, "codigo": "123456" }`

3.  **Datos Cliente**:
    - **Endpoint**: `PUT /{version}/registro/datosCliente`
    - **Descripción**: El usuario completa sus datos personales básicos.
    - **Body**: `{ "idUsuario": 1, "nombre": "Juan", "apellidoPaterno": "Perez", ... }`

4.  **Registrar Correo**:
    - **Endpoint**: `POST /{version}/registro/correo`
    - **Descripción**: El usuario registra su correo electrónico y se le envía un código de verificación.
    - **Body**: `{ "idUsuario": 1, "correo": "juan@example.com" }`

5.  **Verificar Correo**:
    - **Endpoint**: `POST /{version}/registro/verificarCorreo`
    - **Descripción**: El usuario verifica su correo electrónico con el código recibido.
    - **Body**: `{ "idUsuario": 1, "codigo": "654321" }`

6.  **Registrar Biométricos**:
    - **Endpoint**: `POST /{version}/registro/biometricos`
    - **Descripción**: El usuario registra sus datos biométricos (simulado con token de dispositivo).
    - **Body**: `{ "idUsuario": 1, "idDispositivo": "device_123", "token": "bio_token_abc" }`

7.  **Aceptar Términos**:
    - **Endpoint**: `POST /{version}/registro/terminos`
    - **Descripción**: El usuario acepta los términos y condiciones.
    - **Body**: `{ "idUsuario": 1, "version": "v1.0" }`

8.  **Completar Registro**:
    - **Endpoint**: `POST /{version}/registro/completar`
    - **Descripción**: El usuario establece su contraseña y finaliza el registro. El usuario queda en estatus `Activo`.
    - **Body**: `{ "idUsuario": 1, "contrasena": "Password123!", "confirmacionContrasena": "Password123!" }`

### Gestión de Cuenta

Una vez registrado, el usuario puede gestionar su cuenta:

- **Actualizar Email**: `PUT /{version}/usuario/{idUsuario}/actualizaEmail` (Requiere verificación posterior por email).
- **Actualizar Teléfono**: `PUT /{version}/usuario/{idUsuario}/actualizaTelefono` (Requiere verificación posterior por SMS).
- **Actualizar Contraseña**: `PUT /{version}/usuario/{idUsuario}/contrasena`.

### Health Check
- `GET /health`: Un endpoint para verificar que el servicio está en funcionamiento (liveness probe).
- `GET /ready`: Un endpoint para verificar que el servicio está listo para recibir peticiones (readiness probe).

## Estructura del Proyecto

La solución está organizada en las siguientes capas, siguiendo una arquitectura limpia:

- **Template.RestAPI:** El punto de entrada principal de la aplicación. Expone los endpoints de la API RESTful, gestiona las peticiones y respuestas HTTP, y contiene la configuración de Swagger (OpenAPI).

- **Template.Funcionalidad:** Contiene la lógica de negocio principal de la aplicación. Se encarga de orquestar las operaciones, validaciones y la interacción con la capa de datos.

- **Template.DOM:** Define los modelos de dominio y las entidades del negocio. Estas son las estructuras de datos principales que se utilizan en toda la aplicación.

- **Template.UnitTest:** Contiene tests unitarios y de integración para asegurar la calidad y el correcto funcionamiento del servicio.

## Configuración

### Base de Datos

El proyecto utiliza **User Secrets** para gestionar la cadena de conexión a la base de datos, tanto para el entorno de desarrollo (RestAPI) como para las pruebas unitarias (UnitTest).

#### 1. Configurar User Secrets

Ejecuta los siguientes comandos en la raíz del proyecto para establecer la cadena de conexión. Asegúrate de reemplazar `<TU_CADENA_DE_CONEXION>` con tu cadena de conexión real a SQL Server.

**Ejemplo de cadena de conexión:**
`"Server=localhost,1433;Database=WalletServiceDb;User Id=sa;Password=TuPasswordFuerte123!;TrustServerCertificate=True;"`

**Para el proyecto RestAPI:**

```bash
dotnet user-secrets set "dbConnectionString" "Server=localhost,1433;Database=WalletServiceDb;User Id=sa;Password=TuPasswordFuerte123!;TrustServerCertificate=True;" --project Wallet.RestAPI
```

**Para el proyecto UnitTest:**

```bash
dotnet user-secrets set "dbConnectionString" "Server=localhost,1433;Database=WalletServiceDb;User Id=sa;Password=TuPasswordFuerte123!;TrustServerCertificate=True;" --project Wallet.UnitTest
```

#### 2. Configurar JWT Secrets

El servicio utiliza JWT para la autenticación. Es necesario configurar la clave secreta, el emisor y la audiencia.

**Para el proyecto RestAPI:**

```bash
dotnet user-secrets set "Jwt:Key" "TuSuperSecretoKey123!" --project Wallet.RestAPI
dotnet user-secrets set "Jwt:Issuer" "WalletService" --project Wallet.RestAPI
dotnet user-secrets set "Jwt:Audience" "WalletServiceUsers" --project Wallet.RestAPI
```

**Para el proyecto UnitTest:**

```bash
dotnet user-secrets set "Jwt:Key" "TuSuperSecretoKey123!" --project Wallet.UnitTest
dotnet user-secrets set "Jwt:Issuer" "WalletService" --project Wallet.UnitTest
dotnet user-secrets set "Jwt:Audience" "WalletServiceUsers" --project Wallet.UnitTest
```

#### 3. Migraciones de Base de Datos

Para aplicar las migraciones y actualizar la base de datos, utiliza el siguiente comando:

```bash
dotnet ef database update --project Wallet.DOM --startup-project Wallet.RestAPI
```

Para crear una nueva migración después de realizar cambios en los modelos:

```bash
dotnet ef migrations add <NombreDeLaMigracion> --project Wallet.DOM --startup-project Wallet.RestAPI
```

### Docker

El proyecto incluye configuración para Docker y Docker Compose, facilitando el despliegue en entornos de desarrollo.

#### 1. Configurar Variables de Entorno

Crea un archivo `.env` en la raíz del proyecto basándote en el archivo de ejemplo `.env.example`.

```bash
cp .env.example .env
```

Asegúrate de configurar las variables según tus necesidades:

- `ASPNETCORE_ENVIRONMENT`: Entorno de ejecución (ej. Development).
- `MSSQL_SA_PASSWORD`: Contraseña para el usuario `sa` de SQL Server.
- `DB_PORT`: Puerto expuesto para la base de datos (por defecto 1433).
- `API_PORT_HTTP`: Puerto HTTP para la API (por defecto 8080).
- `API_PORT_HTTPS`: Puerto HTTPS para la API (por defecto 8081).
- `JWT_KEY`: Clave secreta para firmar los tokens JWT.
- `JWT_ISSUER`: Emisor del token JWT.
- `JWT_AUDIENCE`: Audiencia del token JWT.

#### 2. Ejecutar con Docker Compose

Para levantar la aplicación y la base de datos:

```bash
docker-compose up --build
```

La API estará disponible en `http://localhost:<API_PORT_HTTP>` y la base de datos en `localhost:<DB_PORT>`.