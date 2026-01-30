declare @Pmonth NVARCHAR(20);
declare @fm NVARCHAR(20);

set @Pmonth='202007';
set @fm='2020-07-01';

select	1,	(select '	Annual Increment	'),( SELECT	Sum(	INCREMENT	)	as	INCREMENT	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Annual Increment' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	2,	(select '	Special Increment	'),( SELECT	Sum(	SPLINCR	)	as	SPLINCR	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Special Increment' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	3,	(select '	Stagnation Increments	'),( SELECT	Sum(	STAGALLOW	)	as	STAGALLOW	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Stagnation Increments' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	4,	(select '	Special Pay	'),( SELECT	Sum(	SPLPAY	)	as	SPLPAY	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Special Pay' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	5,	(select '	System Administrator Allowance	'),( SELECT	Sum(	SYSADMN	)	as	SYSADMN	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='System Administrator Allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	6,	(select '	CEO Allowance	'),( SELECT	Sum(	CEOALLW	)	as	CEOALLW	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='CEO Allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	7,	(select '	Teaching Allowance	'),( SELECT	Sum(	TEACH	)	as	TEACH	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Teaching Allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	8,	(select '	Deputation Allowance	'),( SELECT	Sum(	DEPU	)	as	DEPU	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Deputation Allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	9,	(select '	Fixed Personal Allowance	'),( SELECT	Sum(	FPA	)	as	FPA	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Fixed Personal Allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	10,	(select '	FPA-HRA Allowance	'),( SELECT	Sum(	FPHRA	)	as	FPHRA	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='FPA-HRA Allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	11,	(select '	FPIIP	'),( SELECT	Sum(	FPIIP	)	as	FPIIP	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='FPIIP' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	12,	(select '	Officiating Allowance	'),( SELECT	Sum(	OFFI	)	as	OFFI	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Officiating Allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	13,	(select '	Personal Pay	'),( SELECT	Sum(	PERPAY	)	as	PERPAY	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Personal Pay' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	14,	(select '	Personal Qual Allowance	'),( SELECT	Sum(	PERQPAY	)	as	PERQPAY	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Personal Qual Allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	15,	(select '	Br Manager Allowance	'),( SELECT	Sum(	BR_MGR	)	as	BR_MGR	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='Br Manager Allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	16,	(select '	SPL Care Taker	'),( SELECT	Sum(	SP_CARETAKE	)	as	SP_CARETAKE	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Care Taker' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	17,	(select '	SPL Cashier	'),( SELECT	Sum(	SP_CASHIER	)	as	SP_CASHIER	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Cashier' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	18,	(select '	SPL Driver	'),( SELECT	Sum(	SP_DRIVER	)	as	SP_DRIVER	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Driver' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	19,	(select '	SPL Key	'),( SELECT	Sum(	SP_KEY	)	as	SP_KEY	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Key' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	20,	(select '	SPL Lift Operator	'),( SELECT	Sum(	SP_LIFT	)	as	SP_LIFT	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Lift Operator' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	21,	(select '	SPL Non Promotional	'),( SELECT	Sum(	SP_NONPROM	)	as	SP_NONPROM	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Non Promotional' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	22,	(select '	SPL Split Duty -Award staff	'),( SELECT	Sum(	SP_SD_AWARD	)	as	SP_SD_AWARD	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Split Duty -Award staff' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	23,	(select '	SPL Typist	'),( SELECT	Sum(	SP_TYPIST	)	as	SP_TYPIST	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Typist' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	24,	(select '	SPL Watchman	'),( SELECT	Sum(	SP_WATCHMAN	)	as	SP_WATCHMAN	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Watchman' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	25,	(select '	SPL Stenographer	'),( SELECT	Sum(	SP_STENO	)	as	SP_STENO	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Stenographer' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	26,	(select '	SPL Bill Collector	'),( SELECT	Sum(	SP_BILLCOLL	)	as	SP_BILLCOLL	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Bill Collector' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	27,	(select '	SPL Despatch	'),( SELECT	Sum(	SP_DESPATCH	)	as	SP_DESPATCH	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Despatch' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	28,	(select '	SPL Electrician	'),( SELECT	Sum(	SP_ELEC	)	as	SP_ELEC	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Electrician' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	29,	(select '	SPL Dafter	'),( SELECT	Sum(	SP_DAFTARI	)	as	SP_DAFTARI	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Dafter' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	30,	(select '	SPL Cash Cabin	'),( SELECT	Sum(	SP_CASHCAD	)	as	SP_CASHCAD	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Cash Cabin' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	31,	(select '	SPL Telephone Operator	'),( SELECT	Sum(	SP_TELEPHONE	)	as	SP_TELEPHONE	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Telephone Operator' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	32,	(select '	SPL Library	'),( SELECT	Sum(	SP_LIBRARY	)	as	SP_LIBRARY	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Library' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	33,	(select '	SPL Incentive	'),( SELECT	Sum(	SP_INCENTIVE	)	as	SP_INCENTIVE	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Incentive' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	34,	(select '	SPL Arrear Incentive	'),( SELECT	Sum(	SP_ARREAR	)	as	SP_ARREAR	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Incentive' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	35,	(select '	SPL Conveyance	'),( SELECT	Sum(	SP_CONVEY	)	as	SP_CONVEY	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Conveyance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	36,	(select '	SPL Split Duty - Managers	'),( SELECT	Sum(	SP_SD_MGR	)	as	SP_SD_MGR	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Split Duty - Managers' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	37,	(select '	SPL Duplicating/xerox machine	'),( SELECT	Sum(	SP_XEROX	)	as	SP_XEROX	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Duplicating/xerox machine' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	38,	(select '	SPL Record room asst allowance	'),( SELECT	Sum(	SP_RECASST	)	as	SP_RECASST	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Record room asst allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	39,	(select '	SPL Record room sub staff all	'),( SELECT	Sum(	SP_RECSUB	)	as	SP_RECSUB	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Record room sub staff all' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	40,	(select '	SPL Receptionist allowance	'),( SELECT	Sum(	SP_RECEPTION	)	as	SP_RECEPTION	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Receptionist allowance' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	41,	(select '	SPL Spl.Alw.ACSTI	'),( SELECT	Sum(	SP_ACSTI	)	as	SP_ACSTI	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Spl.Alw.ACSTI' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	42,	(select '	SPL Personal Pay	'),( SELECT	Sum(	SP_PERPAY	)	as	SP_PERPAY	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='SPL Personal Pay' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
union all
select	43,	(select '	FACULTY ALLOWANCE	'),( SELECT	Sum(	FACULTYALLW	)	as	FACULTYALLW	from test2.dbo.poldslip where pmonth>='202004' and pmonth<=@Pmonth	) as GW,	(SELECT Sum(All_amount) from pr_emp_payslip_allowance where all_name='FACULTY ALLOWANCE' and payslip_mid in(SELECT id from pr_emp_payslip where  spl_type='Regular' and fm>='2020-04-01' and fm<=@fm) ) as MS 
