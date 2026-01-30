--exec sp_dm_tdsprocress 202003,'2019-20'
create procedure [dbo].[sp_dm_tdsprocress] @pmonth int,@finyear varchar(10) as 

declare @id int;
set @id=1

while (@id<=(select max(id) from test2.dbo.pempmast))
begin

declare @empid int;
set @empid=(select empid from test2.dbo.pempmast  where id=@id )


Declare @sal_basic	float; select @sal_basic	=	(TotalAmt-EncashAmt) from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='Basic'      
Declare @sal_fixed_personal_allowance	float; select @sal_fixed_personal_allowance	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='FPA'
Declare @sal_fpa_hra_allowance	float; select @sal_fpa_hra_allowance	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='FPHRA'
Declare @sal_fpiip	float; select @sal_fpiip	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='FPIIP'
Declare @sal_da	float; select @sal_da	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='DA'
Declare @sal_hra	float; select @sal_hra	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='HRA'
Declare @sal_cca	float; select @sal_cca	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='CCA'
Declare @sal_interim_relief	float; select @sal_interim_relief	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='IRLF'
Declare @sal_telangana_increment	float; select @sal_telangana_increment	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='TSINCR'
Declare @sal_spl_allow	float; select @sal_spl_allow	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='SBASICALW'
Declare @sal_spcl_da	float; select @sal_spcl_da	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='SBASICDA'
Declare @sal_pfperks	float; select @sal_pfperks	=	Amount from test2.dbo.PItOther1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyear and FLddesp='PFPerks'
Declare @sal_loanperks	float; select @sal_loanperks	=	Amount from test2.dbo.PItOther1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyear and FLddesp='LOANPerks'
Declare @sal_incentive	float; select @sal_incentive	=	Amount from test2.dbo.PItOther1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyear and FLddesp='Incentive'

Declare @Exgratia	float; select @Exgratia	=	Amount from test2.dbo.PItOther1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyear and FLddesp='Exgratia'
Declare @Medical_Aid	float; select @Medical_Aid	=	Amount from test2.dbo.PItOther1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyear and FLddesp='Medical Aid'
Declare @codperks	float; select @codperks	=	Amount from test2.dbo.PItOther1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyear and FLddesp='codperks'
							
Declare @house_rent_allowance	float; select @house_rent_allowance	=	Hratax from test2.dbo.PItVal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth 		
Declare @total_of_sec10	float; select @total_of_sec10	=	Sec10 from test2.dbo.PItVal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth 		
							
Declare @standard_deductions	float; select @standard_deductions	=	StanDed from test2.dbo.PItVal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth 		
Declare @tax_of_employement	float; select @tax_of_employement	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='Ptax'
							
							
Declare @other_income_by_the_emp	float; select @other_income_by_the_emp	=	TotalTax from test2.dbo.PItVal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth 		
Declare @interest_on_housing	float; select @interest_on_housing	=	Sec24 from test2.dbo.PItVal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth 		
							
Declare @annual_inc float; 	select @annual_inc	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth and FldName='INCREMENT';				
							
Declare @ltc	float; select @ltc	=	Amount from test2.dbo.PItOther1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyear and FLddesp='LTC';
							
Declare @education_cess	float; select @education_cess	=	CESS from test2.dbo.PItVal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth 		
Declare @tax_payable	float; select @tax_payable	=	TaxPaid from test2.dbo.PItVal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth 		

Declare @section_87a float;  select @section_87a	=	sec88 from test2.dbo.PItVal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth 		
							
declare @gross_amount float;
set @gross_amount=(@sal_basic	+
@sal_fixed_personal_allowance	+
@sal_fpa_hra_allowance	+
@sal_fpiip	+
@sal_da	+
@sal_hra	+
@sal_cca	+
@sal_interim_relief	+
@sal_telangana_increment	+
@sal_spl_allow	+
@sal_spcl_da	+
@sal_pfperks	+
@sal_loanperks	+
@sal_incentive )

declare @tds_aggregate float;
set @tds_aggregate=(@standard_deductions+@tax_of_employement);

declare @balance_gross_min_sec10 float;
set @balance_gross_min_sec10=(@gross_amount-@total_of_sec10);

declare @income_chargeable_bal_minus_agg float;
set @income_chargeable_bal_minus_agg=(@balance_gross_min_sec10-@tds_aggregate);

declare @gross_total_income float;
set @gross_total_income=(@balance_gross_min_sec10-@tds_aggregate);

declare @pf_year_ded float;
set   @pf_year_ded= ( select sum(dd_provident_fund) as amount from pr_emp_payslip where emp_code = @empid 
and fy=2020 and id in(select top 1 id from pr_emp_payslip where emp_code=@empid and fy =2020 order by id desc));


declare @pf_year_curr float;
set @pf_year_curr=(select sum(dd_provident_fund) pf from pr_emp_payslip where emp_code = @empid and fm<'2020-06-01' and fm>='2020-04-01');

declare @total_income float;
set @total_income=(@gross_total_income-@gross_total_income);

declare @tax_on_total_income float;
set @tax_on_total_income=(select totaltax from test2.dbo.PItVal1920 p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@pmonth);


--exec sp_dm_tdsprocress 202003,2019-20

declare @fm date;
set @fm=(select cast(left(@pmonth,4) as varchar(20))+'-'+ right(@pmonth,2)+'-'+'01');

declare @fy int;
set @fy=(select year(@fm));


insert into [pr_emp_tds_process]  values (1,@fy,@fm, (select id from employees where empid=@empid),@empid,'',
@sal_basic	,
@sal_fixed_personal_allowance	,
@sal_fpa_hra_allowance	,
@sal_fpiip	,
@sal_da	,
@sal_hra	,
@sal_cca	,
@sal_interim_relief	,
@sal_telangana_increment	,
@sal_spl_allow	,
@sal_spcl_da	,
@sal_pfperks	,
@sal_loanperks	,
@sal_incentive,
0,
0,
@gross_amount,		
@house_rent_allowance	,
@total_of_sec10	,
@balance_gross_min_sec10,
@standard_deductions	,
@tax_of_employement	,
@tds_aggregate,
@income_chargeable_bal_minus_agg,
@other_income_by_the_emp	,
@interest_on_housing	,
@gross_total_income,
((@pf_year_ded*12)+@pf_year_curr+(case when @interest_on_housing>=200000 then 200000 else @interest_on_housing end)),
@total_income,
@tax_on_total_income,
@section_87a,
@education_cess	,
@tax_payable,0,00,(@tax_payable),0,0,1,100,1,1);

set @id=@id+1
end


update new_num set last_num=@id where  table_name='pr_emp_tds_process';



--delete from pr_emp_tds_process order by id







