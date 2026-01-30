CREATE procedure [dbo].[sp_dm_main2]
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

delete from  [dm_emp_mn_task2] where id not in
(select id from [dm_emp_mn_task2] where task  like '%PS%' or  task  like '%ENCASH%' or task  like '%ADHOC%' or task like '%PNCASH%');

	while exists(select top 1 * from dm_emp_mn_task2)
	begin
				select top 1 @pkid = id, @empcode=emp_code, @mn=mn,@task=task from dm_emp_mn_task2;


				if (CHARINDEX('PS', @task)>0)
				begin
				print(concat('2 ', CAST(SYSDATETIME() AS TIME)))
				execute sp_dm_payslip @empcode, @mn
				end

				if (CHARINDEX('ENCASH', @task)>0)
				begin
				print(concat('27 ', CAST(SYSDATETIME() AS TIME)))
				execute sp_dm_encashment @empcode, @mn
				end

				if (CHARINDEX('ADHOC', @task)>0)
				begin
				print(concat('28 ', CAST(SYSDATETIME() AS TIME)))
				execute sp_dm_adhoc @empcode, @mn
				end

				if (CHARINDEX('PNCASH', @task)>0)
				begin
				print(concat('29 ', CAST(SYSDATETIME() AS TIME)))
				execute sp_dm_encashment_present @empcode, @mn
				end
                delete from dm_emp_mn_task2 where id = @pkid;
	end



end



--exec sp_dm_main;
