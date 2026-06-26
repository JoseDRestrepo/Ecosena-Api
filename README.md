# ECOSENA API

## Como correrlo en local (docker-compose)

Para correr el proyecto en local hay que ejecutar el siguiente comando:
```
	docker compose up --build
```
La api va a correr en el puerto http://localhost:8080

## Endpoints de la aplicacion

- /api/Auth/login POST: Este endpoint permite al usuario ingresar usando las credenciales documento y contraseña
- /api/Auth/register POST: Este endpoint permite al usuario registrarse
- /api/Profile GET: Este endpoint le da al usuario la informacion de su perfil
- /api/Profile PUT: Este endpoint permite al usuario modificar datos de su perfil en la aplicacion
- /api/Blog GET: Este endpoint le permite al usuario acceder a una lista de las entradas del Blog
- /api/Blog/$[id] GET: Este endpoint permite al usuario leer una entrada
- /api/Blog POST: Este endpoint permite a un usuario con rol administrador crear una entrada en el blog
- /api/Blog/$[id] PUT: Este endpoint permite a un usuario con rol administrador editar una entrada en el blog
- /api/Blog/$[id] DELETE: Este endpoint permite a un usuario con rol administrador eliminar una entrada del blog
- /api/Report/AllReports GET: Este endpoint permite a un usuario con rol administrador ver una lista de todos los reportes
- /api/Report/Reports GET: Este endpoint permite a un usuario con rol aprendiz ver una lista de sus reportes hechos
- /api/Report POST: Este endpoint permitea a un aprendiz realizar un reporte
- /api/Report/$[id] PUT: Este enpoint permite a un administrador pasar el reporte al siguiente estado (Pendiente -> En progreso -> Resuelto)
- /api/Report/ReportsExcel GET: Este endpoint permite a un administrador tener datos generales de los reportes del mes mas una lista de estos en formato Excel

## Estructura de los directorios

- /Data/ : Contiene el contexto de las bases de datos
- /Entities/ : Contiene las clases que representan las tablas de la base de datos
- /Services/ : Contiene todos los servicios que usa la aplicacion
- /Interfaces/ : Contiene todas las interfaces que luego se usan para inyeccion de dependencias de los servicios
- /Models/ : Contiene los DTOs de la aplicacion
- /Resources/ : Contiene los recursos de la aplicacion
