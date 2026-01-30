create procedure a @desig_id int, @pay_period date,@basic int
as

if (select max(basic) from pr_basic_anual_incr_master where 
designation_id =@desig_id and pay_period=@pay_period)>@basic 

begin

select top 1 increment,'annual' as type from pr_basic_anual_incr_master where 
designation_id =@desig_id and pay_period=@pay_period and @basic >=basic order by basic desc
end
else

begin

select top 1 increment,'stagnation' as type from pr_basic_stag_incr_master where 
designation_id =@desig_id and pay_period=@pay_period and @basic >=basic order by basic desc
end
