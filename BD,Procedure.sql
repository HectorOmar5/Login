create database DB_Access

use DB_Access

create table Usuarios(
idUsuario int primary key identity(1,1),
Correo varchar(100),
Contrasena varchar(100)
)

/*---------------------------------------------------------------------------------*/
create or alter procedure sp_RegistrarUsuario(
@correo varchar(100),  /*Parametro de entrada -- Insertar en la tabla usuarios*/
@Contrasena varchar(100),
@Registrado bit output,  /*Parametro de salida -- Obtener resultado -- tipo bit, solo maneja 1 y 0*/
@Mensaje varchar(100) output /*Texto de salida */
)
as
begin 
	if(not exists(select * from Usuarios where Correo = @Correo)) /* Validar usuario que no se repita con el mismo correo*/
	begin
		insert into Usuarios(Correo, Contrasena) values (@Correo, @Contrasena) /* Inserta correo y clave */
		set @Registrado = 1 /* Toma el valor de 1 en caso de que se registre (V)*/
		set @Mensaje = 'Usuario Registrado'
	end
	else
	begin
		set @Registrado = 0 /* Toma el valor de 0 en caso de que ya esté registrado (F) */
		set @Mensaje = 'El Correo Ya Existe'
	end
end

/*-------------------------------------------------------------------------------*/
create procedure sp_Validarusuario
(
	@Correo varchar (100), /* Recibe parametros de entrada*/
	@Contrasena varchar(100)
)
as
begin 
	/* Validar usuario con los datos que se envian (@Correo y @Contrasena) si el usuario existe*/
	if(exists(select * from Usuarios where Correo = @Correo and Contrasena = @Contrasena))
		/* Si el usuario existe selecciona el idUsuario con su respectivo correo y contraseña  */
		select idUsuario from Usuarios where Correo = @Correo and Contrasena = @Contrasena
		else
			select '0' /* Si no encuentra al usuario, retorname el valor 0 */
end

/*------------------------Registrar Usuario--------------------------------------------------------------*/

declare @registrado bit, @mensaje varchar(100)
								/* Contraseña cifrada en SHA256 */
exec sp_RegistrarUsuario 'omar@gmail.com','b460b1982188f11d175f60ed670027e1afdd16558919fe47023ecd38329e0b7f',
	@registrado output, @mensaje output

select @registrado /* 1 = registrado --- 0 = ya esta registrado */
select @mensaje

/*------------------------Validar Usuario--------------------------------------------------------------*/

exec sp_Validarusuario 'omar@gmail.com','b460b1982188f11d175f60ed670027e1afdd16558919fe47023ecd38329e0b7f'

select * from Usuarios