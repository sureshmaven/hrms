---New Table related to Bond and Investments
CREATE Table pr_PinvstMast(
Fid int,
Fno NVARCHAR(50),
DOI datetime,
NSecurity NVARCHAR(100),
CRate decimal(18,2),
AmountInvst decimal(18,2),
Addr1 NVARCHAR(500),
Addr2 NVARCHAR(500),
Ph1 NVARCHAR(20),
Ph2 NVARCHAR(20),
DMonth NVARCHAR(100),
CAT int,
DOM NVARCHAR(100),
Dop NVARCHAR(100),
IntDt NVARCHAR(100),
InvstDueDt Datetime,
STLT int,
Invstclose NVARCHAR(100),
CumAmountInvstPaid Decimal(18,2)
)

--exec sp_get_pr_PinvstMast
CREATE Procedure sp_get_pr_PinvstMast as
Begin
SELECT * into #pr_PinvstMast from test2.dbo.PinvstMast
INSERT into pr_PinvstMast([Fid],[Fno],[DOI],[NSecurity],[CRate],[AmountInvst],[Addr1],[Addr2],[Ph1],[Ph2],[DMonth],[CAT],[DOM],[Dop],[IntDt],[InvstDueDt],[STLT],[Invstclose],[CumAmountInvstPaid])
SELECT * from #pr_PinvstMast
DROP table #pr_PinvstMast

SELECT * into #pr_PinvstMast1 from test2.dbo.PinvstMastapr062012beforeupdatefno
INSERT into pr_PinvstMast([Fid],[Fno],[DOI],[NSecurity],[CRate],[AmountInvst],[Addr1],[Addr2],[Ph1],[Ph2],[DMonth],[CAT],[DOM],[Dop],[IntDt],[InvstDueDt],[STLT],[Invstclose],[CumAmountInvstPaid])
SELECT * from #pr_PinvstMast1
DROP table #pr_PinvstMast1
End


