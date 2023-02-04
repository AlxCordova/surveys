# Acme Surveys

API Rest para creación de encuestas. Desarrollado en .net 6 y SQL Server Express 15.0

## Installation

Utilizar Visual Studio 2022 o Visual Studio Code

Para VS Code ingresar a la ruta del proyecto, en donde se encuentra el archivo Survey.API.csproj y mediante una consola ejecutar el comando:

```bash
dotnet run
```
Esto ejecutara el proyecto en modo de desarrollo para poder testear el API. En caso de que de algún inconveniente con alguna dependencia, ejecutar el siguiente comando:

```bash
dotnet restore
```

Para verificar la listda de dependencias instaladas en el proyecto ejecutar el siguiente comando:

```bash
dotnet list package
```

Se incluirá el script utilizado para la base de datos, así como la colección de peticiones realizadas desde Postman.

## SQL
Se agrego el archivo Survey.sql, el cual contiene la estructura de base de datos utilizada para realizar las pruebas, así como también los procedimientos almacenados utilizados.
