CREATE procedure sp_TDSgsliRetiring as
declare @finstartyear NVARCHAR(20);
declare @finendyear NVARCHAR(20);
declare @currentmonth NVARCHAR(20);
declare @retiredate NVARCHAR(20);
declare @empcode int;
declare @fy int;
declare @pfid int;
declare @id int;
declare @name NVARCHAR(20);
declare @amount decimal(18,2);
declare @projamount decimal(18,2);
declare @totamount decimal(18,2);
declare @monthcount int;

set @finstartyear='2020-04-01'
set @finendyear='2021-03-31'
set @currentmonth=CAST((select top 1 fm from pr_emp_payslip where active=1) as NVARCHAR)

set @fy= 2021;
select Empid,Retirementdate into #RetireEmployees from Employees where Retirementdate between @finstartyear and @finendyear and Empid in(select distinct emp_code from pr_emp_payslip where fm>=@finstartyear and fm<=@currentmonth) order by Empid

While exists (SELECT * from #RetireEmployees)
begin
set @empcode =(select top 1 Empid from #RetireEmployees)
CREATE table #gsli (id int, name NVARCHAR(50),amount decimal(18,2))

Insert into #gsli values ((select '6' as id),(select 'GSLI' as name),(select sum(paydedu.dd_amount) as amount from pr_emp_payslip_deductions paydedu where paydedu.dd_name = 'GSLI' and emp_code = @empcode and paydedu.payslip_mid in(select id from pr_emp_payslip where emp_code=@empcode and fy =@fy )) )
update new_num set last_num=(select max(id)+1 from pr_emp_tds_section_deductions) where table_name='pr_emp_tds_section_deductions'
set @monthcount=(select DATEDIFF(Month,@currentmonth,@retiredate));
if(@monthcount<0)
begin
set @monthcount=0;
end
WHILE exists (SELECT top 1 * from #gsli)
begin

set @id=(select last_num from new_num where table_name='pr_emp_tds_section_deductions')
set @pfid=(select top 1 id from #gsli)
set @name=(select top 1 name from #gsli)
set @amount=(select top 1 amount from #gsli)
if(@amount is null)
begin
set @amount=0;
print @empcode;
print @amount;
end
--select * from pr_emp_payslip where fm='2020-06-01' and spl_type='Regular'
set @projamount=(select case when (paydedu.dd_amount) is null then 0 else (paydedu.dd_amount) end as amount from pr_emp_payslip_deductions paydedu where paydedu.dd_name = 'GSLI' and emp_code = @empcode and paydedu.payslip_mid in(select id from pr_emp_payslip where emp_code=@empcode and fm=@currentmonth and spl_type='Regular') and paydedu.dd_amount>0)
if(@projamount is null)
begin
set @projamount=0;
end
set @totamount=@amount+(@projamount*@monthcount);

Insert into pr_emp_tds_section_deductions values(@id,@fy,@currentmonth,(select eid from employees where empid=@empcode),@empcode,@pfid,'Section80C',@totamount,@totamount,@totamount,1,1001)

update new_num set last_num=@id+1 where table_name='pr_emp_tds_section_deductions';
set @id=@id+1;
 
 
print 'GSLI';
print @empcode;
--select * from pr_emp_tds_section_deductions
Delete from #gsli where id=@pfid and name=@name and amount=@amount;
set @pfid=0;
set @amount=0;
set @totamount=0;
end
Drop table #gsli;
--select * from #licdetails;
Delete from #RetireEmployees where Empid=@empcode
end
Drop table #RetireEmployees;