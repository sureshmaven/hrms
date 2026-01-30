create procedure loans_olddata @loan nvarchar(10) as

declare @id int;
set @id=1
declare @tempdate date;
declare @tempdate1 date;

declare @loantype varchar(10);
set @loantype=@loan
while (@id<=(select max(id) from test2.dbo.pempmast))
begin

declare @empid int;
set @empid=(select empid from test2.dbo.pempmast where id=@id )

declare @pmonth int;
set @pmonth=202009
declare @lastmonth int;
declare @lastmonth1 int;

set @lastmonth=(select top 1 max(pmonth) from test2.dbo.poldloan loan join test2.dbo.pempmast m on loan.eid=m.eid
where m.empid=@empid
and loanid=@loantype group by pmonth
order by pmonth desc)--202002
if (@pmonth!= @lastmonth)
begin

set @tempdate=DATEADD(month, 1,(concat(left(@lastmonth,4),'-',right(@lastmonth,2),'-',01)));
set @lastmonth1=(select convert(varchar(6),@tempdate,112))
insert into dm_emp_mn_task values(@empid,@lastmonth1,concat(@loantype,','));
end
print 'Testing purpose';
print '@pmonth';
print @pmonth;
print '@tempdate';
print @tempdate;
print '@lastmonth';
print @lastmonth;
print '@lastmonth1';
print @lastmonth1;
print '@empid';
print @empid;
print '@loantype';
print @loantype;
print '@loan';
print @loan;
print '@tempdate1';
print @tempdate1;
print 'Testing purpose';
set @id=@id+1
end

--create table LoansData_temp(empid int, pmonth int,loantype nvarchar(100));
--select * from LoansData_temp order by pmonth desc
--delete from LoansData_temp
--drop table LoansData_temp
--exec loans_olddata 'FEST'
--exec loans_olddata 'HLADD'
--exec loans_olddata 'HLPLT'
--exec loans_olddata 'HOUS1'
--exec loans_olddata 'HL2BC'
--exec loans_olddata 'HL2A'
--exec loans_olddata 'PFHT1'
--exec loans_olddata 'PFHT2'
--exec loans_olddata 'PFLT1'
--exec loans_olddata 'PFLT2'
--exec loans_olddata 'PFLT3'
--exec loans_olddata 'PFLT4'
--exec loans_olddata 'VEH2W'
--exec loans_olddata 'VEH4W'
--exec loans_olddata 'PERS'
