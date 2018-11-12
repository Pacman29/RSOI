#!/bin/sh
dotnet build
cd $TRAVIS_BUILD_DIR/RSOI.Test
dotnet test