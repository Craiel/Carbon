set OLDDIR=%CD%
chdir /d %~dp0

rem Copy everything over
echo xcopy %~dp0\* %1\*

rem Clean out some things we don't want to be deployed
del %1\Deploy.bat

chdir /d %OLDDIR%