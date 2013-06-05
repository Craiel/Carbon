set OLDDIR=%CD%

SET VERSIONROOT=%1\..\..\..\CarbonExt\protobuf-csharp-port\2.4.1.521\tools
SET CPPTARGET=%1\CPP
SET SHARPTARGET=%1\C#

echo Regenerating Carbon Protocol...
echo Working Dir:  %CD%
echo Version Root: %VERSIONROOT%
echo CPP:          %CPPTARGET%
md %CPPTARGET%
echo C#:           %SHARPTARGET%
md %SHARPTARGET%


chdir /d %VERSIONROOT%

protoc.exe --cpp_out=%CPPTARGET% -I ../protos ../protos/google/protobuf/descriptor.proto
protoc.exe --cpp_out=%CPPTARGET% -I ../protos ../protos/google/protobuf/csharp_options.proto
protoc.exe --descriptor_set_out=%1\obj\network.bin --include_imports -I %1 -I ../protos --cpp_out=%CPPTARGET% %1\Protobuf\network.proto
protoc.exe --descriptor_set_out=%1\obj\resource.bin --include_imports -I %1 -I ../protos --cpp_out=%CPPTARGET% %1\Protobuf\resource.proto
protoc.exe --descriptor_set_out=%1\obj\userinterface.bin --include_imports -I %1 -I ../protos --cpp_out=%CPPTARGET% %1\Protobuf\userinterface.proto
ProtoGen.exe -output_directory=%SHARPTARGET% %1\obj\network.bin
ProtoGen.exe -output_directory=%SHARPTARGET% %1\obj\resource.bin
ProtoGen.exe -output_directory=%SHARPTARGET% %1\obj\userinterface.bin

echo --------------------------------
echo Protocol Generation complete

chdir /d %OLDDIR%