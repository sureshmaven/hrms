

--exec sp_dm_payslip 304,'201910'
--exec sp_dm_payslip 304,'201909'
--exec sp_dm_payslip 304,'201908'
--exec sp_dm_payslip 304,'201907'
--exec sp_dm_payslip 304,'201906'
--exec sp_dm_payslip 304,'201905'
--exec sp_dm_payslip 304,'201904'



create procedure [dbo].[sp_dm_payslip](@employeecode int,@pmonth varchar(20))
as 
begin
declare @fy NVARCHAR(100);
declare @fyQry NVARCHAR(100);
set @fyQry = 'select @fy = dbo.getFY('+@pmonth+');';
execute sp_executesql 
    @fyQry, 
    N'@fy NVARCHAR(100) OUTPUT', 
    @fy = @fy output;

declare @fm NVARCHAR(MAX);
set @fm = cast(left(@pmonth,4) as varchar(20))+'-'+ right(@pmonth,2)+'-'+'01';

set nocount on;
declare @new_id int;
declare @query  AS NVARCHAR(MAX);
declare @query_final as nvarchar(max);
declare @tablename as varchar(40);
IF EXISTS (select * from [TEST2].dbo.pset where MFlag='N' AND PMonth=@pmonth)
begin
declare @StartValue int;
set @StartValue=(select last_num from new_num where table_name = 'pr_emp_payslip');
declare @initValue int;

set @initValue = @StartValue+1;

select @initValue as id,getdate() as gen_date,@fy as fy,
@fm as fm,(select id from employees where empid = @employeecode) as emp_id,@employeecode as emp_code,
(select d.Name from  Employees e join Designations d on e.CurrentDesignation = d.Id 
where e.EmpId= @employeecode) as designation,
(select d.Name from  Employees e join Branches d on e.Branch = d.Id where e.EmpId= @employeecode) as branch,
p.WorkDays as working_days,
p.LopDays as lop,p.Basic as er_basic,p.DA as er_da,p.CCA as er_cca,p.HRA as er_hra,
p.IRLF as er_interim_relief,p.TSINCR as er_telangana_inc,p.Grpay as gross_amount,
p.PF as dd_provident_fund,p.Tds as dd_income_tax,p.Ptax as dd_prof_tax,p.SUBCLUB as dd_club_subscription,
p.TGASSN as dd_telangana_officers_assn,p.Netpay as net_amount,p.Deductions as deductions_amount, 0 as active,1000 as trans_id,p.SBASICDA as spl_da,p.SBASICALW as spl_allw, 'Regular' as spl_type, 1 as final_process 
into #pr_emp_payslip
from [TEST2].dbo.PPayslip p join [TEST2].dbo.pempmast m on p.EID = m.EID  where m.Empid = @employeecode and p.Pmonth=@pmonth;



--select * from #pr_emp_payslip;

--select * from pr_emp_payslip


insert into pr_emp_payslip(id,gen_date,fy,
fm,emp_id,emp_code,
designation,branch,working_days,lop,er_basic,er_da,er_cca,er_hra,er_interim_relief,er_telangana_inc,
gross_amount,dd_provident_fund,dd_income_tax,dd_prof_tax,dd_club_subscription,dd_telangana_officers_assn,
net_amount,deductions_amount,active,trans_id,spl_da,spl_allw,spl_type,final_process) select * from #pr_emp_payslip;


declare @lastnum int;
set @lastnum = (select top 1 id from pr_emp_payslip order by id desc)
update new_num set last_num=@lastnum where table_name='pr_emp_payslip';

--get good will data and keep in temp table for that employee
Select p.* 
into #gw_data 
from [TEST2].dbo.PPayslip p join [TEST2].dbo.pempmast m on p.EID = m.EID  where m.Empid = @employeecode and p.Pmonth=@pmonth;

CREATE  TABLE #codes_master (
id INT, 
name nvarchar(100), 
code nvarchar(100),
type nvarchar(100)
);

--get codes
insert into #codes_master select id, name,code,type from pr_earn_field_master where code!='' union all 
select id, name,code,type from pr_allowance_field_master where code!='' ;

declare @ID int
declare @name nvarchar(100)
declare @code nvarchar(50)
declare @val nvarchar(50)
declare @type nvarchar(50)

declare @new_num int;
set @new_num=(select last_num from new_num where table_name = 'pr_emp_payslip_allowance');

set @new_id = @new_num;
--loop

while exists (select * from #codes_master)
begin
	set @new_id = @new_id+1;
    select @ID = (select top 1 id from #codes_master order by id asc);

	select @name = name, @code = code,@type= type from #codes_master where id = @ID;
	--get value from gw table
	set @query = 'Select @val = ' + @code + ' from #gw_data'
	execute sp_executesql 
    @query, 
    N'@val nvarchar(50) OUTPUT', 
    @val = @val output;

INSERT into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,
	all_amount,all_type,active,trans_id) 
VALUES( @new_id,(select id from employees where empid =  @employeecode  ), @employeecode ,
(select id from pr_emp_payslip where fm= @fm and  emp_code = @employeecode and spl_type='Regular'), @ID ,
@name,@val ,@type,0,1000);
	

    delete #codes_master where id = @ID and name=@name and type=@type and code=@code;
end
  
  --update new num
  declare @lastnuma int;
set @lastnuma = (select top 1 id from pr_emp_payslip_allowance order by id desc)
update new_num set last_num=@lastnuma where table_name='pr_emp_payslip_allowance';

--drop temp table

drop table #codes_master;

drop table #pr_emp_payslip;


--deduction data
CREATE TABLE #codes_master_allded (
id INT, 
name nvarchar(100), 
code nvarchar(100),
type nvarchar(100)
);


insert into #codes_master_allded select id, name,code,type from pr_deduction_field_master where code!='' union all 
select id,loan_description as name,loan_id as code,'Loan' as type from pr_loan_master where active=1 and loan_id!='PFL1' and loan_id!='PFL2' union all
select id,name,code,type from dm_payslip_loans;


declare @IDsss int
declare @namesss nvarchar(100)
declare @codesss nvarchar(50)
declare @valsss nvarchar(50)
declare @typesss nvarchar(50)
declare @new_idsss int
declare @querysss nvarchar(50)
--select * from pr_emp_pay_field
declare @new_numsss int;
set @new_numsss=(select last_num from new_num where table_name = 'pr_emp_payslip_deductions');

set @new_idsss = @new_numsss;
--loop


while exists (select * from #codes_master_allded)
begin
set @new_idsss = @new_idsss+1;
select @IDsss = (select top 1 id from #codes_master_allded order by id asc);

select @namesss = name, @codesss = code,@typesss= type from #codes_master_allded where id = @IDsss;

--get value from gw table
set @querysss = 'Select @valsss = ' + @codesss +  ' from #gw_data'
execute sp_executesql 
@querysss, 
N'@valsss nvarchar(50) OUTPUT', 
@valsss = @valsss output;


INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,
	dd_amount,dd_type,active,trans_id) 
VALUES( @new_idsss,(select id from employees where empid =  @employeecode  ), @employeecode ,
(select id from pr_emp_payslip where fm= @fm and  emp_code = @employeecode and spl_type='Regular'), @IDsss ,
@namesss,@valsss ,@typesss,0,1000);

delete #codes_master_allded where id = @IDsss and name=@namesss and type=@typesss and code=@codesss;
end

select (select last_num+1 from new_num where table_name = 'pr_emp_payslip_deductions') as id,
(select id from Employees where empid=@employeecode) as emp_id,@employeecode as emp_code,
(select id from pr_emp_payslip where emp_code=@employeecode and fy=@fy and fm=@fm and spl_type='Regular') as payslip_mid,
0 as dd_mid,'HFC' as dd_name,
p.HFC as dd_amount,'Monthly' as dd_type,0 as active,1000 as trans_id into #hfcData from test2.dbo.PPayslip p 
join test2.dbo.PEMPMAST m on p.Eid=m.Eid where m.empid=@employeecode;

insert into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) select * from #hfcData;

drop table #hfcData;


declare @lasthfc int;
set @lasthfc = (select top 1 id from pr_emp_payslip_deductions order by id desc)
update new_num set last_num=@lasthfc where table_name='pr_emp_payslip_deductions';


select (select last_num+1 from new_num where table_name = 'pr_emp_payslip_deductions') as id,
(select id from Employees where empid=@employeecode) as emp_id,@employeecode as emp_code,
(select id from pr_emp_payslip where emp_code=@employeecode  and fy=@fy and fm=@fm and spl_type='Regular') as payslip_mid,
0 as dd_mid,'LIC' as dd_name,
p.LIC as dd_amount,'Monthly' as dd_type,0 as active,1000 as trans_id into #licData from test2.dbo.PPayslip p 
join test2.dbo.PEMPMAST m on p.Eid=m.Eid where m.empid=@employeecode;

insert into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) select * from #licData;

drop table #licData;


declare @lastlic int;
set @lastlic = (select top 1 id from pr_emp_payslip_deductions order by id desc)
update new_num set last_num=@lastlic where table_name='pr_emp_payslip_deductions';

