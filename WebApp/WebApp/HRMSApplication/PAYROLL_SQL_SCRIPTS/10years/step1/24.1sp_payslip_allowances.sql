--exec sp_payslip_allowances
CREATE procedure sp_payslip_allowances as
begin
declare @id bigint;
declare @empid int;
declare @amount decimal(18,2);
declare @payslipid bigint;
--drop table #FacultyAllow
SELECT m.Empid, m.FCLTYALLW,p.id  into #FacultyAllow FROM [test2].[dbo].[pempmast] m  inner join [pr_emp_payslip] p on  m.[Empid] collate SQL_Latin1_General_CP1_CI_AS = p.[emp_code] where  m.FCLTYALLW!=0
set @id=(select last_num+1 from new_num where table_name='pr_emp_payslip_allowance')
while exists(SELECT * from #FacultyAllow)
begin

set @empid =(select top 1 empid from #FacultyAllow)
set @amount=(select top 1 FCLTYALLW from #FacultyAllow)
set @payslipid=(select top 1 id from #FacultyAllow)

Insert into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,all_amount,all_type,active,trans_id) values (@id,(select id from Employees where empid=@empid),@empid,@payslipid,(select id from pr_allowance_field_master where name='FACULTY ALLOWANCE'),'FACULTY ALLOWANCE',@amount,(select type from pr_allowance_field_master where name='FACULTY ALLOWANCE'),(select active from pr_month_details where fm=(select fm from pr_emp_payslip where id=@payslipid)),1000)

delete from #FacultyAllow where empid=@empid and FCLTYALLW=@amount and id=@payslipid;
set @id=@id+1;
end
DROP table #FacultyAllow
update new_num set last_num=@id where table_name='pr_emp_payslip_allowance'
end