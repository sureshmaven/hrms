--exec sp_pr_section_deductions
CREATE procedure sp_pr_section_deductions as
declare @y1 int;
declare @y2 int;
declare @yr NVARCHAR(100);
declare @yrmon NVARCHAR(100);
declare @pmonthyr NVARCHAR(100);
declare @pmonthnew NVARCHAR(100);
set @y1=2019;
set @y2=20;
set @yr=concat(@y1,'-',@y2);
set @pmonthyr=cast(@y1 as int)+1;
set @pmonthnew=CONCAT(@pmonthyr,'-03','-01');

WHILE (@y1<=2020)
BEGIN
--select * from #DeductionList
Select distinct flddesp as [Name] into #DeductionList from test2.dbo.pitother 
declare @tdsdedid int;
declare @ids int;
declare @empcodes int;
declare @grossamount decimal(18,2);
declare @tdsdedname NVARCHAR(500);

update new_num set last_num=(case when (select max(id)+1 from pr_emp_perdeductions) is null then 1 else (select max(id)+1 from pr_emp_perdeductions) end) where table_name = 'pr_emp_perdeductions'
set @ids=(SELECT last_num+1 from new_num where table_name='pr_emp_perdeductions')

WHILE exists(SELECT * from #DeductionList)
begin
set @tdsdedname=(SELECT Top 1 [Name] from #DeductionList)

SELECT p.eid,m.Empid,(SELECT top 1 id from pr_deduction_field_master where name=@tdsdedname and type='per_ded')as m_id,'per_ded'as m_type,p.amount as amount,Concat('Section',p.SecCode) as section, 0 as active, 1000 as trans_id into #DeductionInsert from test2.dbo.pitother p join test2.dbo.pempmast m on p.eid=m.eid where EdFlag='D' and FldDesp=@tdsdedname and FinYear=@yr
print @tdsdedname;
print @yr;
	WHILE exists (SELECT * from #DeductionInsert)
	BEGIN

	CREATE table #Empids (Empid int,Gross decimal(18,2))

	Insert into #Empids (Empid,Gross)SELECT Top 1 Empid,amount from #DeductionInsert

	WHILE exists (SELECT * from #Empids)
	BEGIN
	set @empcodes=(SELECT top 1 Empid from #Empids)
	set @grossamount =(SELECT top 1 Gross from #Empids)
	Insert into pr_emp_perdeductions ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[section],[active],[trans_id])SELECT @ids,@pmonthyr,@pmonthnew,[eid],[Empid],[m_id],[m_type],[amount],[section],[active],[trans_id] FROM #DeductionInsert where Empid=@empcodes and amount=@grossamount

	set @ids=@ids+1;
	print 'Inserted into pr_emp_perdeductions IF';
	print @empcodes;
	print @tdsdedname;
	print @yr;
	Delete from #DeductionList where name =@tdsdedname;
	DELETE from #Empids where Empid=@empcodes and Gross=@grossamount;
	update new_num set last_num=@ids where table_name='pr_emp_perdeductions';
	END

	DROP table #Empids;
	DELETE from #DeductionInsert where Empid=@empcodes and amount=@grossamount;
	END
	DROP table #DeductionInsert;

DELETE from #DeductionList where name=@tdsdedname
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
DROP Table #DeductionList
END

--delete from #DeductionList
update new_num set last_num=@ids where table_name='pr_emp_perdeductions';
update pr_emp_perdeductions set active=1 where fm=(select fm from pr_month_details where active=1)
--select * from pr_emp_perdeductions where fm='2020-09-01'
--exec sp_pr_section_deductions
