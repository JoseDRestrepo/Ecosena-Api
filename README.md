# Ejecucion de la API

Para correr el proyecto en local hay que ejecutar el siguiente comando:
```
	docker compose up --build
```
La api va a correr en el puerto http://localhost:8080
Esta ya estara corriendo en una base de datos de prueba en la nube

# Endpoints de la aplicacion

## /api/Auth/login POST
Este endpoint permite al usuario ingresar usando las credenciales documento y contraseña, genera un JWT y valida la contraseña usando BCrypt

## /api/Auth/register POST
Este endpoint permite al usuario registrarse, protege la contraseña usando BCrypt

## /api/Profile GET 
Este endpoint le da al usuario la informacion de su perfil

## /api/Profile PUT 
Este endpoint permite al usuario modificar datos de su perfil en la aplicacion, tales como su fecha de nacimiento, su foto de perfil o su correo

## /api/Blog GET 
Este endpoint le permite al usuario acceder a una lista de las entradas del Blog

## /api/Blog/$[id] GET
Este endpoint permite al usuario leer una entrada en especifico pasando el id de la entrada

## /api/Blog POST
Este endpoint permite a un usuario con rol administrador crear una entrada en el blog, en caso de no establecer una portada se deja la portada por defecto

## /api/Blog/$[id] PUT
Este endpoint permite a un usuario con rol administrador editar una entrada en el blog pasando el id de la entrada que quiere editar

## /api/Blog/$[id] DELETE
Este endpoint permite a un usuario con rol administrador eliminar una entrada del blog pasando el id de la entrada que quiere eliminar

## /api/Report/AllReports GET 
Este endpoint permite a un usuario con rol administrador ver una lista de todos los reportes

## /api/Report/Reports GET
Este endpoint permite a un usuario con rol aprendiz ver una lista de sus reportes hechos

## /api/Report POST
Este endpoint permite a un aprendiz realizar un reporte

## /api/Report/$[id] PUT 
Este enpoint permite a un administrador pasar el reporte al siguiente estado (Pendiente -> En progreso -> Resuelto) pasando el id del reporte que busca actualizar

## /ReportsExcel GET 
Este endpoint permite a un administrador tener datos generales de los reportes del mes mas una lista de estos en formato Excel con graficos comparando con los datos en general de la aplicacion

# Estructura de los directorios

- /Data/ : Contiene el contexto de las bases de datos
- /Entities/ : Contiene las clases que representan las tablas de la base de datos
- /Services/ : Contiene todos los servicios que usa la aplicacion para interactuar con los datos que se generan en la aplicacion y guardarlos en la base de datos
- /Interfaces/ : Contiene todas las interfaces que luego se usan para inyeccion de dependencias de los servicios
- /Models/ : Contiene los DTOs de la aplicacion
- /Resources/ : Contiene los recursos de la aplicacion
- /Controllers/ : Hacen uso de los servicios y exponen sus funcionalidades en los endpoints de la API, tambien aca se realizan las validaciones y se entregan los mensajes de respuesta por cada request a la api
