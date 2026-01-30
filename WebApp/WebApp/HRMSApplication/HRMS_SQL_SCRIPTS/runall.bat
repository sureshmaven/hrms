for %%G in (*.sql) do sqlcmd /S MSOFT-506\MSSQLSERVER14 /d HRMS171 -U sa -P Mavensoft -i"%%G"


