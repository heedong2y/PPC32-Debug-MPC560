module PPC32DebugMPC560

open System
open System.IO
open System.Diagnostics

let executeCommand (cmd: string) (args: string) =
    let proc = new Process()
    let startInfo = new ProcessStartInfo(cmd, args)
    startInfo.RedirectStandardOutput <- true
    startInfo.UseShellExecute <- false
    startInfo.CreateNoWindow <- false

    proc.StartInfo <- startInfo
    proc.Start() |> ignore

    let output = proc.StandardOutput.ReadToEnd()
    printfn "%s" output

    proc.WaitForExit()

let args = fsi.CommandLineArgs