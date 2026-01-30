create procedure sp_dm_per_earn(@employeecode int,@pmonth varchar(20))
as 
begin
declare @fy NVARCHAR(100);
declare @fyQry NVARCHAR(100);
set @fyQry = 'select @fy = dbo.getFY('+@pmonth+');';
execute sp_executesql 
    @fyQry, 
    N'@fy NVARCHAR(100) OUTPUT', 
    @fy = @fy output;

	declare @fyPeriod NVARCHAR(100);
declare @fyPeriodQry NVARCHAR(100);
set @fyPeriodQry = 'select @fyPeriod = dbo.getFYPeriod('+@pmonth+');';
execute sp_executesql 
    @fyPeriodQry, 
    N'@fyPeriod NVARCHAR(100) OUTPUT', 
    @fyPeriod = @fyPeriod output;
	--print(@fyPeriod);
declare @fm NVARCHAR(MAX);
set @fm = cast(left(@pmonth,4) as varchar(20))+'-'+ right(@pmonth,2)+'-'+'01';

CREATE  TABLE #per_earn (
name nvarchar(100), 
amount INT, 
sec nvarchar(100)
);

insert into #per_earn select p.FldDesp as name, p.Amount as amount,p.SecCode as sec 
from [TEST2].dbo.PItOther p left outer join [TEST2].dbo.pempmast 
m on p.Eid=m.Eid  where FinYear=@fyPeriod and EdFlag='E' and m.Empid=@employeecode and p.FldDesp not in('Interest On NSC (Earning)');

--select * from #per_earn;

declare @new_num int;
declare @new_id int;
set @new_num=(select last_num from new_num where table_name = 'pr_emp_perearning');
set @new_id = @new_num;

declare @name varchar(50);
declare @amount varchar(50);
declare @sec varchar(50);

while exists (select * from #per_earn)
begin
	set @new_id = @new_id+1;
    select @name = (select top 1 name from #per_earn);
	select @amount = (select top 1 amount from #per_earn);
	select @sec = (select top 1 sec from #per_earn);
	if exists(select id from pr_earn_field_master where type='per_earn' and name=@name) 
    begin
	--insertion
	Insert into pr_emp_perearning ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],
[amount],[active],[trans_id]) values 
(@new_id,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode,(select id from pr_earn_field_master where type='per_earn' and name=@name),'per_earn',@amount,1,1000);
	--select * from pr_emp_general;
    end
	else
	begin
	--new insertion in both master and personal earnings table
	declare @StartValue int;

set @StartValue=(select  last_num from new_num where table_name =  'pr_earn_field_master' );

insert into pr_earn_field_master values(@StartValue+1,@name,'per_earn',1,1000,null,null,null);


declare @lastnum int;
set @lastnum = (select top 1 id from pr_earn_field_master order by id desc);

insert into pr_emp_perearning values(@lastnum,@fy,@fm,(select id from Employees where EmpId=@employeecode),@employeecode,
(select id from pr_earn_field_master where name=@name and type='per_earn'),'per_earn',@amount,@sec,1,1000);

update new_num set last_num=@lastnum where table_name='pr_earn_field_master';

	--select * from pr_emp_general;
	end
  delete from #per_earn where name=@name and amount=@amount;
end
end

--exec sp_dm_per_earn 793,'201908';

--select * from pr_emp_perearning;

--delete from pr_emp_perearning;



