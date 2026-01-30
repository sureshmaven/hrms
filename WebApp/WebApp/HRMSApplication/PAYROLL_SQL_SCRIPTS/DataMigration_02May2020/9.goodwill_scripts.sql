alter table test2.dbo.pempmast drop column id;

drop table test2.dbo.PItCal1920;
drop table test2.dbo.PItOther1920;
drop table test2.dbo.PItVal1920;

------------------------------------------------------------------------------------

alter table test2.dbo.pempmast add id int;

DECLARE @counter int;
SET @counter = 0;
UPDATE test2.dbo.pempmast SET @counter = id = @counter + 1;

select * into test2.dbo.PItCal1920 from test2.dbo.PItCal where pmonth in
('201904','201905','201906','201907','201908','201909','201910','201911','201912','202001',
'202002','202003','202004','202005')

select * into test2.dbo.PItOther1920 from test2.dbo.PItOther where finyear in
('2019-20','2020-21')

select * into test2.dbo.PItVal1920 from test2.dbo.PItVal where pmonth in
('201904','201905','201906','201907','201908','201909','201910','201911','201912','202001',
'202002','202003','202004','202005')

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