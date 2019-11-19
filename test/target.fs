: BITSSET? IF 0 0 ELSE 0 THEN ;

: test-basic-assumptions
TESTING BASIC ASSUMPTIONS

T{ -> }T               \ START WITH CLEAN SLATE
( TEST IF ANY BITS ARE SET; ANSWER IN BASE 1 )
T{  0 BITSSET? -> 0 }T      ( ZERO IS ALL BITS CLEAR )
T{  1 BITSSET? -> 0 0 }T      ( OTHER NUMBER HAVE AT LEAST ONE BIT )
T{ -1 BITSSET? -> 0 0 }T
;

: run-tests
test-basic-assumptions ;

compile run-tests test_target.asm
