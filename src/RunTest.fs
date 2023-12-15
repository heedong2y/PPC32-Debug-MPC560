module PPC32DebugMPC560.Run

open Utils
open System
open System.IO
open System.Reflection

let [<Literal>] DebugProgramPath = "C://PEMicro/PKGPPCNEXUSSTARTER/ICDPPCNEXUS.exe"
let [<Literal>] MPC5606bPrefix = "#include \"MPC5606B.h\"\n"
let [<Literal>] MaincPrefix = "int main(void) { \n\t __asm__ ( \n\t\t\""
let [<Literal>] MaincPostfix = "\\n\"\n\t);\n}"


let FindProjectFileDir () =

    let entryAssembly = Assembly.GetEntryAssembly()

    let assemblyLocation =
        match entryAssembly with
        | null -> failwith "Entry assembly not found"
        | asm -> asm.Location

    let rec findProjectDir path =
        let fsprojPath = Path.Combine(path, "*.fsproj")
        if Directory.GetFiles(path, "*.fsproj").Length > 0 then
            path
        else
            let parent = Directory.GetParent(path)
            match parent with
            | null -> failwith "Project file not found"
            | _ -> findProjectDir parent.FullName

    findProjectDir (Path.GetDirectoryName(assemblyLocation))

let BaseDir = FindProjectFileDir ()
let ConfigDir = BaseDir + "/config/"
let MaincPath = BaseDir + "/workdir/ppc32-cpu-test/Sources/main.c"
let MACROPrefix = sprintf "quiet on \n\nREM <File Open>\nhload %s\\workdir\\ppc32-cpu-test\\RAM\\ppc32-cpu-test.elf\ngotil main\nquiet off\n" BaseDir

let decideOutdir () =
  let prefix = "exp"
  let rec loop i =
      let outdir = Path.Combine(BaseDir, "output", sprintf "%s-%d" prefix i)
      if not (Directory.Exists(outdir)) then
          outdir
      else
          loop (i + 1)
  loop 1

let genMainc (inlineAsm : String[]) outdir =
  let boardMain = MPC5606bPrefix + MaincPrefix + String.Join("\\n\" \n\t\t\"", inlineAsm) + MaincPostfix
  let qemuMain = MaincPrefix + (String.Join("\\n\" \n\t\t\"", inlineAsm)) + MaincPostfix

  // Write to outdir/main.c
  File.WriteAllText(Path.Combine(outdir, "main.c"), boardMain)
  // File.WriteAllText("qemu.c", qemuMain)

  printfn "\n[*] `Main.c` is generated"

  // Copy the file to MAINCODE_PATH
  File.Copy(Path.Combine(outdir, "main.c"), MaincPath, true)

let genMacro (inlineAsm : String[]) outDir =
  let inlineCodeMacros =
      Array.mapi (fun i code -> sprintf "\n\nlogfile %s\\sanpshot%d.log\nREM <Inst> %s\nstep\nsnapshot\nlf" outDir (i+1) code) inlineAsm
  let baseMacros = [| MACROPrefix; sprintf "\n\nlogfile %s\\sanpshot0.log\nREM <Inst>\nsnapshot\nlf" outDir |]
  let endMacro = [|"\n\nquit"|]
  let macroCode = Array.concat [baseMacros; inlineCodeMacros; endMacro]

  let macroPath = Path.Combine(outDir, "debug.mac")
  File.WriteAllLines(macroPath, macroCode)

  printfn "\n[*] macro script is generated"

  macroPath

[<EntryPoint>]

let main =

  let inlineAsm = genAsm ConfigDir

  let outdir = decideOutdir()
  Directory.CreateDirectory(outdir) |> ignore

  /// Make Main.c
  genMainc inlineAsm outdir

  // Make macro file
  let macro = genMacro inlineAsm outdir

  // Complie
  build BaseDir false

  // Run
  let argStr = "-scriptfile " + macro
  executeCommand DebugProgramPath argStr true

  exit 0

