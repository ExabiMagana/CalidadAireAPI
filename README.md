# API de Calidad del Aire

## Base de datos

El archivo CalidadAire.sql contiene el script necesario para crear la base de datos, sus tablas, relaciones y algunos sensores iniciales.

## Endpoints de la API

POST    /api/auth/login                 Permite iniciar sesión y obtener un token JWT.
POST    /api/lecturas                   Registra una nueva lectura y evalúa si debe generar una alerta.
GET     /api/lecturas/{id}              Consulta una lectura por su identificador.
GET     /api/lecturas/filtrar           Consulta lecturas aplicando filtros por fecha y contaminante. 
GET     /api/lecturas/{id}/enriquecida  Consulta una lectura junto con información del clima.
GET     /api/alertas                    Consulta las alertas registradas.
GET     /api/alertas/{id}               Consulta una alerta por su identificador.

las alertas se generan automáticamente al registrar una lectura.

## Autenticación
La API utiliza JWT para controlar el acceso a los endpoints protegidos.

Para utilizar la API:
1 Ejecutar el endpoint POST `/api/auth/login`
2. Ingresar el nombre de usuario y la contraseña (El usuario ejemplo es `admin` `Admin123*`)
3. Copiar el token recibido en la respuesta.
4. Usar el token para realizar las solicitudes protegidas.

## Instalación y ejecución

### 1. Clonar el repositorio

`git clone https://github.com/ExabiMagana/CalidadAireAPI.git`

Ingresar a la carpeta del proyecto:

`cd CalidadAire`

### 2. Crear la base de datos

Abrir un cliente de MySQL.

Ejecutar el contenido del archivo `CalidadAire.sql` para crear la base de datos y sus respectivas tablas.

### 3. Configurar la conexión

En el archivo `appsettings.json` establecer la conexión a MySQL y la clave secreta para JWT.

Ejemplo:

`
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=calidad_aire;User=root;Password=TU_CONTRASEÑA;"
  },
  "Jwt": {
    "Key": "SE_DEBE_CONFIGURAR_LOCALMENTE",
    "Issuer": "CalidadAireAPI",
    "Audience": "CalidadAireAPI",
    "ExpirationMinutes": 60
  },
}
`

### 4. Restaurar los paquetes

Desde la terminal, ejecutar:

`dotnet restore`

### 5. Ejecutar el proyecto

`dotnet run`
