CREATE procedure [dbo].[sp_dm_main1]
as 
begin
declare @pkid integer, @empcode integer, @mn varchar(10);
-- loan related processing
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

while exists(select top 1 * from dm_emp_mn_task1)
begin
select top 1 @pkid = id, @empcode=emp_code, @mn=mn,@task=task from dm_emp_mn_task1;
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


if (@value = 'LOAN')
begin
print(concat('9 ', CAST(SYSDATETIME() AS TIME)))
execute sp_dm_sub_loans @empcode, @mn
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


set @pos = CHARINDEX(',', @task, @pos+@len) +1
END
			end
else
begin

set @value = @task


if (@value = 'LOAN')
begin
execute sp_dm_sub_loans @empcode, @mn
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





end

print(@task);
delete from dm_emp_mn_task1 where id = @pkid;
end
end


--exec sp_dm_main;
