
use Cinema;

CREATE FULLTEXT CATALOG cat WITH ACCENT_SENSITIVITY = ON	--��������������� �����
 AS DEFAULT
 AUTHORIZATION [dbo]
 CREATE UNIQUE INDEX ui_index ON FilmInfo(id_film);  

CREATE FULLTEXT INDEX ON FilmInfo
(description language 1033)
key index ui_index ON cat 
 WITH CHANGE_TRACKING auto					--��� ��� TestTable �����
GO


------ freetext and contains -----------

/*
select *from FilmInfo
where freetext (description, '�����* and not ���');

select *from FilmInfo
where freetext (description, '��������');

select *from FilmInfo
where contains (description, '�-����');

select *from FilmInfo
where contains(description, '"�*" near �����')

*/
--------------------------- � � � � � � � � �     T - S Q L ---------------------------------

create procedure freetextUser
(
@text nvarchar(100)
)
as
begin try
select id_film, category , title, writer, description from FilmInfo
where freetext (description, @text);
end try
begin catch
		select ERROR_MESSAGE() AS ErrorMessage;
		return 2
end catch
return 1
go



create procedure containsUser
(
@text nvarchar(100)
)
as
begin try
select id_film, category , title, writer, description from FilmInfo
where contains (description, @text);
end try
begin catch
		select ERROR_MESSAGE() AS ErrorMessage;
		return 2
end catch
return 1
go


/*
	select * from FilmInfo;

	SELECT f.*, d.title, d.description
		FROM CONTAINSTABLE(FilmInfo, description, N'"����*" and not ����� ', LANGUAGE 1033) f
		INNER JOIN FilmInfo d	
			ON f.[KEY] = d.id_film
			ORDER BY RANK DESC

	SELECT f.*, d.title, d.description
		FROM FREETEXTTABLE(FilmInfo, description, N'name', LANGUAGE 1033) f
		INNER JOIN FilmInfo d	
			ON f.[KEY] = d.id_film
			ORDER BY RANK DESC

*/
-------------------------------- � � � � � � �     T - S Q L --------------------------------------------

create procedure freetexttableUser
(
@text nvarchar(200)
)
as
begin try
 select id_film, category, title, writer, description from FilmInfo as m
  inner join freetexttable(FilmInfo, description, @text) as c 
  on m.id_film = c.[key]
  ORDER BY RANK DESC
end try
begin catch
		select ERROR_MESSAGE() AS ErrorMessage;
		return 2
end catch
return 1
go


create procedure containstableUser
(
@text nvarchar(200)
)
as
begin try
 select id_film, category, title, writer, description from FilmInfo as m
  inner join containstable(FilmInfo, description, @text) as c 
  on m.id_film = c.[key]
  ORDER BY RANK DESC
end try
begin catch
		select ERROR_MESSAGE() AS ErrorMessage;
		return 2
end catch
return 1
go



-------------------------------- S T O P     L I S T ---------------------------------------

create fulltext stoplist yourStoplist
from system stoplist;
go


--alter fulltext index on FilmInfo
--set stoplist yourStoplist;


create procedure newStopList(
@word nvarchar(max) )
as
begin try
exec ('alter fulltext stoplist yourStoplist add' +  @word + 'language 1033;');
end try
begin catch
select
ERROR_MESSAGE() as ErrorMessage;
end catch
go


create procedure deleteStopList(
@word nvarchar(max))
as
begin try
exec ('alter fulltext stoplist yourStoplist drop' +  @word + 'language 1033;');
end try
begin catch
select
ERROR_MESSAGE() as ErrorMessage;
end catch
go
