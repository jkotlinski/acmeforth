# ACMEforth

## What?

ACMEforth is a cross-compiling 16-bit Forth targeting the Commodore 64, using [https://sourceforge.net/projects/acme-crossass/](ACME) for assembling.
All Forth 2012 Core and Core Extension words are supported, although interpreting and compiling only works on PC.

## Examples

`sh run_examples.sh` runs the examples.

## Why?

I want a C64 Forth setup that gives

 * Instant compiles
 * Lean output (no dictionary or unused words)
 * Macro support with CREATE/DOES>
 * Convenient text editing on PC
