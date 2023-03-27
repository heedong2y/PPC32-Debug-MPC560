### Powershell Script for PPC ELF make
### Hee Dong Yang (heedong@kaist.ac.kr)

$source = Get-Location
$file = "tool.tar"
$url = "Path/to/"


# Check if file exists in current directory
if (Test-Path "$source\Tools") {

	& .\Tools\eclipse\ecd.exe build -data $source\workdir\ -project $source\workdir\ppc32-cpu-test --config RAM -cleanBuild
	cp .\Tools\gnu\bin\make.exe .\workdir\ppc32-cpu-test\RAM
	
}
else {
	
	# Invoke-WebRequest -Uri $url -OutFile .\$file

	tar -xvf "$source\$file" -C $source

	& .\Tools\eclipse\ecd.exe build -data $source\workdir\ -project $source\workdir\ppc32-cpu-test --config RAM -cleanBuild
	
	cp .\Tools\gnu\bin\make.exe .\workdir\ppc32-cpu-test\RAM

}
