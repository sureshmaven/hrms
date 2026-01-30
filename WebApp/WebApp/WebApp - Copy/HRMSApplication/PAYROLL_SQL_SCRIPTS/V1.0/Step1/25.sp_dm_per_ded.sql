----Not in use
----Not in use
----Not in use
----Not in use
----Not in use
--create procedure sp_dm_per_ded(@fy nvarchar(10))
--as 
---- begin
---- declare @fy NVARCHAR(100);
---- declare @fyQry NVARCHAR(100);
---- set @fyQry = 'select @fy = dbo.getFY('+@pmonth+');';
---- execute sp_executesql 
--    -- @fyQry, 
--    -- N'@fy NVARCHAR(100) OUTPUT', 
--    -- @fy = @fy output;

--	-- declare @fyPeriod NVARCHAR(100);
---- declare @fyPeriodQry NVARCHAR(100);
---- set @fyPeriodQry = 'select @fyPeriod = dbo.getFYPeriod('+@pmonth+');';
---- execute sp_executesql 
--    -- @fyPeriodQry, 
--    -- N'@fyPeriod NVARCHAR(100) OUTPUT', 
--    -- @fyPeriod = @fyPeriod output;
--	-- --print(@fyPeriod);
---- declare @fm NVARCHAR(MAX);
---- set @fm = cast(left(@pmonth,4) as varchar(20))+'-'+ right(@pmonth,2)+'-'+'01';

--CREATE  TABLE #per_ded (
--name nvarchar(100), 
--amount INT, 
--sec nvarchar(100)
--);

--insert into #per_ded select p.FldDesp as name, p.Amount as amount,CONCAT('Section',p.SecCode) as sec 
--from [TEST2].dbo.PItOther p left outer join [TEST2].dbo.pempmast 
--m on p.Eid=m.Eid  where FinYear=@fy and EdFlag='D'

----select * from #per_ded;

--declare @new_num int;
--declare @new_id int;
--set @new_num=(select last_num from new_num where table_name = 'pr_emp_perdeductions');
--set @new_id = @new_num;

--declare @name varchar(50);
--declare @amount varchar(50);
--declare @sec varchar(50);

--while exists (select * from #per_ded)
--begin
--	set @new_id = @new_id+1;
--    select @name = (select top 1 name from #per_ded);
--	select @amount = (select top 1 amount from #per_ded);
--	select @sec = (select top 1 sec from #per_ded);
--	if not exists(select id from pr_deduction_field_master where type='per_ded' and name=@name) 
-- --   begin
--	----insertion
--	-- Insert into pr_emp_perdeductions ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],
-- --[amount],[section],[active],[trans_id]) values 
-- --(@new_id,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode,(select id from pr_deduction_field_master where type='per_ded' and name=@name),'per_ded',@amount,@sec,1,1000);
--	--select * from pr_emp_general;
-- --   end
--	--else
--	begin
--	--new insertion in both master and personal earnings table
--	declare @StartValue int;

--set @StartValue=(select  last_num from new_num where table_name =  'pr_deduction_field_master' );

--insert into pr_deduction_field_master values(@StartValue+1,@name,'per_ded',1,1000,null,null,null);


--declare @lastnum int;
--set @lastnum = (select top 1 id from pr_deduction_field_master order by id desc);

---- insert into pr_emp_perdeductions values(@lastnum,@fy,@fm,(select id from Employees where EmpId=@employeecode),@employeecode,
---- (select id from pr_deduction_field_master where name=@name and type='per_ded'),'per_ded',@amount,@sec,1,1000);

--update new_num set last_num=@lastnum where table_name='pr_deduction_field_master';

--	--select * from pr_emp_general;
--	end
--  delete from #per_ded where name=@name and amount=@amount;
--end

--	insert into pr_emp_perdeductions 
--select row_number() OVER(ORDER BY m.empid ASC),2021,(select fm from pr_month_details where active=1),
--e.id,m.empid,mas.id,'per_ded',p.Amount as amount,CONCAT('Section',p.SecCode)
--,1,1000 from test2.dbo.pitother p join test2.dbo.pempmast m on p.eid=m.eid 
--join pr_deduction_field_master mas on mas.name=p.FldDesp   
--COLLATE SQL_Latin1_General_CP1_CI_AS and type='per_ded' and  EdFlag='D' and FinYear=@fy
--join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS

----end

----exec sp_dm_per_ded 175,'201910';

----select * from pr_emp_perdeductions;

----delete from pr_emp_perdeductions;

---- select p.FldDesp, p.Amount,CONCAT('Section',p.SecCode) as SecCode,m.Empid 
---- from [TEST2].dbo.PItOther p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid  
---- where FinYear='2019-20' and EdFlag='D' and m.Empid=175;



