--DROP ALL DATABASE OBJECTS
--Basic settings

USE master
go


CREATE DATABASE Cinema ON PRIMARY
(
	NAME = 'FilmInfo',
	FILENAME = 'D:\FilmInfo.mdf',
	SIZE = 512MB,
	MAXSIZE = UNLIMITED,
	FILEGROWTH = 2048KB
),

-- Secondary File Group
FILEGROUP SecondaryFileGroup
(
	NAME = 'SecondaryTables1',
	FILENAME = 'D:\SecondaryTables1.ndf',
	SIZE = 256MB,
	MAXSIZE = UNLIMITED,
	FILEGROWTH = 1024KB
),
(
	NAME = 'SecondaryTables2',
	FILENAME = 'D:\SecondaryTables2.ndf',
	SIZE = 256MB,
	MAXSIZE = UNLIMITED,
	FILEGROWTH = 1024KB
)
LOG ON
(
	-- Log File
	NAME = 'DB_Log',
	FILENAME = 'D:\LogForCinema.ldf',
	SIZE = 5MB,
	MAXSIZE = 64MB,
	FILEGROWTH = 1024KB
)
GO



--имя входа sql server
CREATE LOGIN CinemaAdmin WITH PASSWORD = '0000', DEFAULT_DATABASE = Cinema, CHECK_EXPIRATION = OFF, CHECK_POLICY = On;
go
CREATE LOGIN CinemaUser WITH PASSWORD = '0000', DEFAULT_DATABASE = Cinema, CHECK_EXPIRATION = OFF, CHECK_POLICY = On;
go
CREATE LOGIN CinemaGuest WITH PASSWORD = '0000', DEFAULT_DATABASE = Cinema, CHECK_EXPIRATION = OFF, CHECK_POLICY = On;
go


CREATE USER CinemaAdmin FOR LOGIN CinemaAdmin WITH DEFAULT_SCHEMA=[dbo]
CREATE USER CinemaUser FOR LOGIN CinemaUser WITH DEFAULT_SCHEMA=[dbo]
CREATE USER CinemaGuest FOR LOGIN CinemaGuest WITH DEFAULT_SCHEMA=[dbo]


CREATE ROLE CinemaUser_Role
go
grant execute on CREATEUSER to CinemaUser_Role
grant execute on NEWORDER to CinemaUser_Role
grant execute on DELETEUSER to CinemaUser_Role
grant execute on DELETEORDER to CinemaUser_Role
grant execute on UPDATEUSER to CinemaUser_Role
grant execute on ENTRANCE to CinemaUser_Role
grant execute on GETSCHEDULE  to CinemaUser_Role
grant execute on GETFILM  to CinemaUser_Role
grant execute on GETUSER  to CinemaUser_Role
grant execute on GETUSERORDER  to CinemaUser_Role
grant execute on POISKFILM  to CinemaUser_Role
go

CREATE ROLE CinemaGuest_Role
go
grant execute on ENTRANCE  to CinemaGuest_Role
grant execute on GETSCHEDULE  to CinemaGuest_Role
grant execute on GETFILM  to CinemaGuest_Role
grant execute on POISKFILM  to CinemaGuest_Role
go


EXEC sp_addrolemember 'db_owner', 'CinemaAdmin'
go
exec sp_addrolemember 'CinemaUser_Role', 'CinemaUser'
go
exec sp_addrolemember 'CinemaGuest_Role', 'CinemaGuest'
go

------------------------------------------- С О З Д А Н И Е     Т А Б Л И Ц ---------------------------------------------------------

use Cinema
go

create table FilmInfo (
	id_film int not null IDENTITY(1000,1),
	title nvarchar(300) not null,
	category nvarchar(10) not null check (category = 'Триллер'or category = 'Мелодрама' or category = 'Комедия'),
	allowable_age int not null check (allowable_age <=18 and allowable_age >= 0),
	premiere date not null,
	writer nvarchar(200) not null,
	directors nvarchar(max) not null,
	description nvarchar(max) not null,
	CONSTRAINT PK_IDFILM PRIMARY KEY CLUSTERED (id_film)
)

create table Schedule (
	id_schedule int not null IDENTITY(1000,1),
	id_film int not null,
	datetime date not null,
	time nvarchar(10) not null,
	countTicket int not null check (countTicket <= 150),	
	CONSTRAINT PK_IDSCHEDULE PRIMARY KEY CLUSTERED (id_schedule),
	CONSTRAINT FK_SCHEDULE_FILM FOREIGN KEY (id_film) REFERENCES FilmInfo (id_film),
)

create table Users (
    id_user int not null identity(1,1),
	login nvarchar(50) not null,
	password nvarchar(200) not null,	-----
	nameUser nvarchar(50) not null,
	surnameUser nvarchar(50) not null,
	email nvarchar(100) not null,
	role nvarchar(50) not null check(role in('admin', 'user', 'guest')),
	CONSTRAINT PK_IDUSER PRIMARY KEY CLUSTERED (id_user)
) 

create table Orders (
	id_order int not null IDENTITY(1,1),
	id_user int not null,
	id_schedule int not null,
	countOrder int not null
	CONSTRAINT PK_IDODER PRIMARY KEY CLUSTERED (id_order),
	CONSTRAINT FK_ORDER_USER FOREIGN KEY (id_user) REFERENCES Users(id_user),
	CONSTRAINT FK_ORDER_SCHEDULE FOREIGN KEY (id_schedule) REFERENCES Schedule(id_schedule)
) 
go

----------------------------------- С О З Д А Н И Е      П Р О Ц Е Д У Р ----------------------------------------
create PROCEDURE CREATEUSER
  (
  @login nvarchar(50),
  @password nvarchar(50),
  @nameUser nvarchar(50),
  @surnameUser nvarchar(50),
  @email nvarchar(100),
  @role nvarchar(50) ,
  @return int out
  )
AS
BEGIN Try
	if EXISTS (select top 1 * from Users where Users.login = @login)
	Begin 
		select @return=0
	end
	else
	begin insert into Users values (@login, @password,@nameUser, @surnameUser, @email, @role)
	select @return =1
   commit
   end
END try
Begin catch
SELECT
		ERROR_MESSAGE() AS ErrorMessage;
end catch
go



create PROCEDURE CREATEFILM
  (
  @title nvarchar(300),
  @category nvarchar(10), 
  @allowable_age int,
  @premiere date,
  @writer nvarchar(200), 
  @directors nvarchar(max),
  @description nvarchar(max)
  )
AS
begin tran
BEGIN Try
  insert into FilmInfo(title, category, allowable_age, premiere, writer, directors, description) 
  values (@title, @category, @allowable_age, @premiere, @writer, @directors, @description)
 commit
END try
Begin catch
rollback
SELECT
		ERROR_MESSAGE() AS ErrorMessage;
end catch
go


create PROCEDURE CREATESCHEDULE
  (
  @id_film int,
  @datetime date,
  @time nvarchar(10),
  @countTicket int
  )
AS
begin tran
BEGIN Try
	insert into Schedule(id_film, datetime, time, countTicket) values (@id_film, @datetime, @time, @countTicket)
	commit
END try
Begin catch
	rollback
	SELECT ERROR_MESSAGE() AS ErrorMessage;
end catch
go



create PROCEDURE DELETEUSER (
  @id_user int
  )
AS
begin tran
BEGIN Try
	if Not EXISTs (select top 1 * from Users where Users.id_user = @id_user) Raiserror(N'Seems the user that trying to delete does not exist',11,1)
	delete from Orders where Orders.id_user  = @id_user
	delete from Users where Users.id_user = @id_user
	commit
END try
Begin catch
	rollback
	SELECT ERROR_MESSAGE() AS ErrorMessage;
end catch
go



create PROCEDURE DELETEFILM (
  @id_film int
  )
