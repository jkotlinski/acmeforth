set -e
./htfc.py test/target-tester.fs test/target.fs
acme -o target-test.prg -f cbm target-test.asm
x64 target-test.prg
echo "Accept text" | ./htfc.py test/tester.fs test/testutilities.fs test/testcore.fs test/testerrorreport.fs test/testcoreplus.fs test/testcoreext.fs test/report-errors.fs
