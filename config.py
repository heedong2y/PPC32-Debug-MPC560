import os
import subprocess
#
# PPC32 Instruction Debug Experiment Script on MPC5606B (using Windows Git bash)
#

BASE_DIR = os.getcwd()
CONFIG_DIR = BASE_DIR + "/config/"
MAINCODE_PATH = BASE_DIR + "/workdir/ppc32-cpu-test/Sources/main.c"
DEBUG_PROGRAM_PATH = "C://PEMicro/PKGPPCNEXUSSTARTER/ICDPPCNEXUS.exe"
MACRO_PREFIX = 'quiet on \n\nREM <File Open>\nhload {}\\debug-output\\ppc32-cpu-test.elf\ngotil main\nquiet ' \
               'off\n\nREM <Modify Register value>\n'.format(BASE_DIR)
MPC5606B_PREFIX = '#include "MPC5606B.h"\n'
MAIN_PREFIX = 'int main(void) { \n\t __asm__ ( \n\t\t"'
MAIN_POSTFIX = '\\n"\n\t);\n}'

# Temporarily set to load a file without generating
def gen_asm(inputpath):
    asm_file = open(inputpath + "asm.conf", "r")
    inline_code = [line.rstrip() for line in asm_file.readlines()]
    return inline_code

# Temporarily set to load a file without generating
def load_reg_info(inputpath):
    reg_file = open(inputpath + "reg.conf", "r")
    reg_info = [line.rstrip() for line in reg_file.readlines()]
    return reg_info

def decide_outdir():
    prefix = "exp"
    i = 0
    while True:
        i += 1
        outdir = os.path.join(BASE_DIR, "output", "%s-%d" % (prefix, i))
        if not os.path.exists(outdir):
            return outdir

# main.c
def gen_mainc(inline_code, outdir):
    boardMain = MPC5606B_PREFIX + MAIN_PREFIX + '\\n" \n\t\t"'.join(inline_code) + MAIN_POSTFIX
    # qemuMain = + MAIN_PREFIX + '\\n" \n\t\t"'.join(inline_code) + MAIN_POSTFIX
    with open(outdir + "/main.c", "w") as f1:
        f1.write(boardMain)


    print("\n[*] `Main.c` is generated")subprocess.run(['cp', outdir + "/main.c", MAINCODE_PATH], stdout=subprocess.PIPE)
    # with open("qemu.c", "w") as f2:
    #     f2.write(qemuMain)

# debug macro
def gen_macro(set_regs, inline_code, outdir):
    macro_code = list()
    macro_code.append(MACRO_PREFIX)
    macro_code.append('\n'.join(set_regs))
    macro_code.append('\n\nREM <Main Start>\nlogfile {}\\sanpshot0.log\nsnapshot\nlf'.format(outdir))
    for i, code in enumerate(inline_code):
        macro_code.append(
            '\n\nREM <Inst> {0}\nlogfile {1}\\sanpshot{2}.log\nstep\nsnapshot\nlf'.format(code, outdir, i + 1))
    macro_code.append('\n\nquit')
    # file write
    with open(outdir + "/debug.mac", "w") as f3:
        f3.writelines(macro_code)

def main():
    inline_code = gen_asm(CONFIG_DIR)
    set_regs = load_reg_info(CONFIG_DIR)

    outdir = decide_outdir()
    os.makedirs(outdir)
    gen_mainc(inline_code, outdir)
    gen_macro(set_regs, inline_code, outdir)
    os.system('sh build.sh')

if __name__ == '__main__':
    main()
