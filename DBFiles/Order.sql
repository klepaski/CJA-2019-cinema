
 ------------------------ C R E A T E    O R D E R ------------------

CREATE PROCEDURE NEWORDER 
(
@id_user int,
@id_schedule int,	
@countOrder int
)
as
begin try
	if exists (select top 1 * from Orders where Orders.id_user = @id_user and Orders.id_schedule = @id_schedule)
	begin
		Raiserror(N'Вы уже заказывали билет на данное мероприятие!',16,1)
		commit
		return
	end
	if exists (select top 1 * from Schedule where Schedule.id_schedule = @id_schedule AND Schedule.countTicket < @countOrder)
	begin
		Raiserror(N'Билетов недостаточно!',17,1)
		commit
		return
	end
	else
	begin
		insert into Orders values (@id_user, @id_schedule, @countOrder)
		update Schedule  set Schedule.countTicket -= @countOrder
			from Schedule where Schedule.id_schedule = @id_schedule
		commit
	end
end try
begin catch
	select ERROR_MESSAGE() as ErrorMessage;
end catch
go

--exec NEWORDER @id_user=2, @id_schedule=1000, @countOrder=27;




-----------------------  D E L E T E    O R D E R ------------------------ 


create PROCEDURE DELETEORDER
  (
  @id_order int
  )
AS
begin tran
BEGIN Try
	if not exists (select top 1 * from Orders where Orders.id_order = @id_order) 
		begin
		raiserror(N'Trying to delete not existing order',16,1)
		commit
		end
	else 
	begin
	 	 update Schedule set Schedule.countTicket += Orders.countOrder
			 from Schedule inner join Orders 
			 on Schedule.id_schedule = Orders.id_schedule and Orders.id_order = @id_order
			 where Schedule.id_schedule in (select id_schedule from Orders WHERE id_order = @id_order)  	 
		 delete from Orders where Orders.id_order = @id_order
	 commit
	end
END try
Begin catch
	rollback--
	SELECT ERROR_MESSAGE() AS ErrorMessage;
end catch
go

--exec deleteorder @id_order=12;