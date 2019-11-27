# ACMEforth

## What?

ACMEforth is a cross-compiling 16-bit Forth targeting the Commodore 64, using [ACME](https://sourceforge.net/projects/acme-crossass/) for assembling.
All [Forth 2012 core words](https://forth-standard.org/standard/core) are supported, although interpreting and compiling only works on PC.

## Examples

`sh run_examples.sh`

## Why?

 * Quick compiles
 * Lean output (no dictionary or unused words)
 * Macro support with CREATE/DOES>
 * Convenient text editing on PC
