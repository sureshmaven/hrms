create procedure sp_dm_ob_share_emp_data @year1 varchar(10) as
/*
declare @id int;
set @id=1

declare @date1 date;
set @date1=	(select cast(concat (@year1,'01') as date) as SubscriptionStart)

while (@id<=(select max(id) from employees))
begin

declare @empid int;
set @empid=(select empid from employees where id=@id )

insert into dm_emp_mn_task values (@empid,@year1,'OB,');

set @id=@id+1
end
*/
insert into dm_emp_mn_task select empid,@year1,'OB,' from employees;

--exec sp_dm_ob_share_emp_data 201904

--select *  from dm_emp_mn_task where mn=201906 order by emp_code
--delete from dm_emp_mn_task 

--select *  from dm_emp_mn_task where emp_code=176
--select empid,doj,retirementdate from employees where empid=176
--select * from dm_emp_mn_task

