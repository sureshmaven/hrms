--exec sp_dm_tdsprocress 202003,'2019-20'
--exec sp_dm_tdsprocress 201203,'2011-12'
--@pmonth int,@finyear varchar(10)
create procedure [dbo].[sp_dm_tdsprocress]  as 
declare @yr NVARCHAR(100);
declare @year NVARCHAR(100);
declare @pmonthyr NVARCHAR(100);
declare @pmonthnew NVARCHAR(100);
declare @finyearyr NVARCHAR(100);

set @yr='201104';
set @year='2012';
set @pmonthyr=SUBSTRING(@yr,0,5)+1;
set @finyearyr=CONCAT(SUBSTRING(@yr,0,5),'-',SUBSTRING(@yr,5,6),'-01');
print @yr;--201203
print @year;

print @yr;
print @pmonthyr;
print @finyearyr;
declare @id int;

update new_num set last_num=(case when (select max(id)+1 from pr_emp_tds_process) is null then 1 else (select max(id)+1 from pr_emp_tds_process) end) where table_name = 'pr_emp_tds_process'
declare @ids int;
declare @empid int;
--set @ids=1;
--set @empid=(select empid from test2.dbo.pempmast  where id=@ids )

while (@yr<=202009)
begin
set @id=(select last_num+1 from new_num where table_name='pr_emp_tds_process')

set @ids=1;

while (@ids<=(select max(id) from test2.dbo.pempmast ))
begin
print 'Test'
set @empid=(select empid from test2.dbo.pempmast  where id=@ids )


Declare @sal_basic	float; select @sal_basic	=	(TotalAmt-EncashAmt) from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='Basic'      
Declare @sal_fixed_personal_allowance	float; select @sal_fixed_personal_allowance	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='FPA'
Declare @sal_fpa_hra_allowance	float; select @sal_fpa_hra_allowance	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='FPHRA'
Declare @sal_fpiip	float; select @sal_fpiip	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='FPIIP'
Declare @sal_da	float; select @sal_da	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='DA'
Declare @sal_hra	float; select @sal_hra	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='HRA'
Declare @sal_cca	float; select @sal_cca	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='CCA'
Declare @sal_interim_relief	float; select @sal_interim_relief	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='IRLF'
Declare @sal_telangana_increment	float; select @sal_telangana_increment	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='TSINCR'
Declare @sal_spl_allow	float; select @sal_spl_allow	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='SBASICALW'
Declare @sal_spcl_da	float; select @sal_spcl_da	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='SBASICDA'
Declare @sal_pfperks	float; 
set @sal_pfperks=0;
--select @sal_pfperks	=	Amount from test2.dbo.PItOther p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyearyr and FLddesp='PFPerks'
Declare @sal_loanperks	float; 
set @sal_loanperks=0;
--select @sal_loanperks	=	Amount from test2.dbo.PItOther p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyearyr and FLddesp='LOANPerks'
Declare @sal_incentive	float; 
set @sal_incentive=0;
--select @sal_incentive	=	Amount from test2.dbo.PItOther p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyearyr and FLddesp='Incentive'

Declare @Exgratia	float; 
set @Exgratia =0;
--select @Exgratia	=	Amount from test2.dbo.PItOther p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyearyr and FLddesp='Exgratia'
Declare @Medical_Aid	float; 
set @Medical_Aid=0;
--select @Medical_Aid	=	Amount from test2.dbo.PItOther p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyearyr and FLddesp='Medical Aid'
Declare @codperks	float; 
set @codperks=0;
--select @codperks	=	Amount from test2.dbo.PItOther p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyearyr and FLddesp='codperks'
							
Declare @house_rent_allowance	float; select @house_rent_allowance	=	Hratax from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr 		
Declare @total_of_sec10	float; select @total_of_sec10	=	Sec10 from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr 		
							
Declare @standard_deductions	float; select @standard_deductions	=	StanDed from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr 		
Declare @tax_of_employement	float; select @tax_of_employement	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='Ptax'
							
							
Declare @other_income_by_the_emp	float; select @other_income_by_the_emp	=	TotalTax from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr 		
Declare @interest_on_housing	float; select @interest_on_housing	=	Sec24 from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr 		
							
Declare @annual_inc float; 	select @annual_inc	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='INCREMENT';				
							
Declare @ltc	float; 
set @ltc=0;
--select @ltc	=	Amount from test2.dbo.PItOther p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and finyear=@finyearyr and FLddesp='LTC';
							
Declare @education_cess	float; select @education_cess	=	CESS from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr 		

Declare @section_87a float;  select @section_87a	=	sec88 from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr 		
							
