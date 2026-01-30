CREATE procedure sp_dm_jaiib_caiib @empcode int, @yearmon int as 

create table #inc (fy int, fm date, eid int, Empcode int, basic_before_inc float, incr_incen_type int, no_of_inc int,Incrementamt float,
incrementdate date,incr_WEF_date date,  active int, authorisation  int ,trans_id int);

declare @res date;
set @res=(select CAST((convert(date,convert(date,left(@yearmon,4)+'-'+right(@yearmon,2)+'-01'),102)) as varchar));


insert into #inc select 
case when (month(cast(inc.incdatewef as date))>=04 ) then Year(cast(inc.incdatewef as date))+1 else year(cast(inc.incdatewef as date)) end  fy,
cast(inc.incdatewef as date) fm,
inc.eid,
mas.Empid,
basicpbinc as basic_before_inc,
incrince as incr_incen_type,
Noofinc as no_of_inc,
incamount as Incrementamt,
cast(inc.Incdate as date) as incrementdate,
cast(incdatewef as date) as incr_WEF_date,'1' as active, '1' as authorisation ,'101' as trans_id
 from [TEST2].[dbo].pempincr inc join [TEST2].[dbo].PEMPMAST mas on inc.eid=mas.Eid 
 where incrince in (218,219) and  year(incdatewef)=year(@res) and month(incdatewef)=month(@res)
 and mas.empid=@empcode ;

 declare @t int;

 set @t =(select top 1 id from pr_emp_jaib_caib_general   order by id desc)

 if @t is null
 begin set @t=0 end;

 insert into pr_emp_jaib_caib_general select @t+1,
fy,
fm,
eid,
empcode,
basic_before_inc,
case when incr_incen_type=218 then 'CAIIB' else 'JAIIB' end,
no_of_inc,
Incrementamt,
incrementdate,
incr_WEF_date,
active,
authorisation,
trans_id from #inc;

declare @id int;

set @id=(select top 1 id from pr_emp_jaib_caib_general order by id desc);

if @id is Null
begin set @id=0
end

update new_num set last_num=@id where table_name='pr_emp_jaib_caib_general';

drop table #inc;


--exec sp_dm_jaiib_caiib 868,201905
--select * from pr_emp_jaib_caib_general   order by id desc
--select * from new_num  where table_name='pr_emp_jaib_caib_general';
