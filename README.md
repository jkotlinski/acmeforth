# Hack n' Trade Forth Compiler

## What?

HTFC is a cross-compiling 16-bit Forth targeting the Commodore C64, using ACME for assembling.
All Forth 2012 Core and Core Extension words are supported.

## Why?

I want a C64 Forth setup that gives

 * Fast builds
 * Lean output (no dictionary or unused words)
 * Full macro support with CREATE/DOES>
 * Convenient text editing on PC

## Usage Example

`COMPILE <word> <outfile.asm>` compiles a word and its dependencies to outfile.asm.

	./htfc.py examples/colorcycle.fs
	acme -o colorcycle.prg -f cbm colorcycle.asm

...or, `sh run_tests.sh` to run the unit tests.

Assembly code can be inlined with `:code` `;code`.

## Cross-Compiling Gotchas

### HERE

HERE only works in the target data space. When running the below code on C64, FOO will return the address of the 1 on the host, which is obviously wrong.

	HERE 1 , CONSTANT FOO

The below code is fine. BAR will point to 1 on both host and target.

	CREATE BAR 1 ,
