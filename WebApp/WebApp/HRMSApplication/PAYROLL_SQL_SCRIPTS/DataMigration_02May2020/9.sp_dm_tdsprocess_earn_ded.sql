create procedure sp_dm_tdsprocess_earn_ded as

--pitcal
insert into pr_emp_perearning 
select row_number() OVER(ORDER BY m.empid ASC),2020,'2020-03-01',
e.id,m.empid,mas.id,'per_earn',(p.TotalAmt-p.EncashAmt),null
,0,1000 from test2.dbo.pitcal p join test2.dbo.pempmast m on p.eid=m.eid 
join pr_earn_field_master mas on mas.name=p.fldname   
COLLATE SQL_Latin1_General_CP1_CI_AS and type='per_earn'
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where pmonth='202003' and p.fldname   not in ('Basic',
'FPA',
'FPHRA',
'FPIIP',
'DA',
'HRA',
'CCA',
'IRLF',
'TSINCR',
'SBASICALW',
'SBASICDA',
'Ptax',
'INCREMENT')




ALTER TABLE dbo.pr_emp_tds_section_deductions DROP CONSTRAINT PK__pr_emp_t__3213E83F84FC2D62


insert into pr_emp_tds_section_deductions 
select row_number() OVER(ORDER BY m.empid ASC),2020,'2020-03-01',
e.id,m.empid,mas.id,concat('Section',seccode),p.amount,p.amount,p.amount
,0,1000 from test2.dbo.pitother p join test2.dbo.pempmast m on p.eid=m.eid 
join pr_deduction_field_master mas on mas.name=p.flddesp   
COLLATE SQL_Latin1_General_CP1_CI_AS and type='per_ded'
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where finyear='2019-20' and edflag='D'
--select * from pr_emp_perdeductions




--'leave Encashment
insert into pr_emp_perearning 
select row_number() OVER(ORDER BY m.empid ASC),2020,'2020-03-01',e.id,m.empid,
(select id from pr_earn_field_master where name='Leave Encashment')
,'per_earn',
encashamt,null,0,1000 from test2.dbo.pitval p 
join test2.dbo.pempmast m on p.eid=m.eid  
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where pmonth='202003'


--pitother
insert into pr_emp_perearning 
select row_number() OVER(ORDER BY m.empid ASC),2020,'2020-03-01',
e.id,m.empid,mas.id,'per_earn',p.amount,null
,0,1000 from test2.dbo.pitother p join test2.dbo.pempmast m on p.eid=m.eid 
join pr_earn_field_master mas on mas.name=p.flddesp   
COLLATE SQL_Latin1_General_CP1_CI_AS and type='per_earn'
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where finyear='2019-20' and edflag='E'
--select * from pr_emp_perearning



--INCREMENT
insert into pr_emp_perearning 
select row_number() OVER(ORDER BY m.empid ASC),2020,'2020-03-01',e.id,m.empid,
(select id from pr_earn_field_master where name='INCREMENT')
,'per_earn',
totalamt,null,0,1000 from test2.dbo.pitcal p 
join test2.dbo.pempmast m on p.eid=m.eid  
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where p.pmonth='202003'  and fldname='INCREMENT'

