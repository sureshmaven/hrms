declare @startmonth date;
set @startmonth='2010-04-01';
declare @startmonth_format int;
set @startmonth_format=(select convert(varchar(6),@startmonth,112))
declare @endmonth date;
set @endmonth='2020-09-01';
declare @endmonth_format int;
set @endmonth_format=(select convert(varchar(6),@endmonth,112))
exec sp_dm_payslip_emp_data @startmonth_format,1
declare @startmn date;
set @startmn=(select DATEADD(mm, 1, @startmonth))
while ((@startmn)<@endmonth)
begin
declare @startmonth_format1 int;
set @startmonth_format1=(select convert(varchar(6),@startmn,112))
exec sp_dm_payslip_emp_data @startmonth_format1,2
set @startmn=(select DATEADD(mm, 1, @startmn))
end
exec sp_dm_payslip_emp_data @endmonth_format,3


declare @lastmonth int;
set @lastmonth=202010;

exec sp_dm_pfnonrepayable

exec pr_emp_repayable

exec sp_dm_ob_share_emp_data 201004

exec GetLoansEmpData @lastmonth,'FEST'
exec GetLoansEmpData @lastmonth,'HL2A'
exec GetLoansEmpData @lastmonth,'HL2BC'
exec GetLoansEmpData @lastmonth,'HLADD'
exec GetLoansEmpData @lastmonth,'HOUS1'
exec GetLoansEmpData @lastmonth,'PERS'
exec GetLoansEmpData @lastmonth,'PFHT1'
exec GetLoansEmpData @lastmonth,'PFHT2'
exec GetLoansEmpData @lastmonth,'PFLT2'
exec GetLoansEmpData @lastmonth,'PFLT3'
exec GetLoansEmpData @lastmonth,'PFLT4'
exec GetLoansEmpData @lastmonth,'VEH2W'
exec GetLoansEmpData @lastmonth,'VEH4W'
---emps not in poldloan
exec sp_dm_loan_single_two_wheller_fornodataemployees 239,@lastmonth
exec sp_dm_sub_loans_hl2a_fornodataemployees 443,@lastmonth
EXEC sp_dm_sub_loans_hl2a_fornodataemployees 360,@lastmonth
exec sp_dm_sub_loans_hl2bc_fornodataemployees 177,@lastmonth
exec sp_dm_sub_loans_hl2bc_fornodataemployees 5750,@lastmonth

EXEC sp_dm_sub_loans_fornodataemployees 360,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	177,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	332,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	371,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	438,@lastmonth

--exec sp_dm_sub_loans_fornodataemployees	452,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	5768,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	5780,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	5794,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	5888,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	5892,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	5908,@lastmonth
exec sp_dm_sub_loans_fornodataemployees	6177,@lastmonth

exec sp_dm_house_plot 425,@lastmonth

---emps not in poldloan

update pr_emp_adv_loans set active=0  where emp_Code=239 and loan_type_mid=24 and sanction_date='2008-01-31';

exec sp_dm_emp_master '2020-10-01',2021;

exec increments '2016-02-01'

exec sp_dm_tdsprocress --201203,'2011-12'

exec sp_dm_tdsprocess_earn_ded 

exec sp_dm_pf_enash_adhoc

exec sp_dm_other_tds

--exec sp_dm_per_ded

exec sp_dm_rent @lastmonth
exec sp_dm_rent 201904
exec sp_dm_rent 201804
exec sp_dm_rent 201704
exec sp_dm_rent 201604
exec sp_dm_rent 201504
exec sp_dm_rent 201404
exec sp_dm_rent 201304
exec sp_dm_rent 200404
exec sp_dm_rent 201104
--exec sp_dm_rent 201004
exec sp_pr_tds_section_deductions
exec sp_pr_section_deductions
exec sp_pr_emp_perearning_ded

Select distinct Empid into #EmpTable from Employees order by Empid asc
declare @empid int;
declare @fy int;
declare @transid int;
set @transid=1;
while exists (select * from #EmpTable)
begin
select top 1 @empid=Empid from #EmpTable order by Empid asc
select @fy=(select fy from pr_month_details where active=1)

if not exists(select Empid from pr_tax_option_emp_wise where Empid=@empid)
begin
Insert into pr_tax_option_emp_wise(EmpId,fy,[Option],trans_id) values(@empid,@fy,1,@transid)
end
else
begin
update pr_tax_option_emp_wise set EmpId=@empid,fy=@fy,[Option]=1,trans_id=@transid where EmpId=@empid;
end

delete #EmpTable where Empid = @empid
set @transid=@transid+1;
End
drop table #EmpTable

exec loans_olddata 'FEST'
exec loans_olddata 'HLADD'
exec loans_olddata 'HLPLT'
exec loans_olddata 'HOUS1'
exec loans_olddata 'HL2BC'
exec loans_olddata 'HL2A'
exec loans_olddata 'PFHT1'
exec loans_olddata 'PFHT2'
exec loans_olddata 'PFLT1'
exec loans_olddata 'PFLT2'
exec loans_olddata 'PFLT3'
exec loans_olddata 'PFLT4'
exec loans_olddata 'VEH2W'
exec loans_olddata 'VEH4W'
exec loans_olddata 'PERS'








