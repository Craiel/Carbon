set OLDDIR=%CD%
chdir /d %~dp0

rem Copy everything over
xcopy /E /I /R /F /Y %~dp0\. %1

rem Clean out some things we don't want to be deployed
del %1\Deploy.bat
del %1\*.cedl

chdir /d %OLDDIR%