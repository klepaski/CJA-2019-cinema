
-------------------------------------- I M P O R T    X M L ------------------------------------------
create PROCEDURE IMPORTXML
AS
set nocount on
declare @trancount int
set @trancount = @@TRANCOUNT
BEGIN Try
if @trancount = 0 begin tran
else save tran UPDATERATING
	set nocount on
	declare @isExist int
	EXEC xp_fileexist N'D:\import.txt', @isExist output
	if cast(@isExist as bit) = 0 Raiserror(N'File does not exist or not accessible due system permissions', 11, 1)
	
	declare @qwe xml 
    select @qwe = Convert(xml, (Select * FROM OPENROWSET(  
		BULK 'D:\import.txt',  
		SINGLE_BLOB) as xmlData))
	
	if exists (select top 1 * from Users where Users.login = (select Child.value ('@login', 'nvarchar(50)') 
				from @qwe.nodes('/Cinema.dbo.Users') as N(Child))) 
		begin
		raiserror(N'User with such login already exists!',16,1)
		rollback; return 3
		end
	else 
		begin
		insert into Users(login, password, nameUser, surnameUser, email, role)
			(select Child.value ('@login', 'nvarchar(50)'),
			Child.value('@nameUser', 'nvarchar(50)'),
			Child.value('@surnameUser', 'nvarchar(50)'),
			Child.value('@password', 'nvarchar(50)'),
			Child.value('@email', 'nvarchar(100)'),
			Child.value('@role', 'varchar(50)') 
		from @qwe.nodes('/Cinema.dbo.Users') as N(Child))
		if @trancount = 0 commit
		end
END try
Begin catch
	declare @xstate int
	set @xstate = XACT_STATE()
	if @xstate = -1 rollback;
	if @xstate = 1 and @trancount = 0 rollback;
	if @xstate = 1 and @trancount > 0 rollback tran UPDATERATING
	SELECT ERROR_MESSAGE() AS ErrorMessage;
	return 2
end catch
return 1
go


--select * from Users;
--exec IMPORTXML



------------------------------------ E X P O R T    X M L ------------------------------------------

create procedure EXPORTXML
as
begin try
set nocount on
declare @ret intexec @ret =  master..xp_cmdshell 'bcp "SELECT * FROM Cinema.dbo.Users for xml auto" queryout "D:\export.txt" -c -t -T -S USER-ой'
if @ret <> 0
begin
Raiserror('Error during write operation',11,1)
end 
end try
begin catch
		select ERROR_MESSAGE() AS ErrorMessage;
		return 2
end catch
return 1
go




sp_configure 'show advanced options', 1
go
reconfigure
go

sp_configure 'xp_cmdshell', 1
go
reconfigure
go
