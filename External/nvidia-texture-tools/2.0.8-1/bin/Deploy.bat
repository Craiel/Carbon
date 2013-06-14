set OLDDIR=%CD%
chdir /d %~dp0

mkdir %1
echo copy *.dll %1
copy *.dll %1
echo copy *.exe %1
copy *.exe %1

chdir /d %OLDDIR%