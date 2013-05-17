set OLDDIR=%CD%

SET VERSIONROOT=..\..\..\CarbonExt\protobuf-csharp-port\2.4.1.521\tools
chdir /d %VERSIONROOT%

protoc.exe --cpp_out=%1\CPP -I ../protos ../protos/google/protobuf/descriptor.proto
protoc.exe --cpp_out=%1\CPP -I ../protos ../protos/google/protobuf/csharp_options.proto
protoc.exe --descriptor_set_out=%1\obj\network.bin --include_imports -I %1 -I ../protos --cpp_out=%1\CPP %1\Protobuf\network.proto
protoc.exe --descriptor_set_out=%1\obj\resource.bin --include_imports -I %1 -I ../protos --cpp_out=%1\CPP %1\Protobuf\resource.proto
ProtoGen.exe -output_directory=%1\C# %1\obj\network.bin
ProtoGen.exe -output_directory=%1\C# %1\obj\resource.bin

chdir /d %OLDDIR%