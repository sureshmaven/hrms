

--FOR INTIAL INSERTION INTO pr_emp_inc_anual_stag
create procedure sp_dm_increments @pmonth as int, @employeecode as int
as
begin

declare @pay_period NVARCHAR(MAX);
set @pay_period = cast(left(@pmonth,4) as varchar(20))+'-'+ right(@pmonth,2)+'-'+'01';
--total emp codes
--select pay_period from pr_basic_anual_incr_master where pay_period=@pay_period;

select distinct(emp_code) into #emp_temp from pr_emp_biological_field where active=1 and emp_code=@employeecode;
declare @ID int;
declare @new_id int;
declare @new_num int;
set @new_num=(select last_num from new_num where table_name = 'pr_emp_payslip_allowance');

set @new_id = @new_num;

while exists (select * from #emp_temp)
begin
--each code
set @new_id = @new_id+1;
select @ID = (select top 1 emp_code from #emp_temp order by emp_code asc);

declare @des_id int;
declare @emp_basic int;


select @des_id=d.id from pr_emp_designation d join Designations m 
		   on d.designation=m.Name join Employees e on e.CurrentDesignation = m.Id where e.EmpId=@ID;
select @emp_basic=amount from pr_emp_pay_field where emp_code=@ID and active=1 and m_id=(select id from pr_earn_field_master where name='Basic' and type='pay_fields');
create table #tab (increment int, inc_type varchar(30));
declare @result int;
declare @inc int;
declare @inc_type varchar(30);
-----------
--INSERT INTO #tab exec a @des_id,@pay_period,@emp_basic;

if (select max(basic) from pr_basic_anual_incr_master where 
designation_id =@des_id and pay_period=@pay_period)>@emp_basic 

begin

INSERT INTO #tab select top 1 increment,'annual' as type from pr_basic_anual_incr_master where 
designation_id =@des_id and pay_period=@pay_period and @emp_basic >=basic order by basic desc
end
else

begin

INSERT INTO #tab select top 1 increment,'stagnation' as type from pr_basic_stag_incr_master where 
designation_id =@des_id and pay_period=@pay_period and @emp_basic >=basic order by basic desc
end
-----------

select @inc = (select top 1 increment from #tab order by increment asc);
select @inc_type = (select top 1 inc_type from #tab order by inc_type asc);
set @result = @inc;
drop table #tab;
--insert into increment anual stagnation
INSERT INTO [dbo].[pr_emp_inc_anual_stag]
           ([id]
           ,[fy]
           ,[fm]
           ,[emp_id]
           ,[emp_code]
           ,[basic_amount]
           ,[increment_amount]
		   ,[increment_type]
           ,[increment_date]
           ,[process]
           ,[authorisation]
           ,[post_process]
           ,[active]
           ,[trans_id])
     VALUES
           (@new_id,(select fy from pr_month_details where active=1)
           ,(select fm from pr_month_details where active=1)
           ,(select id from Employees where EmpId=@ID)
           ,@ID
           ,(select amount from pr_emp_pay_field where emp_code=@ID and active=1 and m_id=(select id from pr_earn_field_master where name='Basic' and type='pay_fields'))
           ,@result
		   ,@inc_type
           ,(select revision_of_date_change from pr_emp_biological_field where emp_code=@ID and active=1)
           ,0
           ,0
           ,0
           ,1
           ,1000);

delete from #emp_temp where emp_code=@ID;
end
--drop 
drop table #emp_temp;
--update new num into pr_emp_inc_anual_stag
declare @lastnum int;
set @lastnum = (select top 1 id from pr_emp_inc_anual_stag order by id desc)
update new_num set last_num=@lastnum where table_name='pr_emp_inc_anual_stag';
end

--exec sp_dm_increments 201602,175

--select * from pr_emp_inc_anual_stag;

--delete from pr_emp_inc_anual_stag;

--select * from pr_basic_anual_incr_master;
--select * from pr_basic_stag_incr_master;

--delete from pr_basic_anual_incr_master;
--delete from pr_basic_stag_incr_master;