

CREATE procedure GetLoansEmpData @pmonth varchar(10), @loancode varchar(10) as
declare @empid int;
insert into dm_emp_mn_task
select empid,pmonth,concat(loanid,',') from test2.dbo.PLoan po join test2.dbo.pempmast m on m.Eid=po.Eid 
where PMonth =@pmonth and LoanId=@loancode;
insert into dm_emp_mn_task
select empid,(select distinct pmonth from test2.dbo.PLoan where pmonth=@pmonth),concat(loanid,',') from test2.dbo.Poldloan po join test2.dbo.pempmast m on m.Eid=po.Eid 
where PMonth =@pmonth-1 and LoanId=@loancode 
and (Amount-(LoanRepaid+CumLoanRepaid))=0  
and ((intamount+intaccured)-(IntRepaid+CumIntRepaid))=0 
--and (IntAmount-CumIntRepaid)=0
--and  empid not in (select emp_code from dm_emp_mn_task)
--delete from dm_emp_mn_task where emp_code=452 and mn='202006' and task='HOUS1,'

