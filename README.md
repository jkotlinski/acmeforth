# Hack n' Trade Forth Compiler

## What?

HTFC (name pending) is a cross-compiling Forth, targeting the Commodore C64.
It uses ACME for assembling.

### Usage Example

`COMPILE <word> <outfile.asm>` compiles a word, and all its dependencies, to outfile.asm.

	./htfc.py examples/colorcycle.fs
	acme -o colorcycle.prg -f cbm colorcycle.asm

...or, `sh run_tests.sh` to run the unit tests.

Assembly code can be inlined with `:code` `;code`.

## Cross-Compiling Gotchas

### Sizes Differ Between Host and Target

Host uses 32-bit address units and cells, while target uses 8-bit address units and 16-bit cells.
Keep in mind if you are programming for the host or the target!

### HERE cannot be relied on

	HERE 1 , CONSTANT FOO

When cross compiled, FOO will no longer point to the 1, since the memory address will not translate to the target.

	VARIABLE BAR 1 BAR !

This is OK - BAR will still point to the 1 when cross compiled.
