-- Create gen_new_transaction procedure

Create procedure [dbo].[gen_new_transaction]
@user_id int='',
	@user_init nvarchar(30)='',
   @app_name    varchar(80) = 'isql',
   @app_revision varCHAR(20)=NULL,
   @wstation VARCHAR(20)=''
As
declare @anewnum   int
declare @rowcount  int
declare @loginame  varchar(30)
   if @app_name = ''
   begin
      print 'You must give a NON-EMPTY string for the argument @app_name'
      return 1
   end
	--work station
	IF @wstation=''
		SELECT @wstation= hostname from master..sysprocesses where spid = @@spid
	--user init
	IF @user_init=''
	BEGIN
		select @loginame = loginame from master..sysprocesses where spid = @@spid
		select @user_init = user_init    from users    WHERE logon_id = @loginame
		if @user_init is null  select @user_init = @loginame	
		set @user_id = 0	
	END
   begin tran
      exec get_new_num 'transaction_tbl'
      select @anewnum = last_num 
      	from new_num    
      	WHERE table_name='transaction_tbl'
      insert into transaction_tbl values (@anewnum,  @user_id,  @user_init, getdate(), @app_name, @app_revision, @wstation,@@spid)
      select @rowcount = @@rowcount
      if (@@rowcount = 1)
      begin
         commit tran
         print 'new transaction_tbl record was created successfully'
         print @anewnum
         return 0
      end
      else
      begin
         rollback tran
         print 'failed to create a new transaction_tbl record.'
         return 1
      end