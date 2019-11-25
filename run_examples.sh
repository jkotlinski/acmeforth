set -e
./acmeforth examples/sierp.fs
acme -o sierp.prg -f cbm sierp.asm
x64 sierp.prg

./acmeforth examples/colorcycle.fs
acme -o colorcycle.prg -f cbm colorcycle.asm
x64 colorcycle.prg
