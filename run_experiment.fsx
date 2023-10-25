// PPC32 Instruction Debug Experiment Script on MPC5606B
// Hee Dong Yang (heedong@kaist.ac.kr)

#r "bin/Release/net7.0/PPC32-Debug-MPC560.dll"

open PPC32DebugMPC560.Utils
open System
open System.IO

let [<Literal>] DebugProgramPath = "C://PEMicro/PKGPPCNEXUSSTARTER/ICDPPCNEXUS.exe"
let [<Literal>] MPC5606bPrefix = "#include \"MPC5606B.h\"\n"
let [<Literal>] MaincPrefix = "int main(void) { \n\t __asm__ ( \n\t\t\""
let [<Literal>] MaincPostfix = "\\n\"\n\t);\n}"


let BaseDir = Directory.GetCurrentDirectory()
let ConfigDir = BaseDir + "/config/"
let MaincPath = BaseDir + "/workdir/ppc32-cpu-test/Sources/main.c"
let MACROPrefix = sprintf "quiet on \n\nREM <File Open>\nhload %s\\workdir\\ppc32-cpu-test\\RAM\\ppc32-cpu-test.elf\ngotil main\nquiet off\n" BaseDir
let PPC32RegName = ["PC"; "CR"; "MSR"; "LR"; "XER"; "CTR"; "R0"; "R1"; "R2"; "R3"; "R4"; "R5"; "R6"; "R7";
                      "R8"; "R9"; "R10"; "R11"; "R12"; "R13"; "R14"; "R15"; "R16"; "R17"; "R18"; "R19"; "R20";
                      "R21"; "R22"; "R23"; "R24"; "R25"; "R26"; "R27"; "R28"; "R29"; "R30"; "R31"]

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


let main () =
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
  //let argStr = "-scriptfile " + macro
  //executeCommand DebugProgramPath argStr true


main()
