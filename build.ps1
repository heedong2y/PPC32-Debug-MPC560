### Powershell Script for PPC ELF make
### Hee Dong Yang (heedong@kaist.ac.kr)

$source = Get-Location


if ($args.Count -eq 0) {
    cd .\workdir\ppc32-cpu-test\RAM
	.\make.exe clean
	.\make.exe all
	cd ..\..\..\
}
elseif ($args[0] -eq "clean") {
	& .\Tools\eclipse\ecd.exe build -data $source\workdir\ -project $source\workdir\ppc32-cpu-test --config RAM -cleanBuild
	cp .\Tools\gnu\bin\make.exe .\workdir\ppc32-cpu-test\RAM
}
