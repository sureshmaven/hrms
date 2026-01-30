for %%G in (*.sql) do sqlcmd /S 40.117.58.249 /d pr_jul10_samanth -U sa -P Reset@123 -i"%%G"


