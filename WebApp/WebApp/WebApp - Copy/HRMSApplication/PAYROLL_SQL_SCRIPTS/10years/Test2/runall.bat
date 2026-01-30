for %%G in (*.sql) do sqlcmd /S 40.117.58.249 /d test2 -U sa -P Reset@123 -i"%%G"