Declare @tax_deducted_at_source	float; 
select @tax_deducted_at_source	=	TaxPaid from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr 		
--select @tax_deducted_at_source	=	(TotalAmt-EncashAmt)  from test2.dbo.PItCal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr and FldName='Tds'							
							
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
and fy=@year and id in(select top 1 id from pr_emp_payslip where emp_code=@empid and fy =@year order by id desc));


declare @pf_year_curr float;
set @pf_year_curr=(select sum(dd_provident_fund) pf from pr_emp_payslip where emp_code = @empid 
and id in(select top 1 id from pr_emp_payslip where emp_code=@empid and fy =@year order by id desc));

declare @total_income float;
set @total_income=(@gross_total_income-@gross_total_income);

declare @tax_on_total_income float;
set @tax_on_total_income=(select totaltax from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr);

Declare @tax_payable float;
if(@education_cess!=0)
begin
select @tax_payable	= 	(@tax_on_total_income+@education_cess);
end
else
begin
select @tax_payable	=0;
end

declare @taxpaidbyemployer float;
set @taxpaidbyemployer=0;

declare @balancetax float;
if(@tax_payable!=0)
begin
set @balancetax=(@tax_payable-(@tax_deducted_at_source+@taxpaidbyemployer));
end
else
begin
set @balancetax=0;
end

declare @balmonths float;
set @balmonths=(select BalMonth from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr);

declare @tdspermonth float;
set @tdspermonth=(select FutureTDS from test2.dbo.PItVal p join test2.dbo.pempmast mas on p.eid=mas.eid where mas.empid=@empid and pmonth=@yr);


--exec sp_dm_tdsprocress 202003,2019-20

--declare @fm date;
--set @fm=(select cast(left(@yr,4) as varchar(20))+'-'+ right(@yr,2)+'-'+'01');

--declare @fy int;
--set @fy=(select year(@fm));


insert into [pr_emp_tds_process]  values (@id,@year,@finyearyr, (select id from employees where empid=@empid),@empid,'',
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
@tax_payable,@tax_deducted_at_source,@taxpaidbyemployer,@balancetax,@balmonths,@tdspermonth,0,100,1,1);
print Concat('Inserted into pr_emp_tds_process' ,@empid)
set @sal_basic	=0;
set @sal_fixed_personal_allowance	=0;
set @sal_fpa_hra_allowance	=0;
set @sal_fpiip	=0;
set @sal_da	=0;
set @sal_hra	=0;
set @sal_cca	=0;
set @sal_interim_relief	=0;
set @sal_telangana_increment	=0;
set @sal_spl_allow	=0;
set @sal_spcl_da	=0;
set @sal_pfperks	=0;
set @sal_loanperks	=0;
set @sal_incentive	=0;
	
set @Exgratia	=0;
set @Medical_Aid	=0;
set @codperks	=0;
	
set @house_rent_allowance	=0;
set @total_of_sec10	=0;
	
set @standard_deductions	=0;
set @tax_of_employement	=0;
	
	
set @other_income_by_the_emp	=0;
set @interest_on_housing	=0;
	
set @annual_inc                     =0;
	
set @ltc	=0;
	
set @education_cess	=0;
set @tax_payable	=0                        
	
set @section_87a                    =0;
	
set @tax_deducted_at_source	=0;
	
set @gross_amount                   =0;
	
set @tds_aggregate                  =0;
                                        	
	
set @balance_gross_min_sec10        =0;
                                        	
	
set @income_chargeable_bal_minus_agg=0;
                                        	
	
set @gross_total_income             =0;
                                        	
	
set @pf_year_ded                    =0;
                                        	
	
	
	
set @pf_year_curr                   =0;
                                        	

	
set @total_income                   =0;
                                        	
	
set @tax_on_total_income            =0;
set @tdspermonth =0;
set @taxpaidbyemployer =0;
set @balancetax =0;
set @balmonths =0;


set @id=@id+1;
set @ids=@ids+1;
end
update new_num set last_num=@id where  table_name='pr_emp_tds_process';
--set @pmonthyr=SUBSTRING(@yr,0,5);
set @finyearyr=CONCAT(SUBSTRING(@yr,0,5),'-',SUBSTRING(@yr,5,6),'-01');

set @yr=@yr+1;

if(SUBSTRING(@yr,5,6) = '13')
begin
set @yr=CONCAT(SUBSTRING(@yr,0,5)+1,'01')
set @year=@year+1;
end
set @finyearyr=CONCAT(SUBSTRING(@yr,0,5),'-',SUBSTRING(@yr,5,6),'-01');
set @pmonthyr=SUBSTRING(@yr,0,5)+1;

print @yr;--201303
print @finyearyr;--2013-03
print @year;

end
update new_num set last_num=@id where  table_name='pr_emp_tds_process';
update pr_emp_tds_process set active=1 where fm=(select top 1 fm from pr_emp_tds_process order by fm desc)
--delete from pr_emp_tds_process order by id
