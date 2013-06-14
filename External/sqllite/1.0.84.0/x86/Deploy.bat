set OLDDIR=%CD%
chdir /d %~dp0

echo copy SQLite.Interop.dll %1
copy SQLite.Interop.dll %1

chdir /d %OLDDIR%