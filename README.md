# Hack n' Trade Forth Compiler

## What?

HTFC is a cross-compiling 16-bit Forth targeting the Commodore 64, using ACME for assembling.
All Forth 2012 Core and Core Extension words are supported, although interpreting and compiling only works on PC.

## Example

	./htfc.py examples/colorcycle.fs
	acme -o colorcycle.prg -f cbm colorcycle.asm

`sh run_tests.sh` runs the test suite.

## Why?

I want a C64 Forth setup that gives

 * Fast builds
 * Lean output (no dictionary or unused words)
 * Macro support with CREATE/DOES>
 * Convenient text editing on PC

## Cross-Compiling Gotchas

### HERE

HERE only works on PC. The below code compiles, but when running on C64, FOO could point anywhere.

	HERE 1 , CONSTANT FOO

The below code works better. BAR gives the address of the 1 on both PC and C64.

	CREATE BAR 1 ,
