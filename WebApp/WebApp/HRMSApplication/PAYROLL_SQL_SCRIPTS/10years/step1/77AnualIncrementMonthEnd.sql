--Month END INSERTION OF pr_emp_inc_anual_stag

create procedure increments_month_end @pay_period varchar(30)
as
begin

--total emp codes
select pay_period from pr_basic_anual_incr_master where pay_period=@pay_period;

select distinct(emp_code) into #emp_temp from pr_emp_biological_field where active=1;
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

declare @temp1 int;
declare @temp2 int;


select @temp1=d.id from pr_emp_designation d join Designations m 
		   on d.designation=m.Name join Employees e on e.CurrentDesignation = m.Id where e.EmpId=@ID;
select @temp2=amount from pr_emp_pay_field where emp_code=@ID and active=1 and m_id=(select id from pr_earn_field_master where name='Basic' and type='pay_fields');
create table #tab (increment int, inc_type varchar(30));
declare @result int;
declare @inc int;
declare @inc_type varchar(30);
INSERT INTO #tab exec a @temp1,@pay_period,@temp2;
select @inc = (select top 1 increment from #tab order by increment asc);
select @inc_type = (select top 1 inc_type from #tab order by inc_type asc);
set @result = @inc;
drop table #tab;
update pr_emp_inc_anual_stag set active=0 where emp_code=@ID and active=1;
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
           ,[trans_id]
		   ,[Increment_Date_WEF])
     VALUES
           (@new_id,(select fy from pr_month_details where active=1)
           ,(select fm from pr_month_details where active=1)
           ,(select id from Employees where EmpId=@ID)
           ,@ID
           ,(select amount from pr_emp_pay_field where emp_code=@ID and active=1 and m_id=(select id from pr_earn_field_master where name='Basic' and type='pay_fields'))
           ,case when @result is null then 0 else @result end
		   ,@inc_type
           ,(select revision_of_date_change from pr_emp_biological_field where emp_code=@ID and active=1)
           ,0
           ,0
           ,0
           ,1
           ,1000
		   ,(select revision_of_date_change from pr_emp_biological_field where emp_code=@ID and active=1));

delete from #emp_temp where emp_code=@ID;
end
--drop 
drop table #emp_temp;
--update new num into pr_emp_inc_anual_stag
declare @lastnum int;
set @lastnum = (select top 1 id from pr_emp_inc_anual_stag order by id desc)
update new_num set last_num=@lastnum where table_name='pr_emp_inc_anual_stag';
end

--exec increments_month_end '2016-02-01'

--select * from pr_emp_inc_anual_stag;

--delete  from pr_emp_inc_anual_stag;
