create procedure sm_dm_attendance

--exec sm_dm_attendance

--select * from pr_month_attendance;create procedure sm_dm_attendance
as
begin
select distinct(emp_code) into #general_codes from pr_emp_general where active=1;
declare @employeecode AS NVARCHAR(100);

declare @new_num int;
declare @new_id int;






set @new_num=(select last_num from new_num where table_name = 'pr_month_attendance');
set @new_id = @new_num;

while exists(select * from #general_codes)
begin
select @employeecode = (select top 1 emp_code from #general_codes order by emp_code asc);
declare @bal NVARCHAR(100);
declare @balQry NVARCHAR(100);

set @balQry = 'select @bal = dbo.getLeaveBalance('+@employeecode+');';
execute sp_executesql 
    @balQry, 
    N'@bal NVARCHAR(100) OUTPUT', 
    @bal = @bal output;
	--print(@bal);
set @new_id = @new_id+1;
insert into pr_month_attendance(id,fy,fm,emp_id,emp_code,status,status_date,leaves_available,lop_days,absent_days,working_days,active,trans_id) 
values(@new_id,(select fy from pr_month_details where active=1),(select fm from pr_month_details where active=1),(select id from Employees where EmpId=@employeecode),@employeecode,
'Regular',null,@bal,0,0,(select month_days from pr_month_details where active=1),1,3000);
delete from #general_codes where emp_code=@employeecode;
end
declare @lastnum_lic int;
set @lastnum_lic = (select top 1 id from pr_month_attendance order by id desc)
update new_num set last_num=@lastnum_lic where table_name='pr_month_attendance';

drop table #general_codes;
end

--exec sm_dm_attendance

--select * from pr_month_attendance;


--delete from pr_month_attendance;