AS
begin tran
BEGIN Try
	if not exists (select top 1 * from FilmInfo where FilmInfo.id_film = @id_film) Raiserror(N'Specified ID Film does not exist',11,1)
	else
	begin
	--delete from Orders where Orders.id_schedule in (select Schedule.id_schedule from Schedule where Schedule.id_film = @id_film)
	delete from Schedule where Schedule.id_film = @id_film
	delete from FilmInfo where FilmInfo.id_film = @id_film
commit
end 
END try
Begin catch
	rollback
	SELECT ERROR_MESSAGE() AS ErrorMessage;
end catch
go



create PROCEDURE DELETESCHEDULE (
  @id_schedule int
  )
AS
begin tran
BEGIN Try
	if not exists (select top 1 * from Schedule where Schedule.id_schedule = @id_schedule) raiserror(N'Trying to delete not existing schedule record',11,1)
	--delete from Orders where Orders.id_schedule = @id_schedule
   delete from Schedule where Schedule.id_schedule = @id_schedule
   commit
END try
Begin catch
	rollback
	SELECT ERROR_MESSAGE() AS ErrorMessage;
end catch
go


create PROCEDURE UPDATEUSER
  (
  @id_user int,
  @login nvarchar(50),
  @password nvarchar(50),
  @nameUser nvarchar(50),
  @surnameUser nvarchar(50),
  @email nvarchar(100)
  )
AS
begin tran
BEGIN Try
	if Not EXISTs (select top 1 * from Users where Users.id_user = @id_user)
	 Raiserror(N'Seems the user that you trying to change does not exist',11,1)
  else 
  begin
  if not exists (SELECT top 1* From Users WHERE Users.login = @login 
  and Users.id_user IN (SELECT Users.id_user FROM Users where Users.id_user = @id_user))
    update Users set Users.login = @login where Users.id_user = @id_user
    update Users set Users.password = @password where Users.id_user = @id_user
    update Users set Users.nameUser = @nameUser where Users.id_user = @id_user
    update Users set Users.surnameUser = @surnameUser where Users.id_user = @id_user
    update Users set Users.email = @email where Users.id_user = @id_user
 commit
   end
END try
Begin catch
	rollback
	SELECT ERROR_MESSAGE() AS ErrorMessage;
end catch
go


create PROCEDURE UPDATEFILM
  (
  @id_film int,
  @title nvarchar(300) = NULL,
  @category nvarchar(10) = NULL, 
  @allowable_age int = NULL,
  @premiere date,
  @writer nvarchar(200)= NULL, 
  @directors  nvarchar(max)= NULL,
  @performes  nvarchar(max)= NULL,
  @description nvarchar(max) = NULL
  )
AS
begin tran
BEGIN Try
	if Not EXISTs (select top 1 * from FilmInfo where FilmInfo.id_film = @id_film) Raiserror(N'Seems the title you intend to change does not exist',11,1)
   update FilmInfo set FilmInfo.title = @title where FilmInfo.id_film = @id_film and @title is not null
   update FilmInfo set FilmInfo.category = @category where FilmInfo.id_film = @id_film and @category is not null
   update FilmInfo set FilmInfo.allowable_age = @allowable_age where FilmInfo.id_film = @id_film and @allowable_age is not null
   update FilmInfo set FilmInfo.premiere = @premiere where FilmInfo.id_film = @id_film and @premiere is not null
   update FilmInfo set FilmInfo.writer = @writer where FilmInfo.id_film = @id_film and @writer is not null
   update FilmInfo set FilmInfo.directors = @directors where FilmInfo.id_film = @id_film and @directors is not null
   update FilmInfo set FilmInfo.description = @description where FilmInfo.id_film = @id_film and @description is not null
 commit
END try
Begin catch
rollback
SELECT ERROR_MESSAGE() AS ErrorMessage;
end catch
go



