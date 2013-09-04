set OLDDIR=%CD%
chdir /d %~dp0

echo copy d3d*.dll %1
copy d3d*.dll %1

chdir /d %OLDDIR%