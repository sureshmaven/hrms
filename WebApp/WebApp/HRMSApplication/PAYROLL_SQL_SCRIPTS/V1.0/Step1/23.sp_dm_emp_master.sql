
create procedure [dbo].[sp_dm_emp_master](@fm varchar(20),@fy int)
as 
begin
-- select Empid into #general_codes 
-- from [TEST2].dbo.PEMPMAST where Dor>GETDATE();

select distinct(emp_code) as Empid into #general_codes 
from dm_emp_mn_task;

declare @employeecode int;
declare @new_id_lic int;
declare @new_id_hfc int;

declare @new_num_lic int;
set @new_num_lic=(select last_num from new_num where table_name = 'pr_emp_lic_details');

set @new_id_lic = @new_num_lic;
set @new_id_lic = @new_id_lic+1;

declare @new_num_hfc int;
set @new_num_hfc=(select last_num from new_num where table_name = 'pr_emp_hfc_details');

set @new_id_hfc = @new_num_hfc;
set @new_id_hfc = @new_id_hfc+1;


while exists (select * from #general_codes)
begin
	--set @new_id_lic = @new_id_lic+1;
	--set @new_id_hfc = @new_id_hfc+1;

    select @employeecode = (select top 1 Empid from #general_codes order by Empid asc);
	print(@employeecode);

--genral insertion data
select (select id from employees where empid = @employeecode) as emp_id,
@fy as fy,@fm as fm,Empid as emp_code,
(select Gender from Employees where EmpId= @employeecode) as sex,
(select MartialStatus from Employees where EmpId=@employeecode) martial_status,'HYDERABAD' as zone,
(select d.Name from  Employees e join Designations d on e.CurrentDesignation = d.Id where e.EmpId= @employeecode) as designation,
DesgnCat as designation_category,
'HYDERABAD' as region_for_p_tax,'HYDERABAD' as p_tax_region,
(select PresentAddress from Employees where EmpId= @employeecode) as address,
(select PermanentAddress from Employees where EmpId= @employeecode) as per_address,
(select MobileNumber from Employees where EmpId= @employeecode) as per_phoneno ,
NativePlace as native_place,Div as division,
Pfno as pf_no,Esicno as uan_no,
Pfjoin as doj_pf,(select PersonalEmailId from Employees where EmpId= @employeecode) as email_id,
IdMark1 as identify_mark1,IdMark2 as identify_mark2,
(select BloodGroup from Employees where EmpId= @employeecode) as blood_group,
Religion as religion,ResvCode as cur_reservation,
JoinReservation as join_reservation,(select PanCardNo from Employees where EmpId= @employeecode) as pan_no,
BrCode as branch_code,
null as pay_bank,Accode as account_code,
BankAcNo as bank_accno,Custid as customer_id,
case when Phc is null then '0.00' else Phc end  as phy_handicapped, case when Houseprovid is null then '0.00' else  Houseprovid end as house_provided,
DATEDIFF(year, Dob, getdate()) as emp_age,
DesgNO as designation_no,STLTEMP as stl_temp,
FESTADV as fest_adv,ARTREMP as artr_emp,(select AadharCardNo from Employees where EmpId= @employeecode) as aadhaar_no,
Dob as dob,Expr as exp, 1 as active, 1000 as trans_id
into #pr_emp_general_migration
from [TEST2].dbo.PEMPMAST where  Empid = @employeecode;

INSERT INTO pr_emp_general(emp_id, fy, fm,
emp_code,sex,martial_status,
zone,designation,designation_category,
region_for_p_tax,p_tax_region,address,
per_address,per_phoneno,native_place,
division,pf_no,uan_no,doj_pf,
email_id,identify_mark1,identify_mark2,
blood_group,religion,cur_reservation,
join_reservation,pan_no,branch_code,
pay_bank,account_code,bank_accno,
customer_id,phy_handicapped,house_provided,
emp_age,designation_no,stl_temp,
fest_adv,artr_emp,aadhaar_no,
dob,exp,active,
trans_id) select * from #pr_emp_general_migration; 
drop table #pr_emp_general_migration;

--father_husband_name,[f/h_relation]

--Biological data
select Empid as emp_code,(select id from employees where empid = @employeecode) as emp_id,
@fy as fy, @fm as fm,(select FatherName from employees where empid = @employeecode)as father_husband_name,'Father' as  [f/h_relation],1 as active, 1000 as trans_id, format(incdate,'yyyy-MM-dd') as revision_of_date_change 
into #pr_emp_biological_migration from [TEST2].dbo.PEMPMAST where  Empid = @employeecode; 

INSERT INTO pr_emp_biological_field (emp_code,emp_id,fy,fm,father_husband_name,[f/h_relation],active,trans_id,revision_of_date_change) select * from #pr_emp_biological_migration; 
drop table #pr_emp_biological_migration;

--payfields data

CREATE  TABLE #codes_master (
id INT, 
name nvarchar(100), 
emp_master_code nvarchar(100),
type nvarchar(100)
);

insert into #codes_master select id, name,emp_master_code,type from pr_earn_field_master where emp_master_code!='' and emp_master_code not in ('ANNUALALLOW','CAIIB','JAIIB','DA','CCA', 'SP_SHFTDUTY', 'SBASICDA')

Select p.* 
into #gw_data 
from [TEST2].dbo.pempmast p where p.Empid = @employeecode;

declare @ID int
declare @name nvarchar(100)
declare @code nvarchar(50)
declare @val nvarchar(50)
declare @type nvarchar(50)
declare @new_id int
declare @query nvarchar(50)
--select * from pr_emp_pay_field
declare @new_num int;
set @new_num=(select last_num from new_num where table_name = 'pr_emp_pay_field');

set @new_id = @new_num;
--loop


while exists (select * from #codes_master)
begin
	set @new_id = @new_id+1;
    select @ID = (select top 1 id from #codes_master order by id asc);

	select @name = name, @code = emp_master_code,@type= type from #codes_master where id = @ID;

	--get value from gw table
	set @query = 'Select @val = ' + @code + ' from #gw_data'
	execute sp_executesql 
    @query, 
    N'@val nvarchar(50) OUTPUT', 
    @val = @val output;
	
	INSERT into pr_emp_pay_field(id,emp_id,emp_code,fy,fm,m_id,m_type,
	amount,active,trans_id) 
VALUES( @new_id,(select id from employees where empid =  @employeecode  ), @employeecode ,@fy,@fm,
(select id from pr_earn_field_master where name=@name and emp_master_code=@code and type=@type),@type, @val ,1,1000);
	
    delete #codes_master where id = @ID and name=@name and type=@type and emp_master_code=@code;
end
  
--drop temp table
--update last num into new num
declare @lastnum int;
set @lastnum = (select top 1 id from pr_emp_pay_field order by id desc)
update new_num set last_num=@lastnum where table_name='pr_emp_pay_field';

drop table #codes_master;
drop table #gw_data;

--allowance data

CREATE TABLE #codes_master_all (
id INT, 
name nvarchar(100), 
emp_master_code nvarchar(100),
type nvarchar(100)
);

insert into #codes_master_all select id, name,emp_master_code,type from pr_allowance_field_master where emp_master_code!='' and type='EMPA' and  emp_master_code not in ('CHILDLOAN')

Select p.* 
into #gw_data_all 
from [TEST2].dbo.pempmast p where p.Empid = @employeecode;

declare @IDs int
declare @names nvarchar(100)
declare @codes nvarchar(50)
declare @vals nvarchar(50)
declare @types nvarchar(50)
declare @new_ids int
declare @querys nvarchar(50)
--select * from pr_emp_pay_field
declare @new_nums int;
set @new_nums=(select last_num from new_num where table_name = 'pr_emp_allowances_gen');

set @new_ids = @new_nums;
--loop


while exists (select * from #codes_master_all)
begin
set @new_ids = @new_ids+1;
select @IDs = (select top 1 id from #codes_master_all order by id asc);

select @names = name, @codes = emp_master_code,@types= type from #codes_master_all where id = @IDs;

--get value from gw table
set @querys = 'Select @vals = ' + @codes +  ' from #gw_data_all'
execute sp_executesql 
@querys, 
N'@vals nvarchar(50) OUTPUT', 
@vals = @vals output;

INSERT into pr_emp_allowances_gen(id,emp_id,emp_code,fy,fm,m_id,m_type,
amount,active,trans_id) 
VALUES( @new_ids,(select id from employees where empid = @employeecode ), @employeecode ,@fy,@fm,
(select id from pr_allowance_field_master where name=@names and emp_master_code=@codes and type=@types),@types, @vals ,1,1000);



delete #codes_master_all where id = @IDs and name=@names and type=@types and emp_master_code=@codes;
end

--drop temp table
--update last num into new num
declare @lastnums int;
set @lastnums = (select top 1 id from pr_emp_allowances_gen order by id desc)
update new_num set last_num=@lastnums where table_name='pr_emp_allowances_gen';

drop table #codes_master_all;
drop table #gw_data_all;

--special allowance data
CREATE TABLE #codes_master_allspl (
id INT, 
name nvarchar(100), 
emp_master_code nvarchar(100),
type nvarchar(100)
);

insert into #codes_master_allspl select id, name,emp_master_code,type from pr_allowance_field_master where emp_master_code!='' and  type='EMPSA'

Select p.* 
into #gw_data_allspl 
from [TEST2].dbo.pempmast p where p.Empid = @employeecode;

declare @IDss int
declare @namess nvarchar(100)
declare @codess nvarchar(50)
declare @valss nvarchar(50)
declare @typess nvarchar(50)
declare @new_idss int
declare @queryss nvarchar(50)
--select * from pr_emp_pay_field
declare @new_numss int;
set @new_numss=(select last_num from new_num where table_name = 'pr_emp_allowances_spl');

set @new_idss = @new_numss;
--loop


while exists (select * from #codes_master_allspl)
begin
set @new_idss = @new_idss+1;
select @IDss = (select top 1 id from #codes_master_allspl order by id asc);

select @namess = name, @codess = emp_master_code,@typess= type from #codes_master_allspl where id = @IDss;

--get value from gw table
set @queryss = 'Select @valss = ' + @codess +  ' from #gw_data_allspl'
execute sp_executesql 
@queryss, 
N'@valss nvarchar(50) OUTPUT', 
@valss = @valss output;

INSERT into pr_emp_allowances_spl(id,emp_id,emp_code,fy,fm,m_id,m_type,
amount,active,trans_id) 
VALUES( @new_idss,(select id from employees where empid = @employeecode ), @employeecode ,@fy,@fm,
(select id from pr_allowance_field_master where name=@namess and emp_master_code=@codess and type=@typess),@typess, @valss ,1,1000);



delete #codes_master_allspl where id = @IDss and name=@namess and type=@typess and emp_master_code=@codess;
end

--drop temp table
--update last num into new num
declare @lastnumss int;
set @lastnumss = (select top 1 id from pr_emp_allowances_spl order by id desc)
update new_num set last_num=@lastnumss where table_name='pr_emp_allowances_spl';

drop table #codes_master_allspl;
drop table #gw_data_allspl;
--deduction data
CREATE TABLE #codes_master_allded (
id INT, 
name nvarchar(100), 
emp_master_code nvarchar(100),
type nvarchar(100)
);

insert into #codes_master_allded select id, name,emp_master_code,type from pr_deduction_field_master where emp_master_code!='' and  type = 'EPD' and emp_master_code not in('ANDHBANK')
--not in('ANDHBANK','NPPL')


Select p.* 
into #gw_data_allded 
from [TEST2].dbo.pempmast p where p.Empid = @employeecode;

declare @IDsss int
declare @namesss nvarchar(100)
declare @codesss nvarchar(50)
declare @valsss nvarchar(50)
declare @typesss nvarchar(50)
declare @new_idsss int
declare @querysss nvarchar(50)
--select * from pr_emp_pay_field
declare @new_numsss int;
set @new_numsss=(select last_num from new_num where table_name = 'pr_emp_deductions');

set @new_idsss = @new_numsss;
--loop


while exists (select * from #codes_master_allded)
begin
set @new_idsss = @new_idsss+1;
select @IDsss = (select top 1 id from #codes_master_allded order by id asc);

select @namesss = name, @codesss = emp_master_code,@typesss= type from #codes_master_allded where id = @IDsss;

--get value from gw table
set @querysss = 'Select @valsss = ' + @codesss +  ' from #gw_data_allded'
execute sp_executesql 
@querysss, 
N'@valsss nvarchar(50) OUTPUT', 
@valsss = @valsss output;

INSERT into pr_emp_deductions(id,emp_id,emp_code,fy,fm,m_id,m_type,
amount,active,trans_id) 
VALUES( @new_idsss,(select id from employees where empid = @employeecode ), @employeecode ,@fy,@fm,
(select id from pr_deduction_field_master where name=@namesss and emp_master_code=@codesss and type=@typesss),@typesss, @valsss ,1,1000);



delete #codes_master_allded where id = @IDsss and name=@namesss and type=@typesss and emp_master_code=@codesss;
end

--drop temp table
--update last num into new num
declare @lastnumsss int;
set @lastnumsss = (select top 1 id from pr_emp_deductions order by id desc)
update new_num set last_num=@lastnumsss where table_name='pr_emp_deductions';

drop table #codes_master_allded;
drop table #gw_data_allded;

--lic data
declare @accountno NVARCHAR(30);
select p.PrmDesc as accno into #AccountNumber from [TEST2].dbo.PEmpOthDet p join [TEST2].dbo.pempmast m on p.EID = m.EID where  p.PrmType='LIC' and m.Empid=@employeecode;

print @accountno;
while exists (select * from #AccountNumber)
begin
set @accountno=(select top 1 accno from #AccountNumber)
select @new_id_lic as id,@fy as fy,@fm as fm,(select id from employees where empid = @employeecode) as emp_id,@employeecode as emp_code,p.PrmDesc as account_no,p.Amount as amount,' Monthly' as pay_type, p.MonthId as pay_months, 'No' as stop,p.StopMnth as stop_month,1 as active,1000 as trans_id 
into #pr_emp_lic_details_migration from [TEST2].dbo.PEmpOthDet p join [TEST2].dbo.pempmast m on p.EID = m.EID where  p.PrmType='LIC' and m.Empid=@employeecode and p.PrmDesc=@accountno;

INSERT INTO pr_emp_lic_details ([id],[fy],[fm],[emp_id],[emp_code],[account_no],[amount],[pay_type],[pay_months],[stop],[stop_month],[active],[trans_id]) select * from #pr_emp_lic_details_migration;
drop table #pr_emp_lic_details_migration;
delete from #AccountNumber where accno=@accountno;
set @new_id_lic = @new_id_lic+1;
print @new_id_lic;

end
drop table #AccountNumber;
-- hfc data
declare @accountno1 NVARCHAR(30);
select p.PrmDesc as accno into #AccountNumber1 from [TEST2].dbo.PEmpOthDet p join [TEST2].dbo.pempmast m on p.EID = m.EID where  p.PrmType='HFC' and m.Empid=@employeecode
while exists(select * from #AccountNumber1)
begin
set @accountno1=(select top 1 accno from #AccountNumber1)
select @new_id_hfc as id,@fy as fy,@fm as fm,(select id from employees where empid = @employeecode) as emp_id,@employeecode as emp_code,p.PrmDesc as account_no,p.Amount as amount,' Monthly' as pay_type, p.MonthId as pay_months,'No' as stop,p.StopMnth as stop_month,1 as active,1000 as trans_id 
into #pr_emp_hfc_details_migration from [TEST2].dbo.PEmpOthDet p join [TEST2].dbo.pempmast m on p.EID = m.EID where  p.PrmType='HFC' and m.Empid=@employeecode and p.PrmDesc=@accountno1;

INSERT INTO pr_emp_hfc_details ([id],[fy],[fm],[emp_id],[emp_code],[account_no],[amount],[pay_type],[pay_months],[stop],[stop_month],[active],[trans_id]) select * from #pr_emp_hfc_details_migration;
drop table #pr_emp_hfc_details_migration;
delete from #AccountNumber1 where accno=@accountno1;
set @new_id_hfc = @new_id_hfc+1;

end
drop table #AccountNumber1;

--code deletion from codes
delete from #general_codes where Empid= @employeecode;

end
declare @lastnum_lic int;
set @lastnum_lic = (select top 1 id from pr_emp_pay_field order by id desc)
update new_num set last_num=@lastnum_lic where table_name='pr_emp_lic_details';

declare @lastnum_hfc int;
set @lastnum_hfc = (select top 1 id from pr_emp_pay_field order by id desc)
update new_num set last_num=@lastnum_hfc where table_name='pr_emp_hfc_details';

drop table #general_codes;


end

--exec sp_dm_emp_master '2019-09-01',2020;
--exec sp_emp_payslip_payfields_migration_all_employess '2019-05-01',2020,'201905';
--exec sp_emp_payslip_payfields_migration_all_employess '2019-06-01',2020,'201906';

