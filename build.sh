#!/bin/bash
### bash script for PPC ELF make

source=$(pwd)

if [ $# -eq 0 ]
then
	cd ./workdir/ppc32-cpu-test/RAM
	./make.exe clean
	./make.exe all
	cd ../../..
elif [ $1 = "clean" ]
then
	./Tools/eclipse/ecd.exe build -data $source/workdir/ -project $source/workdir/ppc32-cpu-test --config RAM -cleanBuild
	cp ./Tools/gnu/bin/make.exe ./workdir/ppc32-cpu-test/RAM
fi
