CREATE procedure [dbo].[sp_dm_main3]
as 
begin
declare @pkid integer, @empcode integer, @mn varchar(10);
declare @task varchar(500);

declare @fest integer,
@HLADD integer,
@HOUS1 integer,
@HL2A integer,
@HL2BC integer,
@PFLT2 integer,
@PFLT3 integer,
@PFLT4 integer,
@PERS integer,
@PFHT1 integer,
@PFHT2 integer,
@VEH4W integer,
@VEH2W integer,
@eid integer,
@pmonth integer;
print(concat('1 ', CAST(SYSDATETIME() AS TIME)))

while exists(select top 1 * from dm_emp_mn_task3)
begin
select top 1 @pkid = id, @empcode=emp_code, @mn=mn,@task=task from dm_emp_mn_task3;
DECLARE @list varchar(8000);
DECLARE @pos INT;
DECLARE @len INT;
DECLARE @value varchar(8000);
set @pos = 0
set @len = 0

if (CHARINDEX(',', @task)>0)
begin
WHILE CHARINDEX(',', @task, @pos+1)>0
BEGIN
set @len = CHARINDEX(',', @task, @pos+1) - @pos
set @value = SUBSTRING(@task, @pos, @len)
print @value;

--rent details
if (@value = 'RD')
begin
print(concat('3 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_rent @empcode, @mn
end

--personal earnings
--if (@value = 'PE')
--begin
--print(concat('4 ', CAST(SYSDATETIME() AS TIME)))
--execute sp_dm_per_earn @empcode, @mn
--end	
--personal deductions
if (@value = 'PD')
begin
print(concat('5 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_per_ded @empcode, @mn
end

if (@value = 'TDS')
begin
print(concat('6 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_tax_source @empcode, @mn
end
if (@value = 'PETDS')
begin
print(concat('7 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_Exgraticas_tds @empcode, @mn
end
if (@value = 'OTDS')
begin
print(concat('8 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_Other_tds @empcode, @mn
end

if (@value = 'OB')
begin
print(concat('10 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_obshare_main @empcode, @mn
end	
if (@value = 'IIB')
begin
print(concat('11 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_jaiib_caiib @empcode, @mn
end	
if (@value = 'NRL')
begin
print(concat('12 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_pfnonrepayable  @empcode, @mn
end

-- if (@value = 'PFOPEN')
-- begin
-- execute sp_dm_pfcontributioncard @empcode, @mn
-- end	

if (@value = 'PROM')
begin
print(concat('13 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_promotions @empcode, @mn
end	



set @pos = CHARINDEX(',', @task, @pos+@len) +1
END
			end
else
begin

set @value = @task

--rent details
if (@value = 'RD')
begin
execute sp_dm_rent @empcode, @mn
end

--personal earnings
--if (@value = 'PE')
--begin
--execute sp_dm_per_earn @empcode, @mn
--end	
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

if (@value = 'OB')
begin
execute sp_dm_obshare_main @empcode, @mn
end	
if (@value = 'IIB')
begin
execute sp_dm_jaiib_caiib @empcode, @mn
end	
if (@value = 'NRL')
begin
execute sp_dm_pfnonrepayable  @empcode, @mn
end

-- if (@value = 'PFOPEN')
-- begin
-- execute sp_dm_pfcontributioncard @empcode, @mn
-- end	

if (@value = 'PROM')
begin
execute sp_dm_promotions @empcode, @mn
end	





end

print(@task);
delete from dm_emp_mn_task3 where id = @pkid;
end
end


--exec sp_dm_main;
