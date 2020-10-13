use Cinema
go

create trigger Add_to_schedule
	on FilmInfo
	after insert
	as
		insert into Schedule (id_film, datetime, time, countTicket)
		select id_film, premiere, '16:30', 20 from INSERTED



create trigger Delete_order_of_schedule
	on Schedule
	instead of delete
	as
	begin
		delete from Orders where Orders.id_schedule = (select id_schedule from deleted);
		delete from Schedule where id_schedule = (select id_schedule from deleted);
	end;
/*
insert into FilmInfo values
	('Гарри Поттер', 'Триллер', 16, '2019-12-22', 'Джон Уизли', 'Стив Кловз', 
	'Гарри, Рон и Гермиона поступают на 4 курс школы чародейства');

insert into FilmInfo values
	('Наруто: Шиппуден', 'Мелодрама', 12, '2019-12-24', 'Масаши Кишимото', 'Шикамару Нара', 
	'жизнь шумного непоседливого ниндзя, который мечтает стать хокаге');

	select * from FilmInfo;
	select * from Schedule;
*/


