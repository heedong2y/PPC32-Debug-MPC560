module PPC32DebugMPC560

open System
open System.IO
open System.Diagnostics

let executeCommand (cmd: string) (args: string) isWindow =
    let proc = new Process()
    let startInfo = new ProcessStartInfo(cmd, args)
    startInfo.RedirectStandardOutput <- true
    startInfo.UseShellExecute <- false
    startInfo.CreateNoWindow <- isWindow

    proc.StartInfo <- startInfo
    proc.Start() |> ignore

    let output = proc.StandardOutput.ReadToEnd()
    printfn "%s" output

    proc.WaitForExit()

let build source isClean =
  if isClean then
      executeCommand ".\\Tools\\eclipse\\ecd.exe" (sprintf "build -data %s\\workdir\\ -project %s\\workdir\\ppc32-cpu-test --config RAM -cleanBuild" source source) false
      File.Copy(".\\Tools\\gnu\\bin\\make.exe", ".\\workdir\\ppc32-cpu-test\\RAM\\make.exe", true)
  else
      Directory.SetCurrentDirectory(Path.Combine(source, "workdir", "ppc32-cpu-test", "RAM"))
      executeCommand ".\\make.exe" "clean" false
      executeCommand ".\\make.exe" "all" false
      Directory.SetCurrentDirectory(source)


// Temporarily set to load a file without generating
let genAsm inputpath =
  File.ReadAllLines(inputpath + "asm.conf")
  |> Array.map (fun line -> line.Trim())

// Temporarily set to load a file without generating
let loadRegInfo inputpath =
  File.ReadAllLines(inputpath + "reg.conf")
  |> Array.map (fun line -> line.Trim())