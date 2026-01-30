-- Month data insertion

declare @i int;
set @i=1
while @i<=11
begin

Declare @todaydate date;
Declare @fm date;
Declare @id int;
Declare @trans int;
Declare @fy int;
Declare @getyear1 int;
Declare @getyear2 int;

set @todaydate=(select convert(varchar(10),GETDATE(),120));
set @fm=(select top(1) fm from pr_incometax_bank_payment order by fm desc);
set @id=(select top(1) id from pr_incometax_bank_payment order by id desc);
set @trans=(select top(1) trans_id from pr_incometax_bank_payment order by trans_id desc);
set @fy=(select top(1) fy from pr_incometax_bank_payment order by id desc);
set @getyear1=(select top(1) month(fm) from pr_incometax_bank_payment order by fm desc);

if(@todaydate!=@fm)
begin
Insert into pr_incometax_bank_payment(id,fy,fm,trans_id) values((@id+1),year(@fm),DATEADD(MONTH,1,@fm),(@trans+1))
if(@getyear1=12)
begin
update pr_incometax_bank_payment set fy=(@fy+1) where id=(@id+1);
end
end
set @i=@i+1;
end





