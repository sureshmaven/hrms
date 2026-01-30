exec sp_dm_payslip_emp_data 201904,1
exec sp_dm_payslip_emp_data 201905,2
exec sp_dm_payslip_emp_data 201906,2
exec sp_dm_payslip_emp_data 201907,2
exec sp_dm_payslip_emp_data 201908,2
exec sp_dm_payslip_emp_data 201909,2
exec sp_dm_payslip_emp_data 201910,2
exec sp_dm_payslip_emp_data 201911,2
exec sp_dm_payslip_emp_data 201912,2
exec sp_dm_payslip_emp_data 202001,2
exec sp_dm_payslip_emp_data 202002,2
exec sp_dm_payslip_emp_data 202003,2
exec sp_dm_payslip_emp_data 202004,2
exec sp_dm_payslip_emp_data 202005,2
exec sp_dm_payslip_emp_data 202006,2
exec sp_dm_payslip_emp_data 202007,2
exec sp_dm_payslip_emp_data 202008,2
exec sp_dm_payslip_emp_data 202009,3

exec sp_dm_pfnonrepayable 202010

exec pr_emp_repayable 202010

exec sp_dm_ob_share_emp_data 201904

exec GetLoansEmpData 202010,'FEST'
exec GetLoansEmpData 202010,'HL2A'
exec GetLoansEmpData 202010,'HL2BC'
exec GetLoansEmpData 202010,'HLADD'
exec GetLoansEmpData 202010,'HOUS1'
exec GetLoansEmpData 202010,'PERS'
exec GetLoansEmpData 202010,'PFHT1'
exec GetLoansEmpData 202010,'PFHT2'
exec GetLoansEmpData 202010,'PFLT2'
exec GetLoansEmpData 202010,'PFLT3'
exec GetLoansEmpData 202010,'PFLT4'
exec GetLoansEmpData 202010,'VEH2W'
exec GetLoansEmpData 202010,'VEH4W'
insert into dm_emp_mn_task values (354,202010,'HL2BC,');
---emps not in poldloan
exec sp_dm_loan_single_two_wheller_fornodataemployees 239,202010
exec sp_dm_sub_loans_hl2a_fornodataemployees 443,202010
EXEC sp_dm_sub_loans_hl2a_fornodataemployees 360,202010
exec sp_dm_sub_loans_hl2bc_fornodataemployees 177,202010
exec sp_dm_sub_loans_hl2bc_fornodataemployees 5750,202010

EXEC sp_dm_sub_loans_fornodataemployees 360,202010
exec sp_dm_sub_loans_fornodataemployees	177,202010
exec sp_dm_sub_loans_fornodataemployees	332,202010
exec sp_dm_sub_loans_fornodataemployees	371,202010
exec sp_dm_sub_loans_fornodataemployees	438,202010



--exec sp_dm_sub_loans_fornodataemployees	452,202010
exec sp_dm_sub_loans_fornodataemployees	5768,202010
exec sp_dm_sub_loans_fornodataemployees	5780,202010
exec sp_dm_sub_loans_fornodataemployees	5794,202010
exec sp_dm_sub_loans_fornodataemployees	5888,202010
exec sp_dm_sub_loans_fornodataemployees	5892,202010
exec sp_dm_sub_loans_fornodataemployees	5908,202010
exec sp_dm_sub_loans_fornodataemployees	6177,202010

exec sp_dm_house_plot 425,202010

---emps not in poldloan

update pr_emp_adv_loans set active=0  where emp_Code=239 and loan_type_mid=24 and sanction_date='2008-01-31';

exec sp_dm_emp_master '2020-10-01',2021;

exec increments '2016-02-01'

exec sp_dm_tdsprocress 202009

exec sp_dm_tdsprocess_earn_ded 

exec sp_dm_pf_enash_adhoc

exec sp_dm_other_tds

--exec sp_dm_per_ded '2020-21'

exec sp_pr_emp_perearning_ded

exec sp_pr_tds_section_deductions

exec sp_pr_section_deductions

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