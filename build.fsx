// F# Script for PPC ELF make
// Hee Dong Yang (heedong@kaist.ac.kr)

#r "bin/Release/net7.0/PPC32-Debug-MPC560.dll"

open PPC32DebugMPC560.Utils
open System.IO

let source = Directory.GetCurrentDirectory()
let args = fsi.CommandLineArgs

if args.Length = 1 then
    build source false
elif args.Length > 1 && args.[1] = "clean" then
    build source true
