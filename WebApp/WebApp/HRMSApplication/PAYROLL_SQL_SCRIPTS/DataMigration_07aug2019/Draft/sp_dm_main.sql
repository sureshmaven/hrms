CREATE procedure [dbo].[sp_dm_main]
as 
begin
declare @pkid integer, @empcode integer, @mn varchar(10);
declare @task varchar(500);

declare @fest integer,
@hladd integer,
@HOUS1 integer,
@HL2A integer,
@HL2BC integer,
@PFLT3 integer,
@PFLT4 integer,
@PFHT1 integer,
@PFHT2 integer,
@VEH4W integer,
@VEH2W integer,
@eid integer,
@pmonth integer;

while exists(select top 1 * from dm_emp_mn_task)
begin
select top 1 @pkid = id, @empcode=emp_code, @mn=mn,@task=task from dm_emp_mn_task;
DECLARE @list varchar(8000);
DECLARE @pos INT;
DECLARE @len INT;
DECLARE @value varchar(8000);
set @pos = 0
set @len = 0

if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%,%'))
begin
WHILE CHARINDEX(',', @task, @pos+1)>0
BEGIN
set @len = CHARINDEX(',', @task, @pos+1) - @pos
set @value = SUBSTRING(@task, @pos, @len)
print @value;
if (@value = 'PS')
begin
execute sp_dm_payslip @empcode, @mn
end
--rent details
if (@value = 'RD')
begin
execute sp_dm_rent @empcode, @mn
end

--personal earnings
if (@value = 'PE')
begin
execute sp_dm_per_earn @empcode, @mn
end	
--personal deductions
if (@value = 'PD')
begin
execute sp_dm_per_ded @empcode, @mn
end

if (@value = 'TDS')
begin
execute sp_dm_tax_source @empcode, @mn
end
if (@value = 'PETDS')
begin
execute sp_dm_Exgraticas_tds @empcode, @mn
end
if (@value = 'OTDS')
begin
execute sp_dm_Other_tds @empcode, @mn
end
if (@value = 'LOAN')
begin
execute sp_dm_sub_loans @empcode, @mn
end	
if (@value = 'OB')
begin
execute sp_dm_obshare_main @empcode, @mn
end	
if (@value = 'NRL')
begin
execute sp_dm_pfnonrepayable  @empcode, @mn
end

if (@value = 'PFOPEN')
begin
execute sp_dm_pfcontributioncard @empcode, @mn
end	

if (@value = 'FEST')
begin
execute sp_dm_loan_fest @empcode, @mn
end	

if (@value = 'PFHT1')
begin
execute sp_dm_loan_pfloans @empcode, @mn
end

if (@value = 'PFHT2')
begin
execute sp_dm_loan_pfloans_ht2 @empcode, @mn
end		

set @pos = CHARINDEX(',', @task, @pos+@len) +1
END
			end
else
begin
if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%PS%'))
begin
execute sp_dm_payslip @empcode, @mn
end			
--rent details
if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%RD%'))
begin
execute sp_dm_rent @empcode, @mn
end		
			
--personal earnings
if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%PE%'))
begin
execute sp_dm_per_earn @empcode, @mn
end	
--personal deductions
if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%PD%'))
begin
execute sp_dm_per_ded @empcode, @mn
end
--tds at source
if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%TDS%'))
begin
execute sp_dm_tax_source @empcode, @mn
end
--exgratia
if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%PETDS%'))
begin
execute sp_dm_Exgraticas_tds @empcode, @mn
end
--other tds deductions
if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%OTDS%'))
begin
execute sp_dm_Other_tds @empcode, @mn
end
if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%LOAN%'))
begin
execute sp_dm_sub_loans @empcode, @mn
end	

if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%OB%'))
begin
execute sp_dm_obshare_main @empcode, @mn
end	


if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%PFOPEN%'))
begin
execute sp_dm_pfcontributioncard @empcode, @mn
end	
if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%NRL%'))
begin
execute sp_dm_pfnonrepayable  @empcode, @mn
end	

if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%FEST%'))
begin
execute sp_dm_loan_fest  @empcode, @mn
end	

if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%PFHT1%'))
begin
execute sp_dm_loan_pfloans  @empcode, @mn
end	

if exists((SELECT * FROM dm_emp_mn_task WHERE task like '%PFHT2%'))
begin
execute sp_dm_loan_pfloans_ht2  @empcode, @mn
end	

end

print(@task);
delete from dm_emp_mn_task where id = @pkid;
end
end