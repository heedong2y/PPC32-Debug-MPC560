// F# Script for PPC ELF make
// Hee Dong Yang (heedong@kaist.ac.kr)

#load "src/Utils.fs"

open PPC32DebugMPC560
open System.IO

let source = Directory.GetCurrentDirectory()
let args = fsi.CommandLineArgs

if args.Length = 1 then
    build source false
elif args.Length > 1 && args.[1] = "clean" then
    build source true
