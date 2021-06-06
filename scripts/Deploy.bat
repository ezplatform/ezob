@echo off 

echo "******************EZOB DEPLOYMENT*******************

set AppFolder=C://temp
set LiveFolder=%AppFolder%/ezob-base-app

for /f "delims=" %%a in ('wmic OS Get localdatetime ^| find "."') do set DateTime=%%a
set Yr=%DateTime:~0,4%
set Mon=%DateTime:~4,2%
set Day=%DateTime:~6,2%
set Hr=%DateTime:~8,2%
set Min=%DateTime:~10,2%
set Sec=%DateTime:~12,2%
set BackupFolder=%AppFolder%/backup/ezob-base-app__%Yr%-%Mon%-%Day%_(%Hr%-%Min%-%Sec%)

if exist "%LiveFolder%/" (
	mkdir "%BackupFolder%"
	echo - Start backup ezob app to %BackupFolder%
	copy "%LiveFolder%" "%BackupFolder%"
) 

if not exist "%LiveFolder%/" (
    echo No backup action for the first deployment.
)

echo - Start publish ezob app
dotnet publish -c Release -r win10-x64 ../ezob.sln --self-contained false -o %LiveFolder%

echo   ****************************************************
echo              /```````````````/\  /``````````````/\
echo             /   ____________/ / /  ________    / /	
echo            /   /\\\\\\\\\\\\\/ /__/\\\\\\\/   /\/       					
echo           /   /\/_________     \__\/    _/ __/\/
echo          /   /           /\          __/  /\\\/
echo         /   /\\\\\\\\\\\\\/       __/  __/\/
echo        /   /_/_________     ____/     /\\\/
echo       /               /\   /````      `````/\
echo      /_______________/ /  /_______________/ /	
echo      \\\\\\\\\\\\\\\\\/   \\\\\\\\\\\\\\\\\/ 
echo  *****************************************************
echo                        Finished! 
pause