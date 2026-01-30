--Migrating section related information into pr_emp_tds_section_deductions but not into pr_emp_perdeductions so this script will insert all section related records into pr_emp_perdeductions.
declare @id int;
declare @maxid int;
--select * from new_num where table_name='pr_emp_perdeductions';
update new_num set last_num=(case when (select max(id)+1 from pr_emp_perdeductions) is null then 1 else (select max(id)+1 from pr_emp_perdeductions)end) where table_name='pr_emp_perdeductions';

select [id],[fy],[fm],[empid]as [emp_id],[empcode]as [emp_code],[m_id],'per_ded' as [m_type],[gross]as [amount],[section_type]as [section],[active],[trans_id] into #perded from pr_emp_tds_section_deductions where trans_id=1000 order by [empid]

set @id=(select last_num from new_num where table_name='pr_emp_perdeductions')
set @maxid = @id;
update #perded set id = id + @maxid
update #perded set trans_id=1111

Insert into pr_emp_perdeductions Select * FROM #perded;

drop table #perded;
update pr_emp_perdeductions set active=1 where fm=(select fm from pr_month_details where active=1)
update new_num set last_num=(select max(id) from pr_emp_perdeductions) where table_name='pr_emp_perdeductions';

