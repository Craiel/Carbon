set OLDDIR=%CD%
chdir /d %1

protoc.exe --cpp_out=../../../Carbon.Server descriptor.proto
protoc.exe --cpp_out=../../../Carbon.Server csharp_options.proto
protoc.exe --descriptor_set_out=network.bin --include_imports --cpp_out=../../../Carbon.Server network.proto
ProtoGen.exe -output_directory=../ network.bin

chdir /d %OLDDIR%