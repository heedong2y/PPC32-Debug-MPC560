#load "src/Utils.fs"
// F# Script for PPC ELF make
// Hee Dong Yang (heedong@kaist.ac.kr)

open PPC32DebugMPC560
open System.IO

let source = Directory.GetCurrentDirectory()

if args.Length = 1 then
    Directory.SetCurrentDirectory(Path.Combine(source, "workdir", "ppc32-cpu-test", "RAM"))
    executeCommand ".\\make.exe" "clean"
    executeCommand ".\\make.exe" "all"
    Directory.SetCurrentDirectory(source)
elif args.Length > 1 && args.[1] = "clean" then
    executeCommand ".\\Tools\\eclipse\\ecd.exe" (sprintf "build -data %s\\workdir\\ -project %s\\workdir\\ppc32-cpu-test --config RAM -cleanBuild" source source)
    File.Copy(".\\Tools\\gnu\\bin\\make.exe", ".\\workdir\\ppc32-cpu-test\\RAM\\make.exe", true)
