CREATE procedure sp_dm_payslip_emp_data @year1 varchar(10), @a varchar(5) as

declare @id int;
set @id=1

declare @date1 date;
set @date1=	(select cast(concat (@year1,'01') as date) as SubscriptionStart)

while (@id<=(select max(id) from employees))
begin

declare @empid int;
set @empid=(select empid from employees where  retirementdate>= @date1 and (doj<DATEADD(mm,1, @date1))  and id=@id )
--2019-11-21>= 2019-11-01
	--2052-08-31 00:00:00.000>=2019-11-01,2019-11-21 00:00:00.000<2019-12-01,--6401

	--2050-04-30 00:00:00.000>=2019-11-01,  2014-02-14 00:00:00.000	<2019-11-01---555
		if(@empid is not NULL) 
		begin
			if(@a=1) 
			begin 
			insert into dm_emp_mn_task values (@empid,@year1,'PS,PROM,IIB,ENCASH,ADHOC,TDS,PNCASH,');
			end
			else if(@a=2)
			insert into dm_emp_mn_task values (@empid,@year1,'PS,PROM,IIB,ENCASH,ADHOC,PNCASH,');
			else if(@a=3)
			insert into dm_emp_mn_task values (@empid,@year1,'PS,RD,PE,JA,PR,CB,PROM,IIB,ENCASH,ADHOC,PNCASH,');
		end


set @id=@id+1


end


--exec p1 201904,1
--exec p1 201905,2
--exec p1 201906,2
--exec p1 201907,2
--exec p1 201908,2
--exec p1 201909,2
--exec p1 201910,2
--exec p1 201911,2
--exec p1 201912,2
--exec p1 202001,2
--exec p1 202002,3

--select *  from dm_emp_mn_task where mn=201906 order by emp_code
--delete from dm_emp_mn_task 

--select *  from dm_emp_mn_task where emp_code=176
--select empid,doj,retirementdate from employees where empid=176



