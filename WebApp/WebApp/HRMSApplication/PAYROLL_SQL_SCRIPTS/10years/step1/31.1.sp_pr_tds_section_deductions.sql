
--exec sp_pr_tds_section_deductions
CREATE procedure sp_pr_tds_section_deductions as
declare @y1 int;
declare @y2 int;
declare @yr NVARCHAR(100);
declare @yrmon NVARCHAR(100);
declare @pmonthyr NVARCHAR(100);
declare @pmonthnew NVARCHAR(100);
set @y1=2011;
set @y2=12;
set @yr=concat(@y1,'-',@y2);
set @pmonthyr=cast(@y1 as int)+1;
set @pmonthnew=CONCAT(@pmonthyr,'-03','-01');

WHILE (@y1<=2020)
BEGIN
--select * from #TdsDeductionList
Select distinct flddesp as [Name] into #TdsDeductionList from test2.dbo.pitother 
Select distinct flddesp as [Name] into #TdsDeductionList1 from test2.dbo.pitother 
declare @tdsdedid int;
declare @ids int;
declare @empcodes int;
declare @grossamount decimal(18,2);
declare @tdsdedname NVARCHAR(500);
declare @tdsdedname1 NVARCHAR(500);
update new_num set last_num=(case when (select max(id)+1 from pr_emp_tds_section_deductions) is null then 1 else (select max(id)+1 from pr_emp_tds_section_deductions) end) where table_name = 'pr_emp_tds_section_deductions'
set @ids=(SELECT last_num+1 from new_num where table_name='pr_emp_tds_section_deductions')

WHILE exists(SELECT * from #TdsDeductionList)
begin
set @tdsdedname=(SELECT Top 1 [Name] from #TdsDeductionList)
set @tdsdedname1=(SELECT Top 1 [Name] from #TdsDeductionList1)
if exists (select id from pr_deduction_field_master where type='per_ded' and name=@tdsdedname)
begin

--WHILE exists(SELECT * from #TdsDeductionList)
--	BEGIN
	set @tdsdedname=(SELECT Top 1 [Name] from #TdsDeductionList)
	--set @tdsdedid=(SELECT [Id] from #TdsDeductionList where Name=@tdsdedname)

	SELECT p.eid,m.Empid,(SELECT id from pr_deduction_field_master where name=@tdsdedname and type='per_ded')as m_id,Concat('Section',p.SecCode)as section_type,p.amount as Gross, p.amount as Qual, p.amount as Ded, 0 as active, 1000 as trans_id into #TdsDeductionInsert from test2.dbo.pitother p join test2.dbo.pempmast m on p.eid=m.eid where EdFlag='D' and FldDesp=@tdsdedname and FinYear=@yr

	WHILE exists (SELECT * from #TdsDeductionInsert)
	BEGIN

	CREATE table #Empids (Empid int,Gross decimal(18,2))

	Insert into #Empids (Empid,Gross)SELECT Top 1 Empid,Gross from #TdsDeductionInsert

	WHILE exists (SELECT * from #Empids)
	BEGIN
	set @empcodes=(SELECT top 1 Empid from #Empids)
	set @grossamount =(SELECT top 1 Gross from #Empids)
	Insert into pr_emp_tds_section_deductions ([id],[fy],[fm],[empid],[empcode],[m_id],[section_type],[gross],[qual],[ded],[active],[trans_id])SELECT @ids,@pmonthyr,@pmonthnew,[eid],[Empid],[m_id],[section_type],[Gross],[Qual],[Ded],[active],[trans_id] FROM #TdsDeductionInsert where Empid=@empcodes and Gross=@grossamount

	set @ids=@ids+1;
	print 'Inserted into pr_emp_tds_section_deductions IF';
	print @empcodes;

	DELETE from #Empids where Empid=@empcodes and Gross=@grossamount;
	update new_num set last_num=@ids where table_name='pr_emp_tds_section_deductions';
	END

	DROP table #Empids;
	DELETE from #TdsDeductionInsert where Empid=@empcodes and Gross=@grossamount;
	END
	DROP table #TdsDeductionInsert;

	--DELETE from #TdsDeductionList where name=@tdsdedname
	--END
	end

else

begin

	declare @StartValue int;
	set @StartValue=(select  max(id) from pr_deduction_field_master);
	insert into pr_deduction_field_master values(@StartValue+1,@tdsdedname,'per_ded',1,1000,null,null,null);

	declare @lastnum int;
	set @lastnum = (select top 1 id from pr_deduction_field_master order by id desc);

	update new_num set last_num=@lastnum where table_name='pr_deduction_field_master';

	--WHILE exists(SELECT * from #TdsDeductionList1)
	--BEGIN
	set @tdsdedname=(SELECT Top 1 [Name] from #TdsDeductionList1)
	--set @tdsdedid=(SELECT [Id] from #TdsDeductionList where Name=@tdsdedname)

	SELECT p.eid,m.Empid,(SELECT id from pr_deduction_field_master where name=@tdsdedname and type='per_ded')as m_id,Concat('Section',p.SecCode)as section_type,p.amount as Gross, p.amount as Qual, p.amount as Ded, 0 as active, 1000 as trans_id into #TdsDeductionInsert1 from test2.dbo.pitother p join test2.dbo.pempmast m on p.eid=m.eid where EdFlag='D' and FldDesp=@tdsdedname and FinYear=@yr

	WHILE exists (SELECT * from #TdsDeductionInsert1)
	BEGIN

	CREATE table #Empids1 (Empid int,Gross decimal(18,2))

	Insert into #Empids1 (Empid,Gross)SELECT Top 1 Empid,Gross from #TdsDeductionInsert1

	WHILE exists (SELECT * from #Empids1)
	BEGIN
	set @empcodes=(SELECT top 1 Empid from #Empids1)
	set @grossamount =(SELECT top 1 Gross from #Empids1)
	Insert into pr_emp_tds_section_deductions ([id],[fy],[fm],[empid],[empcode],[m_id],[section_type],[gross],[qual],[ded],[active],[trans_id])SELECT @ids,@pmonthyr,@pmonthnew,[eid],[Empid],[m_id],[section_type],[Gross],[Qual],[Ded],[active],[trans_id] FROM #TdsDeductionInsert1 where Empid=@empcodes and Gross=@grossamount

	set @ids=@ids+1;
	print 'Inserted into pr_emp_tds_section_deductions ELSE';
	print @empcodes;

	DELETE from #Empids1 where Empid=@empcodes and Gross=@grossamount;
	update new_num set last_num=@ids where table_name='pr_emp_tds_section_deductions';
	END

	DROP table #Empids1;
	DELETE from #TdsDeductionInsert1 where Empid=@empcodes and Gross=@grossamount;
	END
	DROP table #TdsDeductionInsert1;

	--DELETE from #TdsDeductionList1 where name=@tdsdedname
	--END
end
DELETE from #TdsDeductionList where name=@tdsdedname
DELETE from #TdsDeductionList1 where name=@tdsdedname1
end
set @y1=@y1+1;
set @y2=@y2+1;
set @yr=concat(@y1,'-',@y2);
set @pmonthyr=cast(@y1 as int)+1;
if(@y1=(select year(fm) from pr_month_details where active=1))
begin
set @pmonthnew=(select fm from pr_month_details where active=1)
end
else
begin
set @pmonthnew=CONCAT(@pmonthyr,'-03','-01');
end
DROP Table #TdsDeductionList
DROP Table #TdsDeductionList1
END

--delete from #TdsDeductionList
update new_num set last_num=@ids where table_name='pr_emp_tds_section_deductions';
update pr_emp_tds_section_deductions set active=1 where fm=(select fm from pr_month_details where active=1)
--select * from pr_emp_tds_section_deductions
--exec sp_pr_tds_section_deductions
