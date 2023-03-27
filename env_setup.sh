#!/bin/bash

source=$(pwd)
file="tool.tar"
url="Path/to/"

if [ -d "$source/Tools" ]; then
    ./Tools/eclipse/ecd.exe build -data "$source/workdir/" -project "$source/workdir/ppc32-cpu-test" --config RAM -cleanBuild
    cp Tools/gnu/bin/make.exe workdir/ppc32-cpu-test/RAM/
else
    # wget $url
    tar -xvf "$source/$file" -C "$source"
    ./Tools/eclipse/ecd.exe build -data "$source/workdir/" -project "$source/workdir/ppc32-cpu-test" --config RAM -cleanBuild
    cp Tools/gnu/bin/make.exe workdir/ppc32-cpu-test/RAM/
fi
