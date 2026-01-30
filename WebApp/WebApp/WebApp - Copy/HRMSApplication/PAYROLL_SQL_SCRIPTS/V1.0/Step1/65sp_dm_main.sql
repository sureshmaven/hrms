CREATE procedure [dbo].[sp_dm_main]
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

while exists(select top 1 * from dm_emp_mn_task)
begin
select top 1 @pkid = id, @empcode=emp_code, @mn=mn,@task=task from dm_emp_mn_task;
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
if (@value = 'PS')
begin
print(concat('2 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_payslip @empcode, @mn
end
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
----personal deductions
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
if (@value = 'LOAN')
begin
print(concat('9 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_sub_loans @empcode, @mn
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


if (@value = 'FEST')
begin
print(concat('14 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_loan_fest @empcode, @mn
end	

if (@value = 'PERS')
begin
print(concat('15 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_loan_personal @empcode, @mn
end	

if (@value = 'PFHT1')
begin
print(concat('16 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_loan_pfloans @empcode, @mn
end

if (@value = 'PFHT2')
begin
print(concat('17 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_loan_pfloans_ht2 @empcode, @mn
end	

if (@value = 'VEH2W')
begin
print(concat('18 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_loan_single_two_wheller @empcode, @mn
     
end		

if (@value = 'HOUS1')
begin
print(concat('19 ', CAST(SYSDATETIME() AS TIME)))
print'HOUS1';
execute sp_dm_sub_loans @empcode, @mn
end		

if (@value = 'VEH4W')
begin
print(concat('20 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_sub_loans_four_wheeler @empcode, @mn
end	

if (@value = 'PFLT2')
begin
print(concat('21 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_loan_pfloans_lt2 @empcode, @mn
end	

if (@value = 'PFLT3')
begin
print(concat('22 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_loan_pfloans_lt3 @empcode, @mn
end

if (@value = 'PFLT4')
begin
print(concat('23 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_loan_pfloans_lt4 @empcode, @mn
end	

if (@value = 'HLADD')
begin
print(concat('24 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_sub_loans_hladd @empcode, @mn
end	

if (@value = 'HL2BC')
begin
print(concat('25 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_sub_loans_hl2bc @empcode, @mn
end

if (@value = 'HL2A')
begin
print(concat('26 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_sub_loans_hl2a @empcode, @mn
end	

if (@value = 'ENCASH')
begin
print(concat('27 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_encashment @empcode, @mn
end

if (@value = 'ADHOC')
begin
print(concat('28 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_adhoc @empcode, @mn
end

if (@value = 'PNCASH')
begin
print(concat('29 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_encashment_present @empcode, @mn
end

set @pos = CHARINDEX(',', @task, @pos+@len) +1
END
			end
else
begin

set @value = @task
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
if (@value = 'LOAN')
begin
execute sp_dm_sub_loans @empcode, @mn
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


if (@value = 'FEST')
begin
execute sp_dm_loan_fest @empcode, @mn
end	

if (@value = 'PERS')
begin
execute sp_dm_loan_personal @empcode, @mn
end	

if (@value = 'PFHT1')
begin
execute sp_dm_loan_pfloans @empcode, @mn
end

if (@value = 'PFHT2')
begin
execute sp_dm_loan_pfloans_ht2 @empcode, @mn
end	

if (@value = 'VEH2W')
begin
execute sp_dm_loan_single_two_wheller @empcode, @mn
end		

if (@value = 'HOUS1')
begin
print'HOUS1';
execute sp_dm_sub_loans @empcode, @mn
end		

if (@value = 'VEH4W')
begin
execute sp_dm_sub_loans_four_wheeler @empcode, @mn
end	

if (@value = 'PFLT2')
begin
execute sp_dm_loan_pfloans_lt2 @empcode, @mn
end	

if (@value = 'PFLT3')
begin
execute sp_dm_loan_pfloans_lt3 @empcode, @mn
end

if (@value = 'PFLT4')
begin
execute sp_dm_loan_pfloans_lt4 @empcode, @mn
end	

if (@value = 'HLADD')
begin
execute sp_dm_sub_loans_hladd @empcode, @mn
end	

if (@value = 'HL2BC')
begin
execute sp_dm_sub_loans_hl2bc @empcode, @mn
end

if (@value = 'HL2A')
begin
execute sp_dm_sub_loans_hl2a @empcode, @mn
end	

if (@value = 'ENCASH')
begin
execute sp_dm_encashment @empcode, @mn
end

if (@value = 'ADHOC')
begin
execute sp_dm_adhoc @empcode, @mn
end

if (@value = 'PNCASH')
begin
execute sp_dm_encashment_present @empcode, @mn
end

end

print(@task);
delete from dm_emp_mn_task where id = @pkid;
end
end


--exec sp_dm_main;
