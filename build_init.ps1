### Powershell Script for PPC ELF make
### Author : Hee Dong Yang (heedong@kaist.ac.kr)

$source = Get-Location
$file = "tool.tar"
$url = "Path/to/"


# Invoke-WebRequest -Uri $url -OutFile .\$file

tar -xvf "$source\$file" -C $source

# Build Project build-clean
& .\Tools\eclipse\ecd.exe build -data $source\workdir\ -project $source\workdir\ppc32-cpu-test --config RAM -cleanBuild