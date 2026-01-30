

--exec [sp_dm_encashment_present] 304,'201909'
--exec [sp_dm_encashment_present] 513,'201909' 
--select * from PLE_Type;

--delete from PLE_Type where id in(12,13,14);


create procedure [dbo].[sp_dm_encashment_present](@employeecode int,@pmonth varchar(20))
as 
begin
declare @fy NVARCHAR(100);
declare @fyQry NVARCHAR(100);
set @fyQry = 'select @fy = dbo.getFY('+@pmonth+');';
execute sp_executesql 
    @fyQry, 
    N'@fy NVARCHAR(100) OUTPUT', 
    @fy = @fy output;

declare @fm NVARCHAR(MAX);
set @fm = cast(left(@pmonth,4) as varchar(20))+'-'+ right(@pmonth,2)+'-'+'01';

if exists(select * from [TEST2].dbo.PEncashment e join [TEST2].dbo.pempmast m on e.EID = m.EID  join TEST2.dbo.[PEncash ] p on p.eid=e.eid
where  p.paydate between '2019-04-01' and GETDATE() and e.pmonth>201903 and m.Empid = @employeecode and e.pmonth=@pmonth)
begin

insert into PLE_Type values((select id from employees where empid=@employeecode),(select ControllingAuthority from employees where empid=@employeecode),
(select SanctioningAuthority from employees where empid=@employeecode),'PL ENCASHMENT','PL ENCASHMENT',123456,getdate(),'Pending',null,
(select TotalExperience from employees where EmpId=@employeecode),3,'Encashment',2019,(select LeaveBalance from EmpLeaveBalance 
where empid=(select id from employees where empid=@employeecode) and LeaveTypeId=3),(select p.EncashDays from test2.dbo.PEncashment p join test2.dbo.PEMPMAST m on p.eid=m.eid where m.Empid=@employeecode and p.pmonth=@pmonth),(select Branch from employees where empid=@employeecode),
(select Department from employees where empid=@employeecode),(select CurrentDesignation from employees where empid=@employeecode),
(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=@employeecode) and LeaveTypeId=3),
1,0,@fy,@fm,null);
end

end
