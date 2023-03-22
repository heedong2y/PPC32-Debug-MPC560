#include "MPC5606B.h"

int main() {
    int a = 10, b = 20, c;
    asm volatile (
        "mullw %0, %1, %2\n\t" // multiply a and b, result in c
        : "=r" (c) // output operand
        : "r" (a), "r" (b) // input operands
        : // no clobbered registers
    );

    return 0;
}