create procedure sp_dm_tax_source (@employeecode as int, @pmonth as varchar(20))
as
begin
declare @pset NVARCHAR(MAX);
set @pset = (select PMonth from test2.dbo.PSet where MFlag='N');

declare @fy NVARCHAR(100);
declare @fyQry NVARCHAR(100);
set @fyQry = 'select @fy = dbo.getFY('+@pset+');';
execute sp_executesql 
    @fyQry, 
    N'@fy NVARCHAR(100) OUTPUT', 
    @fy = @fy output;


declare @fm NVARCHAR(MAX);
set @fm = cast(left(@pset,4) as varchar(20))+'-'+ right(@pset,2)+'-'+'01';

declare @StartValue int;
set @StartValue=(select last_num from new_num where table_name = 'pr_emp_tds_process');
declare @initValue int;


select tds as tax,'1' as id into #tds_data from test2.dbo.PPayslip p join test2.dbo.PEMPMAST m on p.Eid=m.Eid where m.Empid=@employeecode
union all select sum(Tds) as tax,'2' as id  from test2.dbo.POldslip p join test2.dbo.PEMPMAST m on p.Eid=m.Eid where m.Empid=@employeecode and Pmonth>=@pmonth
union all
select sum(Tds) as tax,'3' as id from [TEST2].dbo.PEncashment e join [TEST2].dbo.pempmast m on e.EID = m.EID  join TEST2.dbo.[PEncash ] p on p.eid=e.eid
where  p.paydate between '2019-04-01' and GETDATE() and e.pmonth>201903 and m.Empid = @employeecode
union all 
select sum(Tds) as tax,'4' as id from TEST2.dbo.PAdhslip p  join [TEST2].dbo.pempmast m on p.EID = m.EID 
where p.paydate between '2019-04-01' and GETDATE() and m.empid=@employeecode;

--select * from #tds_data;

declare @ID int;
declare @tax_total float;
declare @tax float;
set @tax_total = 0;
set @initValue = @StartValue+1;
while exists (select * from #tds_data)
begin
    select @ID = (select top 1 id from #tds_data order by id asc);
	select @tax = tax from #tds_data where id = @ID;
	set @tax_total = @tax_total+@tax;
	delete from #tds_data where id=@ID;
end
--insert into tds 
INSERT INTO pr_emp_tds_process ([id],[fy],[fm],[empid],[empcode],[tax_deducted_at_source],[active],[trans_id],[tds_update],[final]) 
VALUES (@initValue,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode,@tax_total,1,1000,0,0);

 --update new num
  declare @lastnuma int;
set @lastnuma = (select top 1 id from pr_emp_tds_process order by id desc)
update new_num set last_num=@lastnuma where table_name='pr_emp_tds_process';

end

--drop table #tds_data;

--sp_dm_tax_source 371,201904;

--select * from pr_emp_tds_process where empcode=371;

--delete from pr_emp_tds_process where empcode=371;

--select sum(dd_income_tax) from pr_emp_payslip where emp_code=371;

--select fm,spl_type,active from  pr_emp_payslip where emp_code=371;




