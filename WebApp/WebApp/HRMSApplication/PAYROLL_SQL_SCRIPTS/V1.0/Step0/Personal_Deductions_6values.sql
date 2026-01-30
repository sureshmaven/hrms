--Inserting below records for all the emloyees with 0 if no record exists in migration,
--'HFC', 'HFC INT', 'PPF', 'INTEREST ON EDUCATION LOAN', 'TAX SAVER FD', 'INFRASTRUCTURE BONDS', 'Interest On NSC (Deduction)'
declare @empcode int;
declare @id int;
declare @ids int;
declare @name nvarchar(100);

update new_num set last_num=(case when (select max(id)+1 from pr_emp_perdeductions) is null then 1 else (select max(id)+1 from pr_emp_perdeductions)end) where table_name='pr_emp_perdeductions';
SELECT distinct empid as emp_code into #Empids from Employees where eid is not null;

while exists(select * from #Empids)
begin

select id,name into #perdedlist from pr_deduction_field_master  where name in('HFC', 'HFC INT', 'PPF', 'INTEREST ON EDUCATION LOAN', 'TAX SAVER FD', 'INFRASTRUCTURE BONDS', 'Interest On NSC (Deduction)') and type='per_ded';
While exists (select * from #perdedlist)
begin
set @id=(select last_num from new_num where table_name='pr_emp_perdeductions')
set @ids=(select top 1 id from #perdedlist)
set @name=(select top 1 name from #perdedlist)
set @empcode=(select top 1 emp_code from #Empids)

if not exists(select * from pr_emp_perdeductions where emp_code=@empcode and m_id=@ids and active=1)
begin
	Insert into pr_emp_perdeductions([id],[fm],[fy],[emp_id],[emp_code],[m_id],[m_type],[amount],[section],[active],[trans_id])select @id,(select fm from pr_month_details where active=1),(select year(fm)+1 from pr_month_details where active=1),  (select eid from Employees where empid=@empcode),@empcode,@ids,'per_ded',0,'',1,2222;
	set @id=@id+1;
	update new_num set last_num=@id where table_name='pr_emp_perdeductions';
	delete from #perdedlist where name=@name;
end
else
	begin
	update new_num set last_num=@id where table_name='pr_emp_perdeductions';
	delete from #perdedlist where name=@name;
end
end
drop table #perdedlist;
delete from #Empids where emp_code=@empcode;
end
drop table #Empids;