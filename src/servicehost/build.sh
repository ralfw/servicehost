#!/bin/sh
nuget restore
msbuild /t:"Clean;Rebuild" /p:"Configuration=Release;OutputPath=../dist" /p:Platform="x86"
