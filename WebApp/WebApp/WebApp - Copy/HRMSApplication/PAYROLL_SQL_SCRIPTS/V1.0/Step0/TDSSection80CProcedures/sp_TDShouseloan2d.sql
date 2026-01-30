CREATE procedure sp_TDShouseloan2d as
declare @finstartyear NVARCHAR(20);
declare @finendyear NVARCHAR(20);
declare @currentmonth NVARCHAR(20);
declare @empcode int;
declare @fy int;
declare @pfid int;
declare @id int;
declare @name NVARCHAR(50);
declare @amount decimal(18,2);
declare @projamount decimal(18,2);
declare @totamount decimal(18,2);
declare @monthcount int;

set @finstartyear='2020-04-01'
set @finendyear='2021-03-31'
set @currentmonth=CAST((select top 1 fm from pr_emp_payslip where active=1) as NVARCHAR)

set @monthcount=(select DATEDIFF(Month,@currentmonth,@finendyear));
set @fy= 2021;
select Empid into #ActiveEmployees from Employees where Retirementdate>@finendyear and Empid in(select distinct emp_code from pr_emp_payslip where fm=@currentmonth) order by Empid

While exists (SELECT * from #ActiveEmployees)
begin
set @empcode =(select top 1 Empid from #ActiveEmployees)
CREATE table #houseloanaddl (id int, name NVARCHAR(50),amount decimal(18,2))

Insert into #houseloanaddl values ((select '5' as id),(select 'Housing Addl.Loan - 2D' as name),(select sum(paydedu.dd_amount) as amount from pr_emp_payslip_deductions paydedu where paydedu.dd_name = 'Housing Addl.Loan - 2D' and emp_code = @empcode and paydedu.payslip_mid in(select id from pr_emp_payslip where emp_code=@empcode and fy =@fy )) )
update new_num set last_num=(select max(id)+1 from pr_emp_tds_section_deductions) where table_name='pr_emp_tds_section_deductions'
WHILE exists (SELECT top 1 * from #houseloanaddl)
begin

set @id=(select last_num from new_num where table_name='pr_emp_tds_section_deductions')
set @pfid=(select top 1 id from #houseloanaddl)
set @name=(select top 1 name from #houseloanaddl)
set @amount=(select top 1 amount from #houseloanaddl)
--select * from pr_emp_payslip where fm='2020-06-01' and spl_type='Regular'
set @projamount=(select (paydedu.dd_amount) as amount from pr_emp_payslip_deductions paydedu where paydedu.dd_name = 'Housing Addl.Loan - 2D' and emp_code = @empcode and paydedu.payslip_mid in(select id from pr_emp_payslip where emp_code=@empcode and fm=@currentmonth and spl_type='Regular')and paydedu.dd_amount>0)
if(@projamount is null)
begin
set @projamount=0;
end
set @totamount=@amount+(@projamount*@monthcount);

Insert into pr_emp_tds_section_deductions values(@id,@fy,@currentmonth,(select eid from employees where empid=@empcode),@empcode,@pfid,'Section80C',@totamount,@totamount,@totamount,1,1001)

update new_num set last_num=@id+1 where table_name='pr_emp_tds_section_deductions';
set @id=@id+1;
 
 
print 'Housing Addl.Loan - 2D';
print @empcode;
print @amount;
print @totamount;
--select * from pr_emp_tds_section_deductions
Delete from #houseloanaddl where id=@pfid and name=@name and amount=@amount;
set @pfid=0;
set @amount=0;
set @totamount=0;
end
Drop table #houseloanaddl
--select * from #licdetails;
Delete from #ActiveEmployees where Empid=@empcode
end
Drop table #ActiveEmployees;