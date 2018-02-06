-- Edit this file to define your initial database model.

create table Employments
    ( Id int primary key autoincrement
    , Name string(254) 
    , Surname string(64) 
	, Patronymic string(64) null
	, Date date
	, PositionId int references Postions(Id)
	, DepartmentId int references Departments(Id)
	, RoomId int references Rooms(Id)
	, City string(100)
	, Adres string(100)
	, Telphone string(20)
    );

create table Postions
    ( Id int primary key autoincrement
    , PositionName string(100)
	, Salary int
    );
create table Departments
    ( Id int primary key autoincrement
    , DepartmentName string(100)
	, RoomName string(60)
    );


create index IX_Postions_PositionId on Employments
	(PositionId);;

create index IX_Departments_DepartmentId on Employments
	(DepartmentId);;


