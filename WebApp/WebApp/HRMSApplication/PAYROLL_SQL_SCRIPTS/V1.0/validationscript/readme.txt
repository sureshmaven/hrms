1. For every new goodwill DB restore from their production/any backup we need to run 14.goodwill_scripts-onetime.sql under validation scripts to create few columns and tables
2. Create indexes listed under Test folder to drop and create the indexes in goodwill db//check the db credentials in runall.bat
3. point to right DB in V1.0 runall.bat
4. run cp to copy the same runall.bat  in all places
5. run step1, step2, mains, step3   r
6. run validation script and validation script values




Month wise Data -- Payslip, loan EMI's, Loan outstanding, deductions, allowances, rent,
Year wise -- tds, 

120,000
10,000
4m- 40000

66 - 
15
27 - tds
3 -loans


1- D:\PAYROLL_SQL_SCRIPTS\10years\step1\23.sp_dm_emp_master.sql
FY to 2021  -- update script in respective tables

2- 202006 should be included in PVal tables @14.goodwill_scripts_onetime.sql under validation folder




exceptions: 1. 24 rent records without FY
