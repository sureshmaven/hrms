--select * from pr_emp_perearning
--delete from pr_emp_perearning
--exec sp_pr_emp_perearning_ded
CREATE Procedure sp_pr_emp_perearning_ded as

declare @id int;
declare @pmonthyr int;
declare @y1 int;
declare @y2 int;
declare @pmonthnew nvarchar(20);
declare @finyearyr nvarchar(20);
declare @empcode int;
declare @dedcode nvarchar(100);
declare @amount decimal(18,2);

update new_num set last_num=(case when (select max(id)+1 from pr_emp_perearning) is null then 1 else (select max(id)+1 from pr_emp_perearning) end) where table_name = 'pr_emp_perearning'
set @id=(select last_num+1 from new_num where table_name='pr_emp_perearning')
set @y1=2011;
set @y2=12;
set @finyearyr=CONCAT(@y1,'-',@y2);
set @pmonthyr = 2012;
set @pmonthnew = CONCAT(@pmonthyr-1,'-03','-01')
while (@pmonthyr<=2021)
begin
select m.empid,p.FldDesp,p.amount into #PerEarning from test2.dbo.pitother p join test2.dbo.pempmast m on p.eid=m.eid 
join pr_earn_field_master mas on mas.name=p.flddesp   
COLLATE SQL_Latin1_General_CP1_CI_AS and type='per_earn'
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where finyear=@finyearyr and edflag='E' and p.amount!=0

--drop table #PerEarning
--select * from #PerEarning
while exists (select * from #PerEarning)
Begin
set @empcode=(select top 1 empid from #PerEarning)
set @dedcode=(select top 1 FldDesp from #PerEarning)
set @amount=(select top 1 amount from #PerEarning)

Insert into pr_emp_perearning values (@id,@pmonthyr,@pmonthnew,(select eid from Employees where empid=@empcode),@empcode,(select id from pr_earn_field_master where name=@dedcode),'per_earn',@amount,' ',0,1000)

Delete from #PerEarning where empid=@empcode and FldDesp=@dedcode and amount=@amount;
set @id=@id+1;
End

set @y1=@y1+1;
set @y2=@y2+1;
set @finyearyr=CONCAT(@y1,'-',@y2);
set @pmonthyr = @pmonthyr+1;
set @pmonthnew = CONCAT(@pmonthyr-1,'-03','-01')

Drop table #PerEarning;
End

update new_num set last_num=@id where table_name='pr_emp_perearning';
--exec sp_pr_emp_perearning_ded