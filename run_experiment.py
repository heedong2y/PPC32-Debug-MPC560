import os
import subprocess

#
# PPC32 Instruction Debug Experiment Script on MPC5606B (using Windows Git bash)
# 

BASE_DIR = os.getcwd()
CONFIG_DIR = BASE_DIR + "/config/"
MAINCODE_PATH = BASE_DIR + "/workdir/ppc32-cpu-test/Sources/main.c"
DEBUG_PROGRAM_PATH = "C://PEMicro/PKGPPCNEXUSSTARTER/ICDPPCNEXUS.exe"
MACRO_PREFIX = 'quiet on \n\nREM <File Open>\nhload {}\\workdir\\ppc32-cpu-test\\RAM\\ppc32-cpu-test.elf\ngotil ' \
               'main\nquiet off\n\nREM <Modify Register value>\n'.format(BASE_DIR)
MPC5606B_PREFIX = '#include "MPC5606B.h"\n'
MAIN_PREFIX = 'int main(void) { \n\t __asm__ ( \n\t\t"'
MAIN_POSTFIX = '\\n"\n\t);\n}'

ppc32_reg_name = ['PC', 'CR', 'MSR', 'LR', 'XER', 'CTR', 'R0', 'R1', 'R2', 'R3', 'R4', 'R5', 'R6', 'R7',
                  'R8', 'R9', 'R10', 'R11', 'R12', 'R13', 'R14', 'R15', 'R16', 'R17', 'R18', 'R19', 'R20',
                  'R21', 'R22', 'R23', 'R24', 'R25', 'R26', 'R27', 'R28', 'R29', 'R30', 'R31']


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


# Generate main.c
def gen_mainc(inline_code, outdir):
    boardMain = MPC5606B_PREFIX + MAIN_PREFIX + '\\n" \n\t\t"'.join(inline_code) + MAIN_POSTFIX
    # qemuMain = + MAIN_PREFIX + '\\n" \n\t\t"'.join(inline_code) + MAIN_POSTFIX
    with open(outdir + "/main.c", "w") as f1:
        f1.write(boardMain)

    print("\n[*] `Main.c` is generated")
    subprocess.run(['cp', outdir + "/main.c", MAINCODE_PATH], stdout=subprocess.PIPE)
    # with open("qemu.c", "w") as f2:
    #     f2.write(qemuMain)


# Generate debugging macro
def gen_macro(set_regs, inline_code, outdir):
    macro_code = list()
    macro_code.append(MACRO_PREFIX)
    macro_code.append('\n'.join(set_regs))
    macro_code.append('\n\nlogfile {}\\sanpshot0.log\nREM <Inst>  \nsnapshot\nlf'.format(outdir))
    for i, code in enumerate(inline_code):
        macro_code.append(
            '\n\nlogfile {1}\\sanpshot{2}.log\nREM <Inst> {0}\nstep\nsnapshot\nlf'.format(code, outdir, i + 1))
    macro_code.append('\n\nquit')
    # file write
    with open(outdir + "/debug.mac", "w") as f3:
        f3.writelines(macro_code)
    print("\n[*] macro script is generated")
    return outdir + "\debug.mac"


# debug logfile parsing
def log_parsing(path):
    registers = {}
    f = open(path, "r")
    for line in f.readlines():
        # print (line)
        words = line.replace('*', "").split()
        try:
            if words[0] in ppc32_reg_name:
                registers[words[0]] = words[1]
                registers[words[2]] = words[3]
            elif words[1] == '<Inst>':
                registers['inst'] = ' '.join(words[2:])
        except:
            continue

    return registers


def write_diff(file, before_regs, after_regs):
    file.write("-------------------------------\n")
    file.write(f"\n[RUN] {after_regs['inst']}\n")
    file.write("\nBefore:\n")
    # write before reg
    for i, reg in enumerate(ppc32_reg_name):
        file.write("{0} = {1} ".format(reg, before_regs[reg]))
        if (i + 1) % 5 == 0:
            file.write("\n")
    # write after reg
    file.write("\n\nAfter:\n")
    for i, reg in enumerate(ppc32_reg_name):
        file.write("{0} = {1} ".format(reg, after_regs[reg]))
        if (i + 1) % 5 == 0:
            file.write("\n")
    file.write("\n\nDiff:\n")
    for key in before_regs:
        if key in after_regs and before_regs[key] != after_regs[key] and key != 'inst':
            file.write(f"{key}: {before_regs[key]} -> {after_regs[key]}\n")


# Generate result file
def get_debug_result(outdir):
    reg_values = []
    for file_name in os.listdir(outdir):
        if file_name.startswith("sanpshot"):
            regs = log_parsing(os.path.join(outdir, file_name))
            reg_values.append(regs)

    with open(outdir + "/result.txt", "w") as file:
        for i in range(len(reg_values) - 1):
            write_diff(file, reg_values[i], reg_values[i + 1])


def main():
    # asm.config/reg.config load
    inline_code = gen_asm(CONFIG_DIR)
    set_regs = load_reg_info(CONFIG_DIR)

    outdir = decide_outdir()
    os.makedirs(outdir)

    # test setting
    gen_mainc(inline_code, outdir)
    macro = gen_macro(set_regs, inline_code, outdir)

    subprocess.run(['sh', 'build.sh'], stdout=subprocess.PIPE)# test binary build
    print("\n[*] test binary is generated")
    subprocess.run([DEBUG_PROGRAM_PATH, "-scriptfile", macro], stdout=subprocess.PIPE)  # debugging
    print("\n[*] debugging complete")
    get_debug_result(outdir)  # debug logfile parsing & report
    print("\n[*] test result : {}\\result.txt".format(outdir))


if __name__ == '__main__':
    main()
