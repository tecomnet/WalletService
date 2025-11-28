# RestAPI - ASP.NET Core 9.0 Server

This API will serve the functionality of validationg tax ID for different countries

## Registration Process

The user registration process consists of the following steps:

1.  **Pre-Registration**: The user provides their phone number and country code. The system sends a verification code (SMS/Email).
    - Endpoint: `POST /{version}/usuario/preregistro`
2.  **Verification**: The user enters the verification code.
    - Endpoint: `PUT /{version}/usuario/{idUsuario}/confirmaVerificacion`
    - Returns: `true` if successful.
3.  **Set Password**: After verification, the user sets their password. **This step generates the Access Token.**
    - Endpoint: `POST /{version}/usuario/{idUsuario}/contrasena`
    - Returns: `TokenResult` containing the JWT access token.

## Run

Linux/OS X:

```
sh build.sh
```

Windows:

```
build.bat
```

## Run in Docker

```
cd src/RestAPI
docker build -t restapi .
docker run -p 5000:5000 restapi
```
