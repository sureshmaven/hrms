-- run individual scripts

alter table test2.dbo.pempmast drop column id;

drop table test2.dbo.PItCal1920;
drop table test2.dbo.PItOther1920;
drop table test2.dbo.PItVal1920;

------------------------------------------------------------------------------------


select * into test2.dbo.PItCal1920 from test2.dbo.PItCal where pmonth in
('201904','201905','201906','201907','201908','201909','201910','201911','201912','202001',
'202002','202003','202004','202005','202006','202007','202008','202009','202010')

select * into test2.dbo.PItOther1920 from test2.dbo.PItOther where finyear in
('2019-20','2020-21')

select * into test2.dbo.PItVal1920 from test2.dbo.PItVal where pmonth in
('201904','201905','201906','201907','201908','201909','201910','201911','201912','202001',
'202002','202003','202004','202005','202006','202007','202008','202009','202010')

CREATE INDEX idx_finyear1920
ON test2.dbo.PItOther1920 (finyear);

CREATE INDEX idx_pmonth1920
ON test2.dbo.PItCal1920 (pmonth);

CREATE INDEX idx_pmonthpitval1920
ON test2.dbo.PItVal1920 (pmonth);

CREATE INDEX idx_PItCaleid1920
ON test2.dbo.PItCal1920 (eid);

CREATE INDEX idx_pitvaleid1920
ON test2.dbo.PItVal1920 (eid);

CREATE INDEX idx_PItOthereid1920
ON test2.dbo.PItOther1920 (eid);

ALTER TABLE dbo.pr_emp_tds_process DROP CONSTRAINT PK__pr_emp_t__3213E83FA4900422



 insert into pempmast(Eid	,
 Empid	,
 Empname	,
 Desgn	,
 Basic	,
 Dept	,
 Lop	,
 Sex	,
 Fname	,
 Doj	,
 Doc	,
 Expr	,
 Div	,
 Pfno	,
 Esicno	,
 Dol	,
 Dob	,
 Age	,
 Add1	,
 Add2	,
 Add3	,
 Padd1	,
 Padd2	,
 Padd3	,
 Fdob	,
 Mstatus	,
 Pfjoin	,
 Emptds	,
 LIC	,
 Region	,
 EMail	,
 Frel	,
 Hra40	,
 Branch	,
 Zone	,
 EmpShName	,
 PhoneNo	,
 PPhoneNo	,
 NativePlace	,
 IdMark1	,
 IdMark2	,
 BloodGroup	,
 Religion	,
 JoinReservation	,
 ResvCode	,
 ResvCodeJoin	,
 PtaxRegion	,
 DesgnCat	,
 SPLPAY	,
 PH	,
 SYSADMN	,
 DEPU	,
 FPA	,
 FPHRA	,
 INTERIM	,
 FPIIP	,
 MEDICAL	,
 NPSGA	,
 OFFI	,
 PERPAY	,
 PERQPAY	,
 RESATTN	,
 TEACH	,
 SP_CARETAKE	,
 SP_CASHIER	,
 SP_DRIVER	,
 SP_JAMEDAR	,
 SP_KEY	,
 SP_LIFT	,
 SP_NONPROM	,
 SP_SD_AWARD	,
 SP_TYPIST	,
 SP_WATCHMAN	,
 SP_STENO	,
 SP_BILLCOLL	,
 SP_DESPATCH	,
 SP_ELEC	,
 SP_DAFTARI	,
 SP_CASHCAB	,
 SP_TELEPHONE	,
 SP_LIBRARY	,
 SP_INCENTIVE	,
 SP_ARREAR	,
 SP_CONVEY	,
 SP_SD_MGR	,
 SP_XEROX	,
 SP_RECASST	,
 SP_RECSUB	,
 SP_RECEPTION	,
 SP_ACSTI	,
 VIJAYA	,
 VISAKHA	,
 GSLI	,
 OFFASSN	,
 OFFASSNC	,
 SUBCLUB	,
 SUBUNION	,
 SUBLT	,
 SUBST	,
 FPIR	,
 SBF	,
 VPF	,
 PANNO	,
 Regord	,
 Dor	,
 STAGALLOW	,
 SP_PERPAY	,
 relation	,
 BrRoZo	,
 TRANSDATE	,
 incdate	,
 VPFPER	,
 Paybank	,
 Emplgname	,
 Phc	,
 Houseprovid) select * from pempmastold where empid not in (select empid from pempmast);
 
 
 
alter table test2.dbo.pempmast add id int;

DECLARE @counter int;
SET @counter = 0;
UPDATE test2.dbo.pempmast SET @counter = id = @counter + 1;