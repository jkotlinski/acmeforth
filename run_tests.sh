set -e
./htfc.py test/target-tester.fs test/target.fs
acme -o target-test.prg -f cbm target-test.asm
x64 target-test.prg
echo "Accept text" | ./htfc.py test/tester.fs test/testutilities.fs test/testcore.fs test/testerrorreport.fs test/testcoreplus.fs test/testcoreext.fs test/report-errors.fs

# examples
./htfc.py examples/sierp.fs
acme -o sierp.prg -f cbm sierp.asm
x64 sierp.prg

./htfc.py examples/colorcycle.fs
acme -o colorcycle.prg -f cbm colorcycle.asm
x64 colorcycle.prg