--drop temp table
--update last num into new num
declare @lastnumsss int;
set @lastnumsss = (select top 1 id from pr_emp_payslip_deductions order by id desc)
update new_num set last_num=@lastnumsss where table_name='pr_emp_payslip_deductions';

drop table #codes_master_allded;
drop table #gw_data;

end
--set @tablename='[TEST2].dbo.PPayslip';
else 
begin
declare @StartValue1 int;
set @StartValue1=(select last_num from new_num where table_name = 'pr_emp_payslip');
declare @initValue1 int;

set @initValue1 = @StartValue1+1;

select @initValue1 as id,getdate() as gen_date,@fy as fy,
@fm as fm,(select id from employees where empid = @employeecode) as emp_id,@employeecode as emp_code,
(select d.Name from  Employees e join Designations d on e.CurrentDesignation = d.Id 
where e.EmpId= @employeecode) as designation,
(select d.Name from  Employees e join Branches d on e.Branch = d.Id where e.EmpId= @employeecode) as branch,
p.WorkDays as working_days,
p.LopDays as lop,p.Basic as er_basic,p.DA as er_da,p.CCA as er_cca,p.HRA as er_hra,
p.IRLF as er_interim_relief,p.TSINCR as er_telangana_inc,p.Grpay as gross_amount,
p.PF as dd_provident_fund,p.Tds as dd_income_tax,p.Ptax as dd_prof_tax,p.SUBCLUB as dd_club_subscription,
p.TGASSN as dd_telangana_officers_assn,p.Netpay as net_amount,p.Deductions as deductions_amount, 0 as active,1000 as trans_id,p.SBASICDA as spl_da,p.SBASICALW as spl_allw, 'Regular' as spl_type, 1 as final_process  
into #pr_emp_payslipold
from [TEST2].dbo.POldslip p join [TEST2].dbo.pempmast m on p.EID = m.EID  where m.Empid = @employeecode and p.Pmonth=@pmonth;



--select * from #pr_emp_payslip;

--select * from pr_emp_payslip


insert into pr_emp_payslip(id,gen_date,fy,
fm,emp_id,emp_code,
designation,branch,working_days,lop,er_basic,er_da,er_cca,er_hra,er_interim_relief,er_telangana_inc,
gross_amount,dd_provident_fund,dd_income_tax,dd_prof_tax,dd_club_subscription,dd_telangana_officers_assn,
net_amount,deductions_amount,active,trans_id,spl_da,spl_allw,spl_type,final_process) select * from #pr_emp_payslipold;


declare @lastnum1 int;
set @lastnum1 = (select top 1 id from pr_emp_payslip order by id desc)
update new_num set last_num=@lastnum1 where table_name='pr_emp_payslip';

--get good will data and keep in temp table for that employee
Select p.* 
into #gw_data1 
from [TEST2].dbo.POldslip p join [TEST2].dbo.pempmast m on p.EID = m.EID  where m.Empid = @employeecode and p.Pmonth=@pmonth;

CREATE  TABLE #codes_master1 (
id INT, 
name nvarchar(100), 
code nvarchar(100),
type nvarchar(100)
);

--get codes
insert into #codes_master1 select id, name,code,type from pr_earn_field_master where code!='' union all 
select id, name,code,type from pr_allowance_field_master where code!='' ;

declare @ID1 int
declare @name1 nvarchar(100)
declare @code1 nvarchar(50)
declare @val1 nvarchar(50)
declare @type1 nvarchar(50)
declare @new_id1 int;

declare @new_num1 int;
set @new_num1=(select last_num from new_num where table_name = 'pr_emp_payslip_allowance');

set @new_id1 = @new_num1;
--loop

while exists (select * from #codes_master1)
begin
	set @new_id1 = @new_id1+1;
    select @ID1 = (select top 1 id from #codes_master1 order by id asc);

	select @name1 = name, @code1 = code,@type1= type from #codes_master1 where id = @ID1;
	--get value from gw table
	set @query = 'Select @val1 = ' + @code1 + ' from #gw_data1'
	execute sp_executesql 
    @query, 
    N'@val1 nvarchar(50) OUTPUT', 
    @val1 = @val1 output;

INSERT into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,
	all_amount,all_type,active,trans_id) 
VALUES( @new_id1,(select id from employees where empid =  @employeecode  ), @employeecode ,
(select id from pr_emp_payslip where fm= @fm and  emp_code = @employeecode and spl_type='Regular'), @ID1 ,
@name1,@val1 ,@type1,0,1000);
	

    delete #codes_master1 where id = @ID1 and name=@name1 and type=@type1 and code=@code1;
end
  
  --update new num
  declare @lastnuma1 int;
set @lastnuma1 = (select top 1 id from pr_emp_payslip_allowance order by id desc)
update new_num set last_num=@lastnuma1 where table_name='pr_emp_payslip_allowance';

--drop temp table

drop table #codes_master1;

drop table #pr_emp_payslipold;


--deduction data
CREATE TABLE #codes_master_allded1 (
id INT, 
name nvarchar(100), 
code nvarchar(100),
type nvarchar(100)
);


insert into #codes_master_allded1 select id, name,code,type from pr_deduction_field_master where code!=''union all 
select id,loan_description as name,loan_id as code,'Loan' as type from pr_loan_master where active=1 and loan_id!='PFL1' and loan_id!='PFL2' union all
select id,name,code,type from dm_payslip_loans;


declare @IDssss int
declare @namessss nvarchar(100)
declare @codessss nvarchar(50)
declare @valssss nvarchar(50)
declare @typessss nvarchar(50)
declare @new_idssss int
declare @queryssss nvarchar(50)
--select * from pr_emp_pay_field
declare @new_numssss int;
set @new_numsss=(select last_num from new_num where table_name = 'pr_emp_payslip_deductions');

set @new_idsss = @new_numsss;
--loop


while exists (select * from #codes_master_allded1)
begin
set @new_idsss = @new_idsss+1;
select @IDssss = (select top 1 id from #codes_master_allded1 order by id asc);

select @namessss = name, @codessss = code,@typessss= type from #codes_master_allded1 where id = @IDssss;

--get value from gw table
set @queryssss = 'Select @valssss = ' + @codessss +  ' from #gw_data1'
execute sp_executesql 
@queryssss, 
N'@valssss nvarchar(50) OUTPUT', 
@valssss = @valssss output;


INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,
	dd_amount,dd_type,active,trans_id) 
VALUES( @new_idsss,(select id from employees where empid =  @employeecode  ), @employeecode ,
(select id from pr_emp_payslip where fm= @fm and  emp_code = @employeecode and spl_type='Regular'), @IDssss ,
@namessss,@valssss ,@typessss,0,1000);

delete #codes_master_allded1 where id = @IDssss and name=@namessss and type=@typessss and code=@codessss;
end

select (select last_num+1 from new_num where table_name = 'pr_emp_payslip_deductions') as id,
(select id from Employees where empid=@employeecode) as emp_id,@employeecode as emp_code,
(select id from pr_emp_payslip where emp_code=@employeecode and fy=@fy and fm=@fm and spl_type='Regular') as payslip_mid,
0 as dd_mid,'HFC' as dd_name,
p.HFC as dd_amount,'Monthly' as dd_type,0 as active,1000 as trans_id into #hfcDataa from test2.dbo.POldslip p 
join test2.dbo.PEMPMAST m on p.Eid=m.Eid where m.empid=@employeecode and p.Pmonth=@pmonth;

insert into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) select * from #hfcDataa;

drop table #hfcDataa;


declare @lasthfcc int;
set @lasthfcc = (select top 1 id from pr_emp_payslip_deductions order by id desc)
update new_num set last_num=@lasthfcc where table_name='pr_emp_payslip_deductions';


select (select last_num+1 from new_num where table_name = 'pr_emp_payslip_deductions') as id,
(select id from Employees where empid=@employeecode) as emp_id,@employeecode as emp_code,
(select id from pr_emp_payslip where emp_code=@employeecode  and fy=@fy and fm=@fm and spl_type='Regular') as payslip_mid,
0 as dd_mid,'LIC' as dd_name,
p.LIC as dd_amount,'Monthly' as dd_type,0 as active,1000 as trans_id into #licDataa from test2.dbo.POldslip p 
join test2.dbo.PEMPMAST m on p.Eid=m.Eid where m.empid=@employeecode and p.Pmonth=@pmonth;

insert into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) select * from #licDataa;

drop table #licDataa;


declare @lastlicc int;
set @lastlicc = (select top 1 id from pr_emp_payslip_deductions order by id desc)
update new_num set last_num=@lastlicc where table_name='pr_emp_payslip_deductions';
--drop temp table
--update last num into new num
declare @lastnumssss int;
set @lastnumssss = (select top 1 id from pr_emp_payslip_deductions order by id desc)
update new_num set last_num=@lastnumssss where table_name='pr_emp_payslip_deductions';

drop table #codes_master_allded1;
drop table #gw_data1;

end
--set @tablename='[TEST2].dbo.POldslip';



--update new_num for pr_empl_allowern

end