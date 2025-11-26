# WalletService

Este proyecto implementa una API RESTful para un servicio de billetera. Proporciona funcionalidades básicas para la gestión de usuarios y la monitorización del estado del servicio.

## Funcionalidades

La API expone las siguientes funcionalidades:

### Gestión de Usuarios
- `POST /{version}/users`: Crea un nuevo usuario.
- `GET /{version}/users`: Obtiene la información de un usuario.
- `PUT /{version}/users`: Actualiza la información de un usuario existente.
- `DELETE /{version}/users`: Elimina un usuario.

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

**Para el proyecto RestAPI:**

```bash
dotnet user-secrets set "dbConnectionString" "<TU_CADENA_DE_CONEXION>" --project Wallet.RestAPI
```

**Para el proyecto UnitTest:**

```bash
dotnet user-secrets set "dbConnectionString" "<TU_CADENA_DE_CONEXION>" --project Wallet.UnitTest
```

#### 2. Migraciones de Base de Datos

Para aplicar las migraciones y actualizar la base de datos, utiliza el siguiente comando:

```bash
dotnet ef database update --project Wallet.DOM --startup-project Wallet.RestAPI
```

Para crear una nueva migración después de realizar cambios en los modelos:

```bash
dotnet ef migrations add <NombreDeLaMigracion> --project Wallet.DOM --startup-project Wallet.RestAPI
```