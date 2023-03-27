# Pasing Script for Debug result
# Hee Dong Yang (heedong@kaist.ac.kr)

DEBUG_BEFORE_PATH = './debug-output/before_snapshot.log'
DEBUG_AFTER_PATH = './debug-output/after_snapshot.log'
conStr = ['PC', 'MSR', 'LR', 'R0', 'R1', 'R2', 'R3', 'R4', 'R5', 'R6',
          'R7', 'R8', 'R9', 'R10', 'R11', 'R12', 'R13', 'R14', 'R15']


def log_parsing(path):
    registers = {}
    f = open(path, "r")
    for line in f.readlines():
        # print (line)
        words = line.replace('*', "").split()
        try:
            if words[0] in conStr:
                registers[words[0]] = words[1]
                registers[words[2]] = words[3]
        except:
            continue

    return registers


def main():
    beforeRegs = log_parsing(DEBUG_BEFORE_PATH)
    afterRegs = log_parsing(DEBUG_AFTER_PATH)
    print("Before >")
    print(beforeRegs)
    print("After > ")
    print(afterRegs)
    print("Diff:")
    for key in beforeRegs:
        if key in afterRegs and beforeRegs[key] != afterRegs[key]:
            print(f"Register '{key}': '{beforeRegs[key]}' -> '{afterRegs[key]}'")


if __name__ == '__main__':
    main()
