#!/bin/bash
NUGET_PROT=~/.nuget/packages/grpc.tools/1.15.0/tools/linux_x64
$NUGET_PROT/protoc -I../ProtoFiles --csharp_out GRPC   ../ProtoFiles/services.proto --grpc_out GRPC --plugin=protoc-gen-grpc=$NUGET_PROT/grpc_csharp_plugin