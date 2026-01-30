create procedure [dbo].[sp_month_details](@pstart as varchar(20),@todate as varchar(20))
as
begin

declare @yearend as int;
set @yearend = (select top 1 PMonth from [TEST2].dbo.pset order by PMonth desc);
declare @StartValue int;
set @StartValue=(select last_num from new_num where table_name = 'pr_month_details');
declare @initValue int;
set @initValue = @StartValue;

declare @yearstart as int;
set @yearstart = CAST(@pstart as int);
declare @toyear as int;
set @toyear = CAST(@todate as int);

while (@yearstart <= @toyear)
begin

set @initValue = @initValue+1;
declare @fy NVARCHAR(100);
declare @fyQry NVARCHAR(100);
declare @pmonth NVARCHAR(100);
set @pmonth= cast(@yearstart as varchar(20));

set @fyQry = 'select @fy = dbo.getFY('+@pmonth+');';
execute sp_executesql 
    @fyQry, 
    N'@fy NVARCHAR(100) OUTPUT', 
    @fy = @fy output;


declare @fm NVARCHAR(MAX);
set @fm = cast(left(@yearstart,4) as varchar(20))+'-'+ right(@yearstart,2)+'-'+'01';

IF EXISTS (select * from [TEST2].dbo.pset where MFlag='Y' AND PMonth=@yearstart)
begin

select @initValue as id, @fy as fy,@fm as fm,Wholy as week_holidays,
Pholy as paid_holidays, DA_Slabs as da_slabs, DA_Points as da_points,
DA_Percent as da_percent, 0 as active, 1000 as trans_id, PayDays as month_days,
0 as is_interest_calculated, 0 as interest_percent into #ob_share_dummy
from test2.dbo.PSet where PMonth=@yearstart;

insert into pr_month_details (id,fy,fm,week_holidays, paid_holidays,da_slabs,da_points,da_percent,active,trans_id,month_days,
is_interest_calculated,interest_percent) select * from #ob_share_dummy;

drop table #ob_share_dummy;
end
IF EXISTS (select * from [TEST2].dbo.pset where MFlag='N' AND PMonth=@yearstart)
begin

select @initValue as id, @fy as fy,@fm as fm,wholy as week_days,Pholy as paid_holidays,
DA_Slabs as da_slabs, DA_Points as da_points,DA_Percent as da_percent,1 as active,1000 as trans_id,paydays as month_days,0 as is_interest_calculated,0 as interest_percent
 into #month_details from test2.dbo.PSet where PMonth=@yearstart;
 insert into pr_month_details (id,fy,fm,week_holidays,paid_holidays,da_slabs,da_points,da_percent,active,trans_id,month_days,is_interest_calculated,interest_percent) select * from #month_details;
 drop table #month_details;
end
set @yearstart = @yearstart + 1;
end
--last num update
declare @lastnum int;
set @lastnum = (select top 1 id from pr_month_details order by id desc)
update new_num set last_num=@lastnum where table_name='pr_month_details';
update pr_month_details set active=1 where fm in (select top 1 fm from pr_month_details order by fm desc);
print @todate
end


--exec sp_month_details '201904'
--select * from pr_month_details;
--delete from pr_month_details;
--select * from test2.dbo.PSet order by  PMonth desc;

