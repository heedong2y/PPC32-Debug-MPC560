// PPC32 Debug Test
// Hee Dong Yang (heedong@kaist.ac.kr)

#r "bin/Release/net7.0/PPC32-Debug-MPC560.dll"

open PPC32DebugMPC560.Utils
open System.IO

let source = Directory.GetCurrentDirectory()
let file = "tool.tar"
let url = "Path/to/"

let toolsExist = Directory.Exists(Path.Combine(source, "Tools"))

if toolsExist then
    executeCommand @".\Tools\eclipse\ecd.exe" (sprintf "build -data %s\workdir\ -project %s\workdir\ppc32-cpu-test --config RAM -cleanBuild" source source) false
    File.Copy(@".\Tools\gnu\bin\make.exe", @".\workdir\ppc32-cpu-test\RAM\make.exe", true)
else
    // executeCommand "Invoke-WebRequest" (sprintf "-Uri %s -OutFile .\%s" url file)
    executeCommand "tar" (sprintf "-xvf %s\%s -C %s" source file source) false

    executeCommand @".\Tools\eclipse\ecd.exe" (sprintf "build -data %s\workdir\ -project %s\workdir\ppc32-cpu-test --config RAM -cleanBuild" source source) false
    File.Copy(@".\Tools\gnu\bin\make.exe", @".\workdir\ppc32-cpu-test\RAM\make.exe", true)
