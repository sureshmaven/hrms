create procedure sp_dm_rent(@employeecode int,@pmonth varchar(20))
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
	print(@fyPeriod);
declare @fm NVARCHAR(MAX);
set @fm = cast(left(@pmonth,4) as varchar(20))+'-'+ right(@pmonth,2)+'-'+'01';
--select top 1 * from [TEST2].dbo.PItDet where FinYear='2019-20';

declare @StartValue int;
set @StartValue=(select last_num from new_num where table_name = 'pr_emp_rent_details');
declare @initValue int;
set @initValue = @StartValue+1;

Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+1,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 2,(select p.Hra01Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+2,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 3,(select p.Hra02Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+3,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 4,(select p.Hra03Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+4,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 5,(select p.Hra04Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+5,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 6,(select p.Hra05Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+6,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 7,(select p.Hra06Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+7,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 8,(select p.Hra07Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+8,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 9,(select p.Hra08Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+9,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 10,(select p.Hra09Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+10,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 11,(select p.Hra10Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+11,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 12,(select p.Hra11Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);
Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values 
(@StartValue+12,@fy,@fm,(select id from employees where empid = @employeecode),@employeecode, 13,(select p.Hra12Amt from [TEST2].dbo.PItDet p left outer join [TEST2].dbo.pempmast m on p.Eid=m.Eid where p.FinYear=@fyPeriod and m.Empid=@employeecode),1, 1000);

declare @lastnum int;
set @lastnum = (select top 1 id from pr_emp_rent_details order by id desc)
update new_num set last_num=@lastnum where table_name='pr_emp_rent_details';

end

--exec sp_dm_rent 271,'201907';

--select * from pr_emp_rent_details where emp_code=271;
