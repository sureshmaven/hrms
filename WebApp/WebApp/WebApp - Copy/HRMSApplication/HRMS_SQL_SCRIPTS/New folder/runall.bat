for %%G in (*.sql) do sqlcmd /S MSOFT-506\MSSQLSERVER14 /d HRMSPK	 -U sa -P Mavensoft -i"%%G"
pause

