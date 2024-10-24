# SegurosMatic
## Carpetas
- SegurosMatic/ contiene la API web en dotnet
- SegurosMatic.ClienteBlazor/ contiene el cliente en Blazor Web Assembly
- SegurosMatic.Modelos/ es una class library que contiene los modelos y
las interfaces para operar entre el cliente y la API, pueden servir de
intermediario para reutilizar la misma validación entre el cliente y el servidor

## Cambios para probar
En la carpeta del cliente se encuentra la carpeta `wwwroot` que contiene un
archivo `appsettings.json`, este tiene un par clave/valor que apunta al API web.
Se debe cambiar de ser necesario si el API web no tiene una configuración
explícita para usar un puerto específico, etc.

### Conexión a base de datos
Cambiar la cadena de conexión según corresponda en el archivo `appsettings.json`
del API web.
Ej:
```
"ConnectionStrings": {
  "DefaultConnection": "Server=LAPTOP-DJ19MSGT;Database=NombreDB;User Id=NombreUsuario;Password=Contraseña;TrustServerCertificate=True;"
  }
```

## Creación de tablas y stored procedures
Se proveerá de un archivo `*.sql` para hacer el poblado de la base de datos, tanto
con las tablas como con los stored procedures necesarios.
De no ser este el caso, debería de haber un archivo de respaldo de la base de datos
para que sea cargado en el motor de SQL Server que corresponda al entorno que
deseas usar.