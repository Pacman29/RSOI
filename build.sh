#!/bin/sh
cd $TRAVIS_BUILD_DIR/RSOI.Test
dotnet build
dotnet test