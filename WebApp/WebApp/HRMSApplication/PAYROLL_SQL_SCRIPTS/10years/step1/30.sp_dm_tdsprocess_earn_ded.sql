--exec sp_dm_tdsprocess_earn_ded
CREATE procedure sp_dm_tdsprocess_earn_ded as 

declare @id int;
declare @yr NVARCHAR(20);
declare @yrmon NVARCHAR(100);
declare @pmonthyr NVARCHAR(100);
declare @pmonthnew NVARCHAR(100);
declare @finyearyr NVARCHAR(100);
declare @empid int;
declare @fldname nvarchar(100);
declare @amount decimal(18,2);
--declare @finyearyr1 NVARCHAR(100);
set @yr='201104';
set @pmonthyr=SUBSTRING(@yr,0,5)+1;
set @finyearyr=CONCAT(SUBSTRING(@yr,0,5),'-',SUBSTRING(@yr,5,6));
set @pmonthnew=CONCAT(SUBSTRING(@yr,0,5),'-',SUBSTRING(@yr,5,6),'-01');
--set @finyearyr1=CONCAT(SUBSTRING(@yr,0,5)-1,'-',SUBSTRING(@yr,3,2));
update new_num set last_num=(case when (select max(id)+1 from pr_emp_perearning) is null then 1 else (select max(id)+1 from pr_emp_perearning) end) where table_name = 'pr_emp_perearning'
set @id=(select last_num from new_num where table_name='pr_emp_perearning')
print @yr;--201203
print @pmonthyr;--2012
print @finyearyr;--2012-03
print @pmonthnew;--2012-03-01
--print @finyearyr1;--2011-12

while(CAST (@yr as int)<=202010)
begin
--pitcal
--select * from pr_emp_perearning
--select * from #pitcal
select m.empid,p.fldname,(p.TotalAmt-p.EncashAmt) as amount into #pitcal from test2.dbo.pitcal p join test2.dbo.pempmast m on p.eid=m.eid 
join pr_earn_field_master mas on mas.name=p.fldname COLLATE SQL_Latin1_General_CP1_CI_AS and type='per_earn'
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where pmonth=@yr and p.fldname   not in ('Basic',
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
'INCREMENT') and (p.TotalAmt-p.EncashAmt)!=0

while exists (select * from #pitcal)
begin

set @empid= (select top 1 empid from #pitcal)
set @fldname = (select top 1 fldname from #pitcal)
set @amount = (select top 1 amount from #pitcal)

Insert into pr_emp_perearning values (@id,@pmonthyr,@pmonthnew,(select eid from Employees where empid=@empid),@empid,(select id from pr_earn_field_master where name=@fldname),'per_earn',@amount,' ',0,1000)

set @id=@id+1;
delete from #pitcal where empid=@empid and fldname=@fldname and amount=@amount
end
print '1'
drop table #pitcal;
update new_num set last_num=@id where table_name='pr_emp_perearning';


--ALTER TABLE dbo.pr_emp_tds_section_deductions DROP CONSTRAINT PK__pr_emp_t__3213E83F84FC2D62


--insert into pr_emp_tds_section_deductions 
--select row_number() OVER(ORDER BY m.empid ASC),@pmonthyr,@pmonthnew,
--e.id,m.empid,mas.id,concat('Section',seccode),p.amount,p.amount,p.amount
--,0,1000 from test2.dbo.pitother p join test2.dbo.pempmast m on p.eid=m.eid 
--join pr_deduction_field_master mas on mas.name=p.flddesp   
--COLLATE SQL_Latin1_General_CP1_CI_AS and type='per_ded'
--join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
--where finyear=@finyearyr1 and edflag='D'
----select * from pr_emp_perdeductions
--print '2'



--'leave Encashment

select m.empid,encashamt into #leave_Encashment from test2.dbo.pitval p 
join test2.dbo.pempmast m on p.eid=m.eid  
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where pmonth=@yr and encashamt!=0

set @id=(select last_num+1 from new_num where table_name='pr_emp_perearning')
while exists (select * from #leave_Encashment)
begin


set @empid =(select top 1 empid from #leave_Encashment)
set @amount = (select top 1 encashamt from #leave_Encashment)

Insert into pr_emp_perearning values(@id,@pmonthyr,@pmonthnew,(select eid from Employees where empid=@empid),@empid,(select id from pr_earn_field_master where name='Leave Encashment'),'per_earn',@amount,' ',0,1000)

set @id=@id+1;
delete from #leave_Encashment where empid=@empid and encashamt=@amount
end
drop table #leave_Encashment;
update new_num set last_num=@id where table_name='pr_emp_perearning';

print 'Leave Encashment'

--pitother
--insert into pr_emp_perearning 
--select row_number() OVER(ORDER BY m.empid ASC),@pmonthyr,@pmonthnew,
--e.id,m.empid,mas.id,'per_earn',p.amount,null
--,0,1000 from test2.dbo.pitother p join test2.dbo.pempmast m on p.eid=m.eid 
--join pr_earn_field_master mas on mas.name=p.flddesp   
--COLLATE SQL_Latin1_General_CP1_CI_AS and type='per_earn'
--join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
--where finyear=@finyearyr1 and edflag='E'
----select * from pr_emp_perearning
--print '4'

--GSLI
--select * from pr_emp_perdeductions where id=20
select m.empid,totalamt into #GSLI from test2.dbo.pitcal p 
join test2.dbo.pempmast m on p.eid=m.eid  
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where p.pmonth=@yr  and fldname='GSLI' and totalamt!=0

set @id=(select last_num+1 from new_num where table_name='pr_emp_perdeductions')
while exists(select * from #GSLI)
begin


set @empid =(select top 1 empid from #GSLI)
set @amount = (select top 1 totalamt from #GSLI)

Insert into pr_emp_perdeductions values(@id,@pmonthyr,@pmonthnew,(select eid from Employees where empid=@empid),@empid,(select id from pr_deduction_field_master where name='GSLI' and type='EPD'),'per_ded',@amount,'Section80C',0,1000)

set @id=@id+1;
delete from #GSLI where empid=@empid and totalamt=@amount

end
print 'GSLI';
drop table #GSLI;
update new_num set last_num=@id where table_name='pr_emp_perdeductions';
update pr_emp_perdeductions set active=1 where fm=(select fm from pr_month_details where active=1)

--INCREMENT
--drop table #INCREMENT
--select * from #INCREMENT
select m.empid,totalamt into #INCREMENT from test2.dbo.pitcal p 
join test2.dbo.pempmast m on p.eid=m.eid  
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where p.pmonth=@yr  and fldname='INCREMENT' and totalamt!=0

set @id=(select last_num+1 from new_num where table_name='pr_emp_perearning')
while exists(select * from #INCREMENT)
begin


set @empid =(select top 1 empid from #INCREMENT)
set @amount = (select top 1 totalamt from #INCREMENT)

Insert into pr_emp_perearning values(@id,@pmonthyr,@pmonthnew,(select eid from Employees where empid=@empid),@empid,(select id from pr_earn_field_master where name='INCREMENT'),'per_earn',@amount,' ',0,1000)

set @id=@id+1;
delete from #INCREMENT where empid=@empid and totalamt=@amount

end
print 'INCREMENT';
drop table #INCREMENT;
update new_num set last_num=@id where table_name='pr_emp_perearning';

print '5'
set @yr=@yr+1;
if(SUBSTRING(@yr,5,2)='13')
begin
set @yr=CONCAT(SUBSTRING(@yr,0,5)+1,'01');
end
set @pmonthyr=SUBSTRING(@yr,0,5)+1;
set @finyearyr=CONCAT(SUBSTRING(@yr,0,5),'-',SUBSTRING(@yr,5,6));
set @pmonthnew=CONCAT(SUBSTRING(@yr,0,5),'-',SUBSTRING(@yr,5,6),'-01');
--set @finyearyr1=CONCAT(SUBSTRING(@yr,0,5)-1,'-',SUBSTRING(@yr,3,2));
print @yr;--201303
print @pmonthyr;--2013
print @finyearyr;--2013-03
print @pmonthnew;--2013-03-01
--print @finyearyr1;--2012-13

end
update new_num set last_num=@id where table_name='pr_emp_perearning';