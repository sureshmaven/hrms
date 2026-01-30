---Interface report fieldwise queries.

declare @pmonth bigint;
declare @fm nvarchar(20);
set @pmonth=202012;
set @fm='2020-12-01';
select 1 as Sno,'NON PRIORITY PERSONAL LOAN' as Field_name,sum(p.NPPL) as GoodWill,sum(pd.dd_amount)as Maven from test2.dbo.poldslip p join test2.dbo.pempmast m on p.eid=m.eid join  pr_emp_payslip_deductions pd on m.empid=pd.emp_code where p.pmonth=@pmonth and pd.dd_name='NON PRIORITY PERSONAL LOAN' and pd.payslip_mid in(select id from pr_emp_payslip where fm=@fm)
union all
select 2 as Sno,'CCS-HYD' as Field_name,sum(p.ccshyd) as GoodWill,sum(pd.dd_amount)as Maven from test2.dbo.poldslip p join test2.dbo.pempmast m on p.eid=m.eid join  pr_emp_payslip_deductions pd on m.empid=pd.emp_code where p.pmonth=@pmonth and pd.dd_name='CCS - HYD' and pd.payslip_mid in(select id from pr_emp_payslip where fm=@fm)
union all
select 3 as Sno,'VPF - Deduction' as Field_name,sum(p.VPF) as GoodWill,sum(pd.dd_amount)as Maven from test2.dbo.poldslip p join test2.dbo.pempmast m on p.eid=m.eid join  pr_emp_payslip_deductions pd on m.empid=pd.emp_code where p.pmonth=@pmonth and pd.dd_name='VPF Deduction' and pd.payslip_mid in(select id from pr_emp_payslip where fm=@fm)
union all
select 4 as Sno,'Club Subscription' as Field_name,sum(p.SUBCLUB) as GoodWill,sum(pd.dd_amount)as Maven from test2.dbo.poldslip p join test2.dbo.pempmast m on p.eid=m.eid join  pr_emp_payslip_deductions pd on m.empid=pd.emp_code where p.pmonth=@pmonth and pd.dd_name='Club Subscription' and pd.payslip_mid in(select id from pr_emp_payslip where fm=@fm)
union all
select 5 as Sno,'PF Contribution' as Field_name,sum(p.PFcont) as GoodWill,sum(sh.bank_share)as Maven from test2.dbo.poldslip p join test2.dbo.pempmast m on p.eid=m.eid join  pr_ob_share sh on m.empid=sh.emp_code where p.pmonth=@pmonth and  fm=@fm
union all
select 6 as Sno,'Provident Fund' as Field_name,sum(p.PF) as GoodWill,sum(pay.dd_provident_fund)as Maven from test2.dbo.poldslip p join test2.dbo.pempmast m on p.eid=m.eid join  pr_emp_payslip pay on m.empid=pay.emp_code where p.pmonth=@pmonth and  pay.fm=@fm
union all
select 7 as Sno,'LIC' as FieldName,(select sum(LIC)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='LIC') as Maven
union all
select 8 as Sno,'GSLI' as FieldName,(select sum(GSLI)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='GSLI') as Maven
union all
select 9 as Sno,'HFC' as FieldName,(select sum(HFC)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='HFC') as Maven
union all
select 10 as Sno,'COURT DEDUCTIONS' as FieldName,(select sum(COURTDED)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='COURT DEDUCTION') as Maven
union all
select 11 as Sno,'PERSONAL LOAN1' as FieldName,(select sum(PERNLON1)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='Personal Loan') as Maven
union all
select 12 as Sno,'Housing Loan 2A' as FieldName,(select sum(HL2A)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='Housing Loan 2A') as Maven
union all
select 13 as Sno,'Housing Loan Main' as FieldName,(select sum(HOUS1)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='Housing Loan Main') as Maven
union all
select 14 as Sno,'Housing Loan 2B-2C' as FieldName,(select sum(HL2BC)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='Housing Loan 2B-2C') as Maven
union all
select 15 as Sno,'Housing Addl.Loan - 2D' as FieldName,(select sum(HLADD)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='Housing Addl.Loan - 2D') as Maven
union all
select 16 as Sno,'PF Loan ST 1' as FieldName,(select sum(PFHT1)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='PF Loan ST 1') as Maven
union all
select 17 as Sno,'BANKS EMP ASSN TELANGANA' as FieldName,(select sum(TGASSN)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='BANKS EMP ASSN TELANGANA') as Maven
union all
select 18 as Sno,'TELANGANA EMP UNION' as FieldName,(select sum(TGUNION)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='TELANGANA EMP UNION') as Maven
union all
select 19 as Sno,'Union Club Subscription' as FieldName,(select sum(SUBUNION)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='Union Club Subscription') as Maven
union all
select 20 as Sno,'Vehicle Loan (4W)' as FieldName,(select sum(VEH4W)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='Vehicle Loan (4W)') as Maven
union all
select 21 as Sno,'Vehicle Loan (2W)' as FieldName,(select sum(VEH2W)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='Vehicle Loan (2W)') as Maven
union all
select 22 as Sno,'SC/ST Assn ST Subscription' as FieldName,(select sum(SUBST)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='SC/ST Assn ST Subscription') as Maven
union all
select 23 as Sno,'Max Pension' as Field_name,sum(p.Maxpensin) as GoodWill,sum(pension_open)as Maven from test2.dbo.poldslip p join test2.dbo.pempmast m on p.eid=m.eid join  pr_ob_share os on m.empid=os.emp_code where p.pmonth=@pmonth and  os.fm=@fm
union all
select 24 as Sno,'NPS' as Field_name,sum(p.NPS) as GoodWill,sum(pay.NPS)as Maven from test2.dbo.poldslip p join test2.dbo.pempmast m on p.eid=m.eid join  pr_emp_payslip pay on m.empid=pay.emp_code where p.pmonth=@pmonth and  pay.fm=@fm
union all
select 25 as Sno,'NPS-Contribution' as Field_name,sum(p.NPSCONTRIBUTION) as GoodWill,sum(os.NPS_bank_share)as Maven from test2.dbo.poldslip p join test2.dbo.pempmast m on p.eid=m.eid join  pr_ob_share os on m.empid=os.emp_code where p.pmonth=@pmonth and  os.fm=@fm
union all
select 26 as Sno,'Festival Advance' as FieldName,(select sum(FEST)as GoodWill from test2.dbo.poldslip where pmonth=@pmonth)as GoodWill,(select sum(dd_amount) from pr_emp_payslip_deductions where payslip_mid in(select id from  pr_emp_payslip where fm=@fm) and dd_name='Festival Advance') as Maven

