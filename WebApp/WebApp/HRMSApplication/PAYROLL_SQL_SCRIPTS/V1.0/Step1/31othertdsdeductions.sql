
create procedure sp_dm_other_tds
as 
insert into pr_emp_other_tds_deductions 
select row_number() OVER(ORDER BY m.empid ASC),2021,(select fm from pr_month_details where active=1),
e.id,m.empid,p.atds,'dedtds'
,1,1000 from test2.dbo.EXT$ p join test2.dbo.pempmast m on p.eid=m.eid 
join employees e on e.empid=m.empid COLLATE SQL_Latin1_General_CP1_CI_AS

-- begin
-- declare @fys NVARCHAR(100);
-- declare @fyQrys NVARCHAR(100);
-- set @fyQrys = 'select @fys = dbo.getFY('+@pmonths+');';
-- execute sp_executesql 
-- @fyQrys, 
-- N'@fys NVARCHAR(100) OUTPUT', 
-- @fys = @fys output;

-- declare @fyPeriods NVARCHAR(100);
-- declare @fyPeriodQrys NVARCHAR(100);
-- set @fyPeriodQrys = 'select @fyPeriods = dbo.getFYPeriod('+@pmonths+');';
-- execute sp_executesql 
-- @fyPeriodQrys, 
-- N'@fyPeriods NVARCHAR(100) OUTPUT', 
-- @fyPeriods = @fyPeriods output;
-- print(@fyPeriods);
-- declare @fms NVARCHAR(MAX);
-- set @fms = cast(left(@pmonths,4) as varchar(20))+'-'+ right(@pmonths,2)+'-'+'01';
-- --select top 1 * from [TEST2].dbo.PItDet where FinYear='2019-20';

-- declare @StartValues int;
-- set @StartValues=(select last_num from new_num where table_name = 'pr_emp_other_tds_deductions');
-- declare @initValues int;
-- set @initValues = @StartValues+1;

-- Insert into pr_emp_other_tds_deductions ([id],[fy],[fm],[emp_id],[emp_code],[tds_amount],[remarks],[active],[trans_id]) values 
-- (@StartValues+1,@fys,@fms,(select id from employees where empid = @employeecodes),@employeecodes,(select p.ATDS from [TEST2].dbo.EXT$ p left outer join [TEST2].dbo.pempmast m on p.eid=m.Eid where m.empid=@employeecodes),'dedtds',1, 1000);


-- declare @lastnums int;
-- set @lastnums = (select top 1 id from pr_emp_perearning order by id desc)
-- update new_num set last_num=@lastnums where table_name='pr_emp_other_tds_deductions';
-- --exec sp_dm_Exgraticas_tds 554,201908

-- --select * from pr_emp_perearning 
-- --select * from pr_emp_other_tds_deductions
-- ---delete from pr_emp_other_tds_deductions

-- end