create procedure ENTRANCE
(
@login nvarchar(50),
@password nvarchar(50)
)
AS
BEGIN Try
	if EXISTS (select top 1 * from Users where Users.login = @login and Users.password = @password)
		return 1
	else  return 0
   commit
END try
Begin catch
	rollback
	SELECT ERROR_MESSAGE() AS ErrorMessage;
end catch
go



CREATE PROCEDURE GETUSER
(
@login nvarchar(50),
@id_user int out,
@nameUser nvarchar(50) out ,
@surnameUser nvarchar(50) out,
@emailUser nvarchar(100) out,
@role nvarchar(50) out
)
as 
begin try
select @id_user = Users.id_user, @nameUser = Users.nameUser, @surnameUser = Users.surnameUser,
       @emailUser = Users.email, @role = Users.role from Users where Users.login = @login

end try
begin catch
	rollback
	select  ERROR_MESSAGE() as ErrorMessage;
end catch
go



CREATE PROCEDURE GETSCHEDULE
AS
BEGIN TRY
SELECT Schedule.id_schedule, Schedule.datetime, Schedule.time, FilmInfo.category, FilmInfo.title, Schedule.countTicket
 From Schedule Inner Join FilmInfo On Schedule.id_film = FilmInfo.id_film order by Schedule.datetime, Schedule.time
END TRY 
BEGIN CATCH
	ROLLBACK
	SELECT ERROR_MESSAGE() AS ErrorMessage;
end catch
go



CREATE PROCEDURE GETUSERORDER
	(
	@id_user int
	)
AS
BEGIN TRY
	SELECT Orders.id_order, Schedule.datetime, Schedule.time, 
	FilmInfo.category, FilmInfo.title, Orders.countOrder 
	FROM Orders INNER JOIN
	Schedule on Schedule.id_schedule = Orders.id_schedule and  Orders.id_user = @id_user
	inner join FilmInfo on Schedule.id_film = FilmInfo.id_film

END TRY 
BEGIN CATCH
	ROLLBACK
	SELECT ERROR_MESSAGE() AS ErrorMessage;
end catch
go



create procedure POISKFILM
(
@datetime date = null,
@category nvarchar(10) = null
)
as
begin try
	if @datetime is not null and @category is not null
		SELECT Schedule.id_schedule, Schedule.datetime, Schedule.time,
		 FilmInfo.category, FilmInfo.title, Schedule.countTicket 
		 From Schedule Inner Join FilmInfo On
		  Schedule.id_film = FilmInfo.id_film and 
		  Schedule.datetime = @datetime and FilmInfo.category = @category order by Schedule.datetime, Schedule.time
	 else if @datetime is not null 
		select Schedule.id_schedule, Schedule.datetime, Schedule.time,
		 FilmInfo.category, FilmInfo.title, Schedule.countTicket 
		 From Schedule Inner Join FilmInfo On
		  Schedule.id_film = FilmInfo.id_film and 
		  Schedule.datetime = @datetime order by Schedule.datetime, Schedule.time
	 else if  @category is not null 
		  select Schedule.id_schedule, Schedule.datetime, Schedule.time,
		 FilmInfo.category, FilmInfo.title, Schedule.countTicket 
		 From Schedule Inner Join FilmInfo On
		  Schedule.id_film = FilmInfo.id_film and 
		  FilmInfo.category = @category order by Schedule.datetime, Schedule.time
		   else
	SELECT Schedule.id_schedule, Schedule.datetime, Schedule.time, FilmInfo.category, FilmInfo.title, Schedule.countTicket 
	From Schedule Inner Join FilmInfo On Schedule.id_film = FilmInfo.id_film order by Schedule.datetime, Schedule.time
end try
begin catch
	rollback
	select ERROR_MESSAGE() as ErrorMessage;
end catch
go



CREATE PROCEDURE GETFILM
as 
begin try
	select id_film, title, category, allowable_age, 
	premiere, writer, directors, description from FilmInfo
end try
begin catch
	rollback
	select  ERROR_MESSAGE() as ErrorMessage;
end catch
go