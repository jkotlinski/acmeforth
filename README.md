# Hack n' Trade Forth Compiler

## What?

HTFC (name pending) is a cross-compiling Forth, targeting the Commodore C64.
It uses ACME for assembling.

The host supports all Forth 2012 Core and Core Extension words.
The target supports all Core and Core Extension words, except those that are for interpreting and compiling.

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
