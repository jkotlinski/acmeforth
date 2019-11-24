\ ----- testcore.fs

: BITSSET? IF 0 0 ELSE 0 THEN ;

: test-basic-assumptions
." testing basic assumptions" cr
T{ -> }T               \ START WITH CLEAN SLATE
( TEST IF ANY BITS ARE SET; ANSWER IN BASE 1 )
T{  0 BITSSET? -> 0 }T      ( ZERO IS ALL BITS CLEAR )
T{  1 BITSSET? -> 0 0 }T      ( OTHER NUMBER HAVE AT LEAST ONE BIT )
T{ -1 BITSSET? -> 0 0 }T ;

0    CONSTANT 0S
0 INVERT CONSTANT 1S

\ -----

: test-booleans
." testing booleans invert and or xor" cr
T{ 0 0 AND -> 0 }T
T{ 0 1 AND -> 0 }T
T{ 1 0 AND -> 0 }T
T{ 1 1 AND -> 1 }T

T{ 0 INVERT 1 AND -> 1 }T
T{ 1 INVERT 1 AND -> 0 }T

T{ 0S INVERT -> 1S }T
T{ 1S INVERT -> 0S }T

T{ 0S 0S AND -> 0S }T
T{ 0S 1S AND -> 0S }T
T{ 1S 0S AND -> 0S }T
T{ 1S 1S AND -> 1S }T

T{ 0S 0S OR -> 0S }T
T{ 0S 1S OR -> 1S }T
T{ 1S 0S OR -> 1S }T
T{ 1S 1S OR -> 1S }T

T{ 0S 0S XOR -> 0S }T
T{ 0S 1S XOR -> 1S }T
T{ 1S 0S XOR -> 1S }T
T{ 1S 1S XOR -> 0S }T ;

\ -----

1S 1 RSHIFT INVERT CONSTANT MSB
: test-shift
." testing 2* 2/ lshift rshift" cr

( WE TRUST 1S, INVERT, AND BITSSET?; WE WILL CONFIRM RSHIFT LATER )
T{ MSB BITSSET? -> 0 0 }T

T{ 0S 2* -> 0S }T
T{ 1 2* -> 2 }T
T{ 4000 2* -> 8000 }T
T{ 1S 2* 1 XOR -> 1S }T
T{ MSB 2* -> 0S }T

T{ 0S 2/ -> 0S }T
T{ 1 2/ -> 0 }T
T{ 4000 2/ -> 2000 }T
T{ 1S 2/ -> 1S }T            \ MSB PROPOGATED
T{ 1S 1 XOR 2/ -> 1S }T
T{ MSB 2/ MSB AND -> MSB }T

T{ 1 0 LSHIFT -> 1 }T
T{ 1 1 LSHIFT -> 2 }T
T{ 1 2 LSHIFT -> 4 }T
T{ 1 F LSHIFT -> 8000 }T         \ BIGGEST GUARANTEED SHIFT
T{ 1S 1 LSHIFT 1 XOR -> 1S }T
T{ MSB 1 LSHIFT -> 0 }T

T{ 1 0 RSHIFT -> 1 }T
T{ 1 1 RSHIFT -> 0 }T
T{ 2 1 RSHIFT -> 1 }T
T{ 4 2 RSHIFT -> 1 }T
T{ 8000 F RSHIFT -> 1 }T         \ BIGGEST
T{ MSB 1 RSHIFT MSB AND -> 0 }T      \ RSHIFT ZERO FILLS MSBS
T{ MSB 1 RSHIFT 2* -> MSB }T ;

\ -----

0 INVERT         CONSTANT MAX-UINT
0 INVERT 1 RSHIFT      CONSTANT MAX-INT
0 INVERT 1 RSHIFT INVERT   CONSTANT MIN-INT
0 INVERT 1 RSHIFT      CONSTANT MID-UINT
0 INVERT 1 RSHIFT INVERT   CONSTANT MID-UINT+1

0S CONSTANT <FALSE>
1S CONSTANT <TRUE>

: test-comparisons
." testing comparisons: 0= = 0< < > u< min max" cr

T{ 0 0= -> <TRUE> }T
T{ 1 0= -> <FALSE> }T
T{ 2 0= -> <FALSE> }T
T{ -1 0= -> <FALSE> }T
T{ MAX-UINT 0= -> <FALSE> }T
T{ MIN-INT 0= -> <FALSE> }T
T{ MAX-INT 0= -> <FALSE> }T

T{ 0 0 = -> <TRUE> }T
T{ 1 1 = -> <TRUE> }T
T{ -1 -1 = -> <TRUE> }T
T{ 1 0 = -> <FALSE> }T
T{ -1 0 = -> <FALSE> }T
T{ 0 1 = -> <FALSE> }T
T{ 0 -1 = -> <FALSE> }T

T{ 0 0< -> <FALSE> }T
T{ -1 0< -> <TRUE> }T
T{ MIN-INT 0< -> <TRUE> }T
T{ 1 0< -> <FALSE> }T
T{ MAX-INT 0< -> <FALSE> }T

T{ 0 1 < -> <TRUE> }T
T{ 1 2 < -> <TRUE> }T
T{ -1 0 < -> <TRUE> }T
T{ -1 1 < -> <TRUE> }T
T{ MIN-INT 0 < -> <TRUE> }T
T{ MIN-INT MAX-INT < -> <TRUE> }T
T{ 0 MAX-INT < -> <TRUE> }T
T{ 0 0 < -> <FALSE> }T
T{ 1 1 < -> <FALSE> }T
T{ 1 0 < -> <FALSE> }T
T{ 2 1 < -> <FALSE> }T
T{ 0 -1 < -> <FALSE> }T
T{ 1 -1 < -> <FALSE> }T
T{ 0 MIN-INT < -> <FALSE> }T
T{ MAX-INT MIN-INT < -> <FALSE> }T
T{ MAX-INT 0 < -> <FALSE> }T

T{ 0 1 > -> <FALSE> }T
T{ 1 2 > -> <FALSE> }T
T{ -1 0 > -> <FALSE> }T
T{ -1 1 > -> <FALSE> }T
T{ MIN-INT 0 > -> <FALSE> }T
T{ MIN-INT MAX-INT > -> <FALSE> }T
T{ 0 MAX-INT > -> <FALSE> }T
T{ 0 0 > -> <FALSE> }T
T{ 1 1 > -> <FALSE> }T
T{ 1 0 > -> <TRUE> }T
T{ 2 1 > -> <TRUE> }T
T{ 0 -1 > -> <TRUE> }T
T{ 1 -1 > -> <TRUE> }T
T{ 0 MIN-INT > -> <TRUE> }T
T{ MAX-INT MIN-INT > -> <TRUE> }T
T{ MAX-INT 0 > -> <TRUE> }T

T{ 0 1 U< -> <TRUE> }T
T{ 1 2 U< -> <TRUE> }T
T{ 0 MID-UINT U< -> <TRUE> }T
T{ 0 MAX-UINT U< -> <TRUE> }T
T{ MID-UINT MAX-UINT U< -> <TRUE> }T
T{ 0 0 U< -> <FALSE> }T
T{ 1 1 U< -> <FALSE> }T
T{ 1 0 U< -> <FALSE> }T
T{ 2 1 U< -> <FALSE> }T
T{ MID-UINT 0 U< -> <FALSE> }T
T{ MAX-UINT 0 U< -> <FALSE> }T
T{ MAX-UINT MID-UINT U< -> <FALSE> }T

T{ 0 1 MIN -> 0 }T
T{ 1 2 MIN -> 1 }T
T{ -1 0 MIN -> -1 }T
T{ -1 1 MIN -> -1 }T
T{ MIN-INT 0 MIN -> MIN-INT }T
T{ MIN-INT MAX-INT MIN -> MIN-INT }T
T{ 0 MAX-INT MIN -> 0 }T
T{ 0 0 MIN -> 0 }T
T{ 1 1 MIN -> 1 }T
T{ 1 0 MIN -> 0 }T
T{ 2 1 MIN -> 1 }T
T{ 0 -1 MIN -> -1 }T
T{ 1 -1 MIN -> -1 }T
T{ 0 MIN-INT MIN -> MIN-INT }T
T{ MAX-INT MIN-INT MIN -> MIN-INT }T
T{ MAX-INT 0 MIN -> 0 }T

T{ 0 1 MAX -> 1 }T
T{ 1 2 MAX -> 2 }T
T{ -1 0 MAX -> 0 }T
T{ -1 1 MAX -> 1 }T
T{ MIN-INT 0 MAX -> 0 }T
T{ MIN-INT MAX-INT MAX -> MAX-INT }T
T{ 0 MAX-INT MAX -> MAX-INT }T
T{ 0 0 MAX -> 0 }T
T{ 1 1 MAX -> 1 }T
T{ 1 0 MAX -> 1 }T
T{ 2 1 MAX -> 2 }T
T{ 0 -1 MAX -> 0 }T
T{ 1 -1 MAX -> 1 }T
T{ 0 MIN-INT MAX -> 0 }T
T{ MAX-INT MIN-INT MAX -> MAX-INT }T
T{ MAX-INT 0 MAX -> MAX-INT }T ;

\ -----

: test-stack-ops
." testing stack ops: 2drop 2dup 2over 2swap ?dup depth drop dup over rot swap" cr
T{ 1 2 2DROP -> }T
T{ 1 2 2DUP -> 1 2 1 2 }T
T{ 1 2 3 4 2OVER -> 1 2 3 4 1 2 }T
T{ 1 2 3 4 2SWAP -> 3 4 1 2 }T
T{ 0 ?DUP -> 0 }T
T{ 1 ?DUP -> 1 1 }T
T{ -1 ?DUP -> -1 -1 }T
T{ DEPTH -> 0 }T
T{ 0 DEPTH -> 0 1 }T
T{ 0 1 DEPTH -> 0 1 2 }T
T{ 0 DROP -> }T
T{ 1 2 DROP -> 1 }T
T{ 1 DUP -> 1 1 }T
T{ 1 2 OVER -> 1 2 1 }T
T{ 1 2 3 ROT -> 2 3 1 }T
T{ 1 2 SWAP -> 2 1 }T ;

\ -----

: GR1 >R R> ;
: GR2 >R R@ R> DROP ;
: test-return-stack-ops
." testing >r r> r@" cr
T{ 123 GR1 -> 123 }T
T{ 123 GR2 -> 123 }T
T{ 1S GR1 -> 1S }T ;   ( RETURN STACK HOLDS CELLS )

\ -----

: test-add-subtract
." testing add/subtract: + - 1+ 1- abs negate" cr
T{ 0 5 + -> 5 }T
T{ 5 0 + -> 5 }T
T{ 0 -5 + -> -5 }T
T{ -5 0 + -> -5 }T
T{ 1 2 + -> 3 }T
T{ 1 -2 + -> -1 }T
T{ -1 2 + -> 1 }T
T{ -1 -2 + -> -3 }T
T{ -1 1 + -> 0 }T
T{ MID-UINT 1 + -> MID-UINT+1 }T

T{ 0 5 - -> -5 }T
T{ 5 0 - -> 5 }T
T{ 0 -5 - -> 5 }T
T{ -5 0 - -> -5 }T
T{ 1 2 - -> -1 }T
T{ 1 -2 - -> 3 }T
T{ -1 2 - -> -3 }T
T{ -1 -2 - -> 1 }T
T{ 0 1 - -> -1 }T
T{ MID-UINT+1 1 - -> MID-UINT }T

T{ 0 1+ -> 1 }T
T{ -1 1+ -> 0 }T
T{ 1 1+ -> 2 }T
T{ MID-UINT 1+ -> MID-UINT+1 }T

T{ 2 1- -> 1 }T
T{ 1 1- -> 0 }T
T{ 0 1- -> -1 }T
T{ MID-UINT+1 1- -> MID-UINT }T

T{ 0 NEGATE -> 0 }T
T{ 1 NEGATE -> -1 }T
T{ -1 NEGATE -> 1 }T
T{ 2 NEGATE -> -2 }T
T{ -2 NEGATE -> 2 }T

T{ 0 ABS -> 0 }T
T{ 1 ABS -> 1 }T
T{ -1 ABS -> 1 }T
T{ MIN-INT ABS -> MID-UINT+1 }T ;

\ -----

: test-multiply
." testing multiply: s>d * m* um*" cr

T{ 0 S>D -> 0 0 }T
T{ 1 S>D -> 1 0 }T
T{ 2 S>D -> 2 0 }T
T{ -1 S>D -> -1 -1 }T
T{ -2 S>D -> -2 -1 }T
T{ MIN-INT S>D -> MIN-INT -1 }T
T{ MAX-INT S>D -> MAX-INT 0 }T

T{ 0 0 M* -> 0 S>D }T
T{ 0 1 M* -> 0 S>D }T
T{ 1 0 M* -> 0 S>D }T
T{ 1 2 M* -> 2 S>D }T
T{ 2 1 M* -> 2 S>D }T
T{ 3 3 M* -> 9 S>D }T
T{ -3 3 M* -> -9 S>D }T
T{ 3 -3 M* -> -9 S>D }T
T{ -3 -3 M* -> 9 S>D }T
T{ 0 MIN-INT M* -> 0 S>D }T
T{ 1 MIN-INT M* -> MIN-INT S>D }T
T{ 2 MIN-INT M* -> 0 1S }T
T{ 0 MAX-INT M* -> 0 S>D }T
T{ 1 MAX-INT M* -> MAX-INT S>D }T
T{ 2 MAX-INT M* -> MAX-INT 1 LSHIFT 0 }T
T{ MIN-INT MIN-INT M* -> 0 MSB 1 RSHIFT }T
T{ MAX-INT MIN-INT M* -> MSB MSB 2/ }T
T{ MAX-INT MAX-INT M* -> 1 MSB 2/ INVERT }T

T{ 0 0 * -> 0 }T            \ TEST IDENTITIES
T{ 0 1 * -> 0 }T
T{ 1 0 * -> 0 }T
T{ 1 2 * -> 2 }T
T{ 2 1 * -> 2 }T
T{ 3 3 * -> 9 }T
T{ -3 3 * -> -9 }T
T{ 3 -3 * -> -9 }T
T{ -3 -3 * -> 9 }T

T{ MID-UINT+1 1 RSHIFT 2 * -> MID-UINT+1 }T
T{ MID-UINT+1 2 RSHIFT 4 * -> MID-UINT+1 }T
T{ MID-UINT+1 1 RSHIFT MID-UINT+1 OR 2 * -> MID-UINT+1 }T

T{ 0 0 UM* -> 0 0 }T
T{ 0 1 UM* -> 0 0 }T
T{ 1 0 UM* -> 0 0 }T
T{ 1 2 UM* -> 2 0 }T
T{ 2 1 UM* -> 2 0 }T
T{ 3 3 UM* -> 9 0 }T

T{ MID-UINT+1 1 RSHIFT 2 UM* -> MID-UINT+1 0 }T
T{ MID-UINT+1 2 UM* -> 0 1 }T
T{ MID-UINT+1 4 UM* -> 0 2 }T
T{ 1S 2 UM* -> 1S 1 LSHIFT 1 }T
T{ MAX-UINT MAX-UINT UM* -> 1 1 INVERT }T ;

\ -----

: IFFLOORED
   [ -3 2 / -2 = INVERT ] LITERAL IF POSTPONE \ THEN ;

: IFSYM
   [ -3 2 / -1 = INVERT ] LITERAL IF POSTPONE \ THEN ;

IFFLOORED : T/MOD  >R S>D R> FM/MOD ;
IFFLOORED : T/     T/MOD SWAP DROP ;
IFFLOORED : TMOD   T/MOD DROP ;
IFFLOORED : T*/MOD >R M* R> FM/MOD ;
IFFLOORED : T*/    T*/MOD SWAP DROP ;
IFSYM     : T/MOD  >R S>D R> SM/REM ;
IFSYM     : T/     T/MOD SWAP DROP ;
IFSYM     : TMOD   T/MOD DROP ;
IFSYM     : T*/MOD >R M* R> SM/REM ;
IFSYM     : T*/    T*/MOD SWAP DROP ;

: test-divide
." testing divide: fm/mod sm/rem um/mod */ */mod / /mod mod" cr

T{ 0 S>D 1 FM/MOD -> 0 0 }T
T{ 1 S>D 1 FM/MOD -> 0 1 }T
T{ 2 S>D 1 FM/MOD -> 0 2 }T
T{ -1 S>D 1 FM/MOD -> 0 -1 }T
T{ -2 S>D 1 FM/MOD -> 0 -2 }T
T{ 0 S>D -1 FM/MOD -> 0 0 }T
T{ 1 S>D -1 FM/MOD -> 0 -1 }T
T{ 2 S>D -1 FM/MOD -> 0 -2 }T
T{ -1 S>D -1 FM/MOD -> 0 1 }T
T{ -2 S>D -1 FM/MOD -> 0 2 }T
T{ 2 S>D 2 FM/MOD -> 0 1 }T
T{ -1 S>D -1 FM/MOD -> 0 1 }T
T{ -2 S>D -2 FM/MOD -> 0 1 }T
T{  7 S>D  3 FM/MOD -> 1 2 }T
T{  7 S>D -3 FM/MOD -> -2 -3 }T
T{ -7 S>D  3 FM/MOD -> 2 -3 }T
T{ -7 S>D -3 FM/MOD -> -1 2 }T
T{ MAX-INT S>D 1 FM/MOD -> 0 MAX-INT }T
T{ MIN-INT S>D 1 FM/MOD -> 0 MIN-INT }T
T{ MAX-INT S>D MAX-INT FM/MOD -> 0 1 }T
T{ MIN-INT S>D MIN-INT FM/MOD -> 0 1 }T
T{ 1S 1 4 FM/MOD -> 3 MAX-INT }T
T{ 1 MIN-INT M* 1 FM/MOD -> 0 MIN-INT }T
T{ 1 MIN-INT M* MIN-INT FM/MOD -> 0 1 }T
T{ 2 MIN-INT M* 2 FM/MOD -> 0 MIN-INT }T
T{ 2 MIN-INT M* MIN-INT FM/MOD -> 0 2 }T
T{ 1 MAX-INT M* 1 FM/MOD -> 0 MAX-INT }T
T{ 1 MAX-INT M* MAX-INT FM/MOD -> 0 1 }T
T{ 2 MAX-INT M* 2 FM/MOD -> 0 MAX-INT }T
T{ 2 MAX-INT M* MAX-INT FM/MOD -> 0 2 }T
T{ MIN-INT MIN-INT M* MIN-INT FM/MOD -> 0 MIN-INT }T
T{ MIN-INT MAX-INT M* MIN-INT FM/MOD -> 0 MAX-INT }T
T{ MIN-INT MAX-INT M* MAX-INT FM/MOD -> 0 MIN-INT }T
T{ MAX-INT MAX-INT M* MAX-INT FM/MOD -> 0 MAX-INT }T

T{ 0 S>D 1 SM/REM -> 0 0 }T
T{ 1 S>D 1 SM/REM -> 0 1 }T
T{ 2 S>D 1 SM/REM -> 0 2 }T
T{ -1 S>D 1 SM/REM -> 0 -1 }T
T{ -2 S>D 1 SM/REM -> 0 -2 }T
T{ 0 S>D -1 SM/REM -> 0 0 }T
T{ 1 S>D -1 SM/REM -> 0 -1 }T
T{ 2 S>D -1 SM/REM -> 0 -2 }T
T{ -1 S>D -1 SM/REM -> 0 1 }T
T{ -2 S>D -1 SM/REM -> 0 2 }T
T{ 2 S>D 2 SM/REM -> 0 1 }T
T{ -1 S>D -1 SM/REM -> 0 1 }T
T{ -2 S>D -2 SM/REM -> 0 1 }T
T{  7 S>D  3 SM/REM -> 1 2 }T
T{  7 S>D -3 SM/REM -> 1 -2 }T
T{ -7 S>D  3 SM/REM -> -1 -2 }T
T{ -7 S>D -3 SM/REM -> -1 2 }T
T{ MAX-INT S>D 1 SM/REM -> 0 MAX-INT }T
T{ MIN-INT S>D 1 SM/REM -> 0 MIN-INT }T
T{ MAX-INT S>D MAX-INT SM/REM -> 0 1 }T
T{ MIN-INT S>D MIN-INT SM/REM -> 0 1 }T
T{ 1S 1 4 SM/REM -> 3 MAX-INT }T
T{ 2 MIN-INT M* 2 SM/REM -> 0 MIN-INT }T
T{ 2 MIN-INT M* MIN-INT SM/REM -> 0 2 }T
T{ 2 MAX-INT M* 2 SM/REM -> 0 MAX-INT }T
T{ 2 MAX-INT M* MAX-INT SM/REM -> 0 2 }T
T{ MIN-INT MIN-INT M* MIN-INT SM/REM -> 0 MIN-INT }T
T{ MIN-INT MAX-INT M* MIN-INT SM/REM -> 0 MAX-INT }T
T{ MIN-INT MAX-INT M* MAX-INT SM/REM -> 0 MIN-INT }T
T{ MAX-INT MAX-INT M* MAX-INT SM/REM -> 0 MAX-INT }T

T{ 0 0 1 UM/MOD -> 0 0 }T
T{ 1 0 1 UM/MOD -> 0 1 }T
T{ 1 0 2 UM/MOD -> 1 0 }T
T{ 3 0 2 UM/MOD -> 1 1 }T
T{ MAX-UINT 2 UM* 2 UM/MOD -> 0 MAX-UINT }T
T{ MAX-UINT 2 UM* MAX-UINT UM/MOD -> 0 2 }T
T{ MAX-UINT MAX-UINT UM* MAX-UINT UM/MOD -> 0 MAX-UINT }T

\ THE SYSTEM MIGHT DO EITHER FLOORED OR SYMMETRIC DIVISION.
\ SINCE WE HAVE ALREADY TESTED M*, FM/MOD, AND SM/REM WE CAN USE THEM IN TEST.

T{ 0 1 /MOD -> 0 1 T/MOD }T
T{ 1 1 /MOD -> 1 1 T/MOD }T
T{ 2 1 /MOD -> 2 1 T/MOD }T
T{ -1 1 /MOD -> -1 1 T/MOD }T
T{ -2 1 /MOD -> -2 1 T/MOD }T
T{ 0 -1 /MOD -> 0 -1 T/MOD }T
T{ 1 -1 /MOD -> 1 -1 T/MOD }T
T{ 2 -1 /MOD -> 2 -1 T/MOD }T
T{ -1 -1 /MOD -> -1 -1 T/MOD }T
T{ -2 -1 /MOD -> -2 -1 T/MOD }T
T{ 2 2 /MOD -> 2 2 T/MOD }T
T{ -1 -1 /MOD -> -1 -1 T/MOD }T
T{ -2 -2 /MOD -> -2 -2 T/MOD }T
T{ 7 3 /MOD -> 7 3 T/MOD }T
T{ 7 -3 /MOD -> 7 -3 T/MOD }T
T{ -7 3 /MOD -> -7 3 T/MOD }T
T{ -7 -3 /MOD -> -7 -3 T/MOD }T
T{ MAX-INT 1 /MOD -> MAX-INT 1 T/MOD }T
T{ MIN-INT 1 /MOD -> MIN-INT 1 T/MOD }T
T{ MAX-INT MAX-INT /MOD -> MAX-INT MAX-INT T/MOD }T
T{ MIN-INT MIN-INT /MOD -> MIN-INT MIN-INT T/MOD }T

T{ 0 1 / -> 0 1 T/ }T
T{ 1 1 / -> 1 1 T/ }T
T{ 2 1 / -> 2 1 T/ }T
T{ -1 1 / -> -1 1 T/ }T
T{ -2 1 / -> -2 1 T/ }T
T{ 0 -1 / -> 0 -1 T/ }T
T{ 1 -1 / -> 1 -1 T/ }T
T{ 2 -1 / -> 2 -1 T/ }T
T{ -1 -1 / -> -1 -1 T/ }T
T{ -2 -1 / -> -2 -1 T/ }T
T{ 2 2 / -> 2 2 T/ }T
T{ -1 -1 / -> -1 -1 T/ }T
T{ -2 -2 / -> -2 -2 T/ }T
T{ 7 3 / -> 7 3 T/ }T
T{ 7 -3 / -> 7 -3 T/ }T
T{ -7 3 / -> -7 3 T/ }T
T{ -7 -3 / -> -7 -3 T/ }T
T{ MAX-INT 1 / -> MAX-INT 1 T/ }T
T{ MIN-INT 1 / -> MIN-INT 1 T/ }T
T{ MAX-INT MAX-INT / -> MAX-INT MAX-INT T/ }T
T{ MIN-INT MIN-INT / -> MIN-INT MIN-INT T/ }T

T{ 0 1 MOD -> 0 1 TMOD }T
T{ 1 1 MOD -> 1 1 TMOD }T
T{ 2 1 MOD -> 2 1 TMOD }T
T{ -1 1 MOD -> -1 1 TMOD }T
T{ -2 1 MOD -> -2 1 TMOD }T
T{ 0 -1 MOD -> 0 -1 TMOD }T
T{ 1 -1 MOD -> 1 -1 TMOD }T
T{ 2 -1 MOD -> 2 -1 TMOD }T
T{ -1 -1 MOD -> -1 -1 TMOD }T
T{ -2 -1 MOD -> -2 -1 TMOD }T
T{ 2 2 MOD -> 2 2 TMOD }T
T{ -1 -1 MOD -> -1 -1 TMOD }T
T{ -2 -2 MOD -> -2 -2 TMOD }T
T{ 7 3 MOD -> 7 3 TMOD }T
T{ 7 -3 MOD -> 7 -3 TMOD }T
T{ -7 3 MOD -> -7 3 TMOD }T
T{ -7 -3 MOD -> -7 -3 TMOD }T
T{ MAX-INT 1 MOD -> MAX-INT 1 TMOD }T
T{ MIN-INT 1 MOD -> MIN-INT 1 TMOD }T
T{ MAX-INT MAX-INT MOD -> MAX-INT MAX-INT TMOD }T
T{ MIN-INT MIN-INT MOD -> MIN-INT MIN-INT TMOD }T

T{ 0 2 1 */ -> 0 2 1 T*/ }T
T{ 1 2 1 */ -> 1 2 1 T*/ }T
T{ 2 2 1 */ -> 2 2 1 T*/ }T
T{ -1 2 1 */ -> -1 2 1 T*/ }T
T{ -2 2 1 */ -> -2 2 1 T*/ }T
T{ 0 2 -1 */ -> 0 2 -1 T*/ }T
T{ 1 2 -1 */ -> 1 2 -1 T*/ }T
T{ 2 2 -1 */ -> 2 2 -1 T*/ }T
T{ -1 2 -1 */ -> -1 2 -1 T*/ }T
T{ -2 2 -1 */ -> -2 2 -1 T*/ }T
T{ 2 2 2 */ -> 2 2 2 T*/ }T
T{ -1 2 -1 */ -> -1 2 -1 T*/ }T
T{ -2 2 -2 */ -> -2 2 -2 T*/ }T
T{ 7 2 3 */ -> 7 2 3 T*/ }T
T{ 7 2 -3 */ -> 7 2 -3 T*/ }T
T{ -7 2 3 */ -> -7 2 3 T*/ }T
T{ -7 2 -3 */ -> -7 2 -3 T*/ }T
T{ MAX-INT 2 MAX-INT */ -> MAX-INT 2 MAX-INT T*/ }T
T{ MIN-INT 2 MIN-INT */ -> MIN-INT 2 MIN-INT T*/ }T

T{ 0 2 1 */MOD -> 0 2 1 T*/MOD }T
T{ 1 2 1 */MOD -> 1 2 1 T*/MOD }T
T{ 2 2 1 */MOD -> 2 2 1 T*/MOD }T
T{ -1 2 1 */MOD -> -1 2 1 T*/MOD }T
T{ -2 2 1 */MOD -> -2 2 1 T*/MOD }T
T{ 0 2 -1 */MOD -> 0 2 -1 T*/MOD }T
T{ 1 2 -1 */MOD -> 1 2 -1 T*/MOD }T
T{ 2 2 -1 */MOD -> 2 2 -1 T*/MOD }T
T{ -1 2 -1 */MOD -> -1 2 -1 T*/MOD }T
T{ -2 2 -1 */MOD -> -2 2 -1 T*/MOD }T
T{ 2 2 2 */MOD -> 2 2 2 T*/MOD }T
T{ -1 2 -1 */MOD -> -1 2 -1 T*/MOD }T
T{ -2 2 -2 */MOD -> -2 2 -2 T*/MOD }T
T{ 7 2 3 */MOD -> 7 2 3 T*/MOD }T
T{ 7 2 -3 */MOD -> 7 2 -3 T*/MOD }T
T{ -7 2 3 */MOD -> -7 2 3 T*/MOD }T
T{ -7 2 -3 */MOD -> -7 2 -3 T*/MOD }T
T{ MAX-INT 2 MAX-INT */MOD -> MAX-INT 2 MAX-INT T*/MOD }T
T{ MIN-INT 2 MIN-INT */MOD -> MIN-INT 2 MIN-INT T*/MOD }T ;

\ -----

HERE 1 ALLOT
HERE
CONSTANT 2NDA
CONSTANT 1STA

HERE 1 ,
HERE 2 ,
CONSTANT 2ND
CONSTANT 1ST

HERE 1 C,
HERE 2 C,
CONSTANT 2NDC
CONSTANT 1STC

ALIGN 1 ALLOT HERE ALIGN HERE 3 CELLS ALLOT
CONSTANT A-ADDR  CONSTANT UA-ADDR

: BITS ( X -- U )
   0 SWAP BEGIN DUP WHILE DUP MSB AND IF >R 1+ R> THEN 2* REPEAT DROP ;

: test-here
." testing here , @ ! cell+ cells c, c@ c! chars 2@ 2! align aligned +! allot" cr

T{ 1STA 2NDA U< -> <TRUE> }T      \ HERE MUST GROW WITH ALLOT
T{ 1STA 1+ -> 2NDA }T         \ ... BY ONE ADDRESS UNIT
( MISSING TEST: NEGATIVE ALLOT )

T{ 1ST 2ND U< -> <TRUE> }T         \ HERE MUST GROW WITH ALLOT
T{ 1ST CELL+ -> 2ND }T         \ ... BY ONE CELL
T{ 1ST 1 CELLS + -> 2ND }T
T{ 1ST @ 2ND @ -> 1 2 }T
T{ 5 1ST ! -> }T
T{ 1ST @ 2ND @ -> 5 2 }T
T{ 6 2ND ! -> }T
T{ 1ST @ 2ND @ -> 5 6 }T
T{ 1ST 2@ -> 6 5 }T
T{ 2 1 1ST 2! -> }T
T{ 1ST 2@ -> 2 1 }T
T{ 1S 1ST !  1ST @ -> 1S }T      \ CAN STORE CELL-WIDE VALUE

T{ 1STC 2NDC U< -> <TRUE> }T      \ HERE MUST GROW WITH ALLOT
T{ 1STC CHAR+ -> 2NDC }T         \ ... BY ONE CHAR
T{ 1STC 1 CHARS + -> 2NDC }T
T{ 1STC C@ 2NDC C@ -> 1 2 }T
T{ 3 1STC C! -> }T
T{ 1STC C@ 2NDC C@ -> 3 2 }T
T{ 4 2NDC C! -> }T
T{ 1STC C@ 2NDC C@ -> 3 4 }T

T{ UA-ADDR ALIGNED -> A-ADDR }T
T{    1 A-ADDR C!  A-ADDR C@ ->    1 }T
T{ 1234 A-ADDR  !  A-ADDR  @ -> 1234 }T
T{ 123 456 A-ADDR 2!  A-ADDR 2@ -> 123 456 }T
T{ 2 A-ADDR CHAR+ C!  A-ADDR CHAR+ C@ -> 2 }T
T{ 3 A-ADDR CELL+ C!  A-ADDR CELL+ C@ -> 3 }T
T{ 1234 A-ADDR CELL+ !  A-ADDR CELL+ @ -> 1234 }T
T{ 123 456 A-ADDR CELL+ 2!  A-ADDR CELL+ 2@ -> 123 456 }T

( CHARACTERS >= 1 AU, <= SIZE OF CELL, >= 8 BITS )
T{ 1 CHARS 1 < -> <FALSE> }T
T{ 1 CHARS 1 CELLS > -> <FALSE> }T
( TBD: HOW TO FIND NUMBER OF BITS? )

( CELLS >= 1 AU, INTEGRAL MULTIPLE OF CHAR SIZE, >= 16 BITS )
T{ 1 CELLS 1 < -> <FALSE> }T
T{ 1 CELLS 1 CHARS MOD -> 0 }T
T{ 1S BITS 10 < -> <FALSE> }T

T{ 0 1ST ! -> }T
T{ 1 1ST +! -> }T
T{ 1ST @ -> 1 }T
T{ -1 1ST +! 1ST @ -> 0 }T ;

\ -----

: GC1 [CHAR] X ;
: GC2 [CHAR] HELLO ;
: GC3 [ GC1 ] LITERAL ;
: GC4 S" xy" ; \ Changed from XY to xy due to PETSCII.

: test-char
." testing char [char] [ ] bl s" cr

T{ BL -> 20 }T
T{ [ CHAR X ] LITERAL -> 58 }T
T{ [ CHAR HELLO ] LITERAL -> 48 }T
T{ GC1 -> 58 }T
T{ GC2 -> 48 }T
T{ GC3 -> 58 }T
T{ GC4 SWAP DROP -> 2 }T
T{ GC4 DROP DUP C@ SWAP CHAR+ C@ -> 58 59 }T ;

\ -----

: GT1 123 ;
: GT2 ['] GT1 ; IMMEDIATE
create gt1string 3 C, CHAR G C, CHAR T C, CHAR 1 C,
: GT3 GT2 LITERAL ;
: GT4 POSTPONE GT1 ; IMMEDIATE
: GT5 GT4 ;
: GT6 345 ; IMMEDIATE
: GT7 POSTPONE GT6 ;

: test-tick
." testing ' ['] find execute immediate count literal postpone state" cr

T{ ['] gt1 execute -> 123 }T
T{ postpone GT2 EXECUTE -> 123 }T
( HOW TO SEARCH FOR NON-EXISTENT WORD? )
T{ GT1STRING COUNT -> GT1STRING CHAR+ 3 }T

T{ GT5 -> 123 }T
T{ GT7 -> 345 }T ;

\ -----

T{ : GI1 IF 123 THEN ; -> }T
T{ : GI2 IF 123 ELSE 234 THEN ; -> }T
T{ : GI3 BEGIN DUP 5 < WHILE DUP 1+ REPEAT ; -> }T
T{ : GI4 BEGIN DUP 1+ DUP 5 > UNTIL ; -> }T
T{ : GI5 BEGIN DUP 2 >
         WHILE DUP 5 < WHILE DUP 1+ REPEAT 123 ELSE 345 THEN ; -> }T
T{ : GI6 ( N -- 0,1,..N ) DUP IF DUP >R 1- RECURSE R> THEN ; -> }T
: test-control
." testing if else then begin while repeat until recurse" cr

T{ 0 GI1 -> }T
T{ 1 GI1 -> 123 }T
T{ -1 GI1 -> 123 }T
T{ 0 GI2 -> 234 }T
T{ 1 GI2 -> 123 }T
T{ -1 GI1 -> 123 }T

T{ 0 GI3 -> 0 1 2 3 4 5 }T
T{ 4 GI3 -> 4 5 }T
T{ 5 GI3 -> 5 }T
T{ 6 GI3 -> 6 }T

T{ 3 GI4 -> 3 4 5 6 }T
T{ 5 GI4 -> 5 6 }T
T{ 6 GI4 -> 6 7 }T

T{ 1 GI5 -> 1 345 }T
T{ 2 GI5 -> 2 345 }T
T{ 3 GI5 -> 3 4 5 123 }T
T{ 4 GI5 -> 4 5 123 }T
T{ 5 GI5 -> 5 123 }T

T{ 0 GI6 -> 0 }T
T{ 1 GI6 -> 0 1 }T
T{ 2 GI6 -> 0 1 2 }T
T{ 3 GI6 -> 0 1 2 3 }T
T{ 4 GI6 -> 0 1 2 3 4 }T ;

\ -----

T{ : GD1 DO I LOOP ; -> }T
T{ : GD2 DO I -1 +LOOP ; -> }T
T{ : GD3 DO 1 0 DO J LOOP LOOP ; -> }T
T{ : GD4 DO 1 0 DO J LOOP -1 +LOOP ; -> }T
T{ : GD5 123 SWAP 0 DO I 4 > IF DROP 234 LEAVE THEN LOOP ; -> }T
T{ : GD6  ( PAT: T{0 0},{0 0}{1 0}{1 1},{0 0}{1 0}{1 1}{2 0}{2 1}{2 2} )
   0 SWAP 0 DO
      I 1+ 0 DO I J + 3 = IF I UNLOOP I UNLOOP EXIT THEN 1+ LOOP
    LOOP ; -> }T
: test-loop
." testing do loop +loop i j unloop leave exit" cr

T{ 4 1 GD1 -> 1 2 3 }T
T{ 2 -1 GD1 -> -1 0 1 }T
T{ MID-UINT+1 MID-UINT GD1 -> MID-UINT }T

T{ 1 4 GD2 -> 4 3 2 1 }T
T{ -1 2 GD2 -> 2 1 0 -1 }T
T{ MID-UINT MID-UINT+1 GD2 -> MID-UINT+1 MID-UINT }T

T{ 4 1 GD3 -> 1 2 3 }T
T{ 2 -1 GD3 -> -1 0 1 }T
T{ MID-UINT+1 MID-UINT GD3 -> MID-UINT }T

T{ 1 4 GD4 -> 4 3 2 1 }T
T{ -1 2 GD4 -> 2 1 0 -1 }T
T{ MID-UINT MID-UINT+1 GD4 -> MID-UINT+1 MID-UINT }T

T{ 1 GD5 -> 123 }T
T{ 5 GD5 -> 123 }T
T{ 6 GD5 -> 234 }T

T{ 1 GD6 -> 1 }T
T{ 2 GD6 -> 3 }T
T{ 3 GD6 -> 4 1 2 }T ;

T{ 123 CONSTANT X123 -> }T
T{ : EQU CONSTANT ; -> }T
T{ X123 EQU Y123 -> }T
T{ VARIABLE V1 -> }T
T{ : NOP : POSTPONE ; ; -> }T
T{ NOP NOP1 NOP NOP2 -> }T
T{ : DOES1 DOES> @ 1 + ; -> }T
T{ : DOES2 DOES> @ 2 + ; -> }T
T{ CREATE CR1 -> }T
T{ CR1 -> HERE }T
T{ ' CR1 >BODY -> HERE }T
T{ 1 , -> }T
T{ CR1 @ -> 1 }T
T{ DOES1 -> }T
T{ CR1 -> 2 }T
T{ DOES2 -> }T
T{ CR1 -> 3 }T ;
T{ : WEIRD: CREATE DOES> 1 + DOES> 2 + ; -> }T
T{ WEIRD: W1 -> }T
T{ ' W1 >BODY -> HERE }T
T{ W1 -> HERE 1 + }T
T{ W1 -> HERE 2 + }T

: test-defines
." testing defining words: : ; constant variable create does> >body" cr

T{ X123 -> 123 }T
T{ Y123 -> 123 }T

T{ 123 V1 ! -> }T
T{ V1 @ -> 123 }T

T{ NOP1 -> }T
T{ NOP2 -> }T

T{ CR1 -> 3 }T ;

\ ----- <# # #s #> hold sign base >number hex decimal

: S=  \ ( ADDR1 C1 ADDR2 C2 -- T/F ) COMPARE TWO STRINGS.
   >R SWAP R@ = IF         \ MAKE SURE STRINGS HAVE SAME LENGTH
      R> ?DUP IF         \ IF NON-EMPTY STRINGS
    0 DO
       OVER C@ OVER C@ - IF 2DROP <FALSE> UNLOOP EXIT THEN
       SWAP CHAR+ SWAP CHAR+
         LOOP
      THEN
      2DROP <TRUE>         \ IF WE GET HERE, STRINGS MATCH
   ELSE
      R> DROP 2DROP <FALSE>      \ LENGTHS MISMATCH
   THEN ;

: GP1  <# 41 HOLD 42 HOLD 0 0 #> S" ba" S= ; \ Changed from BA to ba due to PETSCII.
: GP2  <# -1 SIGN 0 SIGN -1 SIGN 0 0 #> S" --" S= ;
: GP3  <# 1 0 # # #> S" 01" S= ;
: GP4  <# 1 0 #S #> S" 1" S= ;
24 CONSTANT MAX-BASE         \ BASE 2 .. 36
: COUNT-BITS
   0 0 INVERT BEGIN DUP WHILE >R 1+ R> 2* REPEAT DROP ;
COUNT-BITS 2* CONSTANT #BITS-UD      \ NUMBER OF BITS IN UD
: GP5
   BASE @ <TRUE>
   MAX-BASE 1+ 2 DO         \ FOR EACH POSSIBLE BASE
      I BASE !            \ TBD: ASSUMES BASE WORKS
      I 0 <# #S #> S" 10" S= AND
   LOOP
   SWAP BASE ! ;
: GP6
   BASE @ >R  2 BASE !
   MAX-UINT MAX-UINT <# #S #>      \ MAXIMUM UD TO BINARY
   R> BASE !            \ S: C-ADDR U
   DUP #BITS-UD = SWAP
   0 DO               \ S: C-ADDR FLAG
      OVER C@ [CHAR] 1 = AND      \ ALL ONES
      >R CHAR+ R>
   LOOP SWAP DROP ;
: GP7
   BASE @ >R    MAX-BASE BASE !
   <TRUE>
   A 0 DO
      I 0 <# #S #>
      1 = SWAP C@ I 30 + = AND AND
   LOOP
   MAX-BASE A DO
      I 0 <# #S #>
      1 = SWAP C@ 41 I A - + = AND AND
   LOOP
   R> BASE ! ;
CREATE GN-BUF 0 C,
: GN-STRING   GN-BUF 1 ;
: GN-CONSUMED   GN-BUF CHAR+ 0 ;
: GN'      GN-BUF C!  GN-STRING ;
: >NUMBER-BASED
   BASE @ >R BASE ! >NUMBER R> BASE ! ;
: GN1   \ ( UD BASE -- UD' LEN ) UD SHOULD EQUAL UD' AND LEN SHOULD BE ZERO.
   BASE @ >R BASE !
   <# #S #>
   0 0 2SWAP >NUMBER SWAP DROP      \ RETURN LENGTH ONLY
   R> BASE ! ;
: GN2   \ ( -- 16 10 )
   BASE @ >R  HEX BASE @  DECIMAL BASE @  R> BASE ! ;

: test-format
." testing <# # #s #> hold sign base >number hex decimal" cr

T{ GP1 -> <TRUE> }T
T{ GP2 -> <TRUE> }T
T{ GP3 -> <TRUE> }T
T{ GP4 -> <TRUE> }T
T{ GP5 -> <TRUE> }T
T{ GP6 -> <TRUE> }T
T{ GP7 -> <TRUE> }T

\ >NUMBER TESTS
T{ 0 0 '0' GN' >NUMBER -> 0 0 GN-CONSUMED }T
T{ 0 0 '1' GN' >NUMBER -> 1 0 GN-CONSUMED }T
T{ 1 0 '1' GN' >NUMBER -> BASE @ 1+ 0 GN-CONSUMED }T
T{ 0 0 '-' GN' >NUMBER -> 0 0 GN-STRING }T   \ SHOULD FAIL TO CONVERT THESE
T{ 0 0 '+' GN' >NUMBER -> 0 0 GN-STRING }T
T{ 0 0 '.' GN' >NUMBER -> 0 0 GN-STRING }T

T{ 0 0 '2' GN' 10 >NUMBER-BASED -> 2 0 GN-CONSUMED }T
T{ 0 0 '2' GN' 2 >NUMBER-BASED -> 0 0 GN-STRING }T
T{ 0 0 'F' GN' 10 >NUMBER-BASED -> F 0 GN-CONSUMED }T
T{ 0 0 'G' GN' 10 >NUMBER-BASED -> 0 0 GN-STRING }T
T{ 0 0 'G' GN' MAX-BASE >NUMBER-BASED -> 10 0 GN-CONSUMED }T
T{ 0 0 'Z' GN' MAX-BASE >NUMBER-BASED -> 23 0 GN-CONSUMED }T

T{ 0 0 2 GN1 -> 0 0 0 }T
T{ MAX-UINT 0 2 GN1 -> MAX-UINT 0 0 }T
T{ MAX-UINT DUP 2 GN1 -> MAX-UINT DUP 0 }T
T{ 0 0 MAX-BASE GN1 -> 0 0 0 }T
T{ MAX-UINT 0 MAX-BASE GN1 -> MAX-UINT 0 0 }T
T{ MAX-UINT DUP MAX-BASE GN1 -> MAX-UINT DUP 0 }T

T{ GN2 -> 10 A }T ;

\ -----

CREATE FBUF 00 C, 00 C, 00 C,
CREATE SBUF 12 C, 34 C, 56 C,
: SEEBUF FBUF C@  FBUF CHAR+ C@  FBUF CHAR+ CHAR+ C@ ;

: test-fill-move
." testing fill move" cr

T{ FBUF 0 20 FILL -> }T
T{ SEEBUF -> 00 00 00 }T

T{ FBUF 1 20 FILL -> }T
T{ SEEBUF -> 20 00 00 }T

T{ FBUF 3 20 FILL -> }T
T{ SEEBUF -> 20 20 20 }T

T{ FBUF FBUF 3 CHARS MOVE -> }T      \ BIZARRE SPECIAL CASE
T{ SEEBUF -> 20 20 20 }T

T{ SBUF FBUF 0 CHARS MOVE -> }T
T{ SEEBUF -> 20 20 20 }T

T{ SBUF FBUF 1 CHARS MOVE -> }T
T{ SEEBUF -> 12 20 20 }T

T{ SBUF FBUF 3 CHARS MOVE -> }T
T{ SEEBUF -> 12 34 56 }T

T{ FBUF FBUF CHAR+ 2 CHARS MOVE -> }T
T{ SEEBUF -> 12 12 34 }T

T{ FBUF CHAR+ FBUF 2 CHARS MOVE -> }T
T{ SEEBUF -> 12 34 34 }T ;

\ -----

: OUTPUT-TEST
   ." YOU SHOULD SEE THE STANDARD GRAPHIC CHARACTERS:" CR
   41 BL DO I EMIT LOOP CR
   61 41 DO I EMIT LOOP CR
   7F 61 DO I EMIT LOOP CR
   ." YOU SHOULD SEE 0-9 SEPARATED BY A SPACE:" CR
   9 1+ 0 DO I . LOOP CR
   ." YOU SHOULD SEE 0-9 (WITH NO SPACES):" CR
   [CHAR] 9 1+ [CHAR] 0 DO I 0 SPACES EMIT LOOP CR
   ." YOU SHOULD SEE A-G SEPARATED BY A SPACE:" CR
   [CHAR] G 1+ [CHAR] A DO I EMIT SPACE LOOP CR
   ." YOU SHOULD SEE 0-5 SEPARATED BY TWO SPACES:" CR
   5 1+ 0 DO I [CHAR] 0 + EMIT 2 SPACES LOOP CR
   ." YOU SHOULD SEE TWO SEPARATE LINES:" CR
   S" LINE 1" TYPE CR S" LINE 2" TYPE CR
   ." YOU SHOULD SEE THE NUMBER RANGES OF SIGNED AND UNSIGNED NUMBERS:" CR
   ."   SIGNED: " MIN-INT . MAX-INT . CR
   ." UNSIGNED: " 0 U. MAX-UINT U. CR
;

\ -----

CREATE ABUF 50 CHARS ALLOT

: ACCEPT-TEST
   CR ." PLEASE TYPE UP TO 80 CHARACTERS:" CR
   ABUF 50 ACCEPT
   CR ." RECEIVED: " [CHAR] " EMIT
   ABUF SWAP TYPE [CHAR] " EMIT CR
;

\ ----- testcoreplus.fs

DECIMAL

VARIABLE ITERATIONS
VARIABLE INCREMENT
: GD7 ( LIMIT START INCREMENT -- )
   INCREMENT !
   0 ITERATIONS !
   DO
      1 ITERATIONS +!
      I
      ITERATIONS @  6 = IF LEAVE THEN
      INCREMENT @
   +LOOP ITERATIONS @
;

: test+doloop1
." TESTING DO +LOOP with run-time increment, negative increment, infinite loop" cr
T{  4  4 -1 GD7 -> 4 1 }T
T{  1  4 -1 GD7 -> 4 3 2 1 4 }T
T{  4  1 -1 GD7 -> 1 0 -1 -2 -3 -4 6 }T
T{  4  1  0 GD7 -> 1 1 1 1 1 1 6 }T
T{  0  0  0 GD7 -> 0 0 0 0 0 0 6 }T
T{  1  4  0 GD7 -> 4 4 4 4 4 4 6 }T
T{  1  4  1 GD7 -> 4 5 6 7 8 9 6 }T
T{  4  1  1 GD7 -> 1 2 3 3 }T
T{  4  4  1 GD7 -> 4 5 6 7 8 9 6 }T
T{  2 -1 -1 GD7 -> -1 -2 -3 -4 -5 -6 6 }T
T{ -1  2 -1 GD7 -> 2 1 0 -1 4 }T
T{  2 -1  0 GD7 -> -1 -1 -1 -1 -1 -1 6 }T
T{ -1  2  0 GD7 -> 2 2 2 2 2 2 6 }T
T{ -1  2  1 GD7 -> 2 3 4 5 6 7 6 }T
T{  2 -1  1 GD7 -> -1 0 1 3 }T
T{ -20 30 -10 GD7 -> 30 20 10 0 -10 -20 6 }T
T{ -20 31 -10 GD7 -> 31 21 11 1 -9 -19 6 }T
T{ -20 29 -10 GD7 -> 29 19 9 -1 -11 5 }T ;

\ -----

\ Contributed by Andrew Haley

MAX-UINT 8 RSHIFT 1+ CONSTANT USTEP
USTEP NEGATE CONSTANT -USTEP
MAX-INT 7 RSHIFT 1+ CONSTANT STEP
STEP NEGATE CONSTANT -STEP

VARIABLE BUMP

T{ : GD8 BUMP ! DO 1+ BUMP @ +LOOP ; -> }T

\ Two's complement arithmetic, wraps around modulo wordsize
\ Only tested if the Forth system does wrap around, use of conditional
\ compilation deliberately avoided

MAX-INT 1+ MIN-INT = CONSTANT +WRAP?
MIN-INT 1- MAX-INT = CONSTANT -WRAP?
MAX-UINT 1+ 0=       CONSTANT +UWRAP?
0 1- MAX-UINT =      CONSTANT -UWRAP?

: GD9  ( n limit start step f result -- )
   >R IF GD8 ELSE 2DROP 2DROP R@ THEN -> R> }T
;

: test+doloop-largesmall
." TESTING DO +LOOP with large and small increments" cr

T{ 0 MAX-UINT 0 USTEP GD8 -> 256 }T
T{ 0 0 MAX-UINT -USTEP GD8 -> 256 }T
T{ 0 MAX-INT MIN-INT STEP GD8 -> 256 }T
T{ 0 MIN-INT MAX-INT -STEP GD8 -> 256 }T

T{ 0 0 0  USTEP +UWRAP? 256 GD9
T{ 0 0 0 -USTEP -UWRAP?   1 GD9
T{ 0 MIN-INT MAX-INT  STEP +WRAP? 1 GD9
T{ 0 MAX-INT MIN-INT -STEP -WRAP? 1 GD9 ;

\ -----

: (-MI) MAX-INT DUP NEGATE + 0= IF MAX-INT NEGATE ELSE -32767 THEN ;
(-MI) CONSTANT -MAX-INT

: test+doloop-maxmin
." TESTING DO +LOOP with maximum and minimum increments" cr

T{ 0 1 0 MAX-INT GD8  -> 1 }T
T{ 0 -MAX-INT NEGATE -MAX-INT OVER GD8  -> 2 }T

T{ 0 MAX-INT  0 MAX-INT GD8  -> 1 }T
T{ 0 MAX-INT  1 MAX-INT GD8  -> 1 }T
T{ 0 MAX-INT -1 MAX-INT GD8  -> 2 }T
T{ 0 MAX-INT DUP 1- MAX-INT GD8  -> 1 }T

T{ 0 MIN-INT 1+   0 MIN-INT GD8  -> 1 }T
T{ 0 MIN-INT 1+  -1 MIN-INT GD8  -> 1 }T
T{ 0 MIN-INT 1+   1 MIN-INT GD8  -> 2 }T
T{ 0 MIN-INT 1+ DUP MIN-INT GD8  -> 1 }T ;

\ -----

: SET-I  ( n1 n2 n3 -- n1-n2 | 1 )
   OVER = IF - ELSE 2DROP 1 THEN
;

: -SET-I ( n1 n2 n3 -- n1-n2 | -1 )
   SET-I DUP 1 = IF NEGATE THEN
;

: PL1 20 1 DO I 18 I 3 SET-I +LOOP ;
: PL2 20 1 DO I 20 I 2 SET-I +LOOP ;
: PL3 20 5 DO I 19 I 2 SET-I DUP 1 = IF DROP 0 I 6 SET-I THEN +LOOP ;
: PL4 20 1 DO I MAX-INT I 4 SET-I +LOOP ;
: PL5 -20 -1 DO I -19 I -3 -SET-I +LOOP ;
: PL6 -20 -1 DO I -21 I -4 -SET-I +LOOP ;
: PL7 -20 -1 DO I MIN-INT I -5 -SET-I +LOOP ;
: PL8 -20 -5 DO I -20 I -2 -SET-I DUP -1 = IF DROP 0 I -6 -SET-I THEN +LOOP ;

: test+do+loop
." TESTING +LOOP setting I to an arbitrary value" cr

T{ PL1 -> 1 2 3 18 19 }T
T{ PL2 -> 1 2 }T
T{ PL3 -> 5 6 0 1 2 19 }T
T{ PL4 -> 1 2 3 4 }T
T{ PL5 -> -1 -2 -3 -19 -20 }T
T{ PL6 -> -1 -2 -3 -4 }T
T{ PL7 -> -1 -2 -3 -4 -5 }T
T{ PL8 -> -5 -6 0 -1 -2 -20 }T ;

\ -----

: ACK ( m n -- u )    \ Ackermann function, from Rosetta Code
   OVER 0= IF  NIP 1+ EXIT  THEN       \ ack(0, n) = n+1
   SWAP 1- SWAP                        ( -- m-1 n )
   DUP  0= IF  1+  RECURSE EXIT  THEN  \ ack(m, 0) = ack(m-1, 1)
   1- OVER 1+ SWAP RECURSE RECURSE     \ ack(m, n) = ack(m-1, ack(m,n-1))
;

: test+multirecurse
." TESTING multiple RECURSEs in one colon definition" cr
T{ 0 0 ACK ->  1 }T
T{ 3 0 ACK ->  5 }T
T{ 2 4 ACK -> 11 }T ;

\ -----

: MELSE IF 1 ELSE 2 ELSE 3 ELSE 4 ELSE 5 THEN ;
: test+melse
." TESTING multiple ELSE's in an IF statement" cr
\ Discussed on comp.lang.forth and accepted as valid ANS Forth
T{ 0 MELSE -> 2 4 }T
T{ -1 MELSE -> 1 3 5 }T ;

\ -----

123 CONSTANT IW1 IMMEDIATE
: IW2 IW1 LITERAL ;
VARIABLE IW3 IMMEDIATE 234 IW3 !
: IW4 IW3 [ @ ] LITERAL ;
variable IW3-noname immediate
:NONAME [ 345 ] IW3-noname [ ! ] ; DROP
CREATE IW5 456 , IMMEDIATE
variable IW35
:NONAME IW5 [ @ IW35 ! ] ; DROP
: IW6 CREATE , IMMEDIATE DOES> @ 1+ ;
111 IW6 IW7
: IW8 IW7 LITERAL 1+ ;

: test+immediate
." TESTING IMMEDIATE with CONSTANT  VARIABLE and CREATE [ ... DOES> ]" cr

T{ postpone IW1 -> 123 }T
T{ IW2 -> 123 }T
T{ postpone IW3 @ -> 234 }T
T{ IW4 -> 234 }T
T{ postpone IW3-noname @ -> 345 }T
T{ IW35 @ -> 456 }T
T{ postpone IW7 -> 112 }T
T{ IW8 -> 113 }T ;

\ -----

VARIABLE IT1 0 IT1 !
: IT2 1234 IT1 ! ; IMMEDIATE IMMEDIATE
: IT3 IT2 ;

: test+immediate-toggle
." TESTING that IMMEDIATE doesn't toggle a flag" cr
T{ IT1 @ -> 1234 }T ;

\ -----

T{ : GC5 S" A string"2DROP ; GC5 -> }T
T{ ( A comment)1234 -> 1234 }T
: PB1 CR ." You should see 2345: "." 2345"( A comment) CR ;

: test+parse
." TESTING parsing behaviour"
T{ GC5 -> }T
T{ PB1 -> }T ;

\ -----

VARIABLE OLD-BASE

\ Check number prefixes in compile mode
T{ : nmp  #8327 $-2cbe %011010111 ''' ; nmp -> 8327 -11454 215 39 }T

: test+number-prefixes
." TESTING number prefixes # $ % and 'c' character input" cr
\ Adapted from the Forth 200X Draft 14.5 document

DECIMAL BASE @ OLD-BASE !
T{ #1289 -> 1289 }T
T{ #-1289 -> -1289 }T
T{ $12eF -> 4847 }T
T{ $-12eF -> -4847 }T
T{ %10010110 -> 150 }T
T{ %-10010110 -> -150 }T
T{ 'z' -> 122 }T
T{ 'Z' -> 90 }T
\ Check BASE is unchanged
T{ BASE @ OLD-BASE @ = -> <TRUE> }T

\ Repeat in Hex mode
16 OLD-BASE ! 16 BASE ! [ 16 base ! ]
T{ #1289 -> 509 }T
T{ #-1289 -> -509 }T
T{ $12eF -> 12EF }T
T{ $-12eF -> -12EF }T
T{ %10010110 -> 96 }T
T{ %-10010110 -> -96 }T
T{ 'z' -> 7a }T
T{ 'Z' -> 5a }T
\ Check BASE is unchanged
T{ BASE @ OLD-BASE @ = -> <TRUE> }T   \ 2
DECIMAL [ DECIMAL ]
T{ nmp -> 8327 -11454 215 39 }T
;

\ -----

: !"#$%&'()*+,-./0123456789:;<=>? 1 ;
: @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^ 2 ;
: _`abcdefghijklmnopqrstuvwxyz{|} 3 ;
: _`abcdefghijklmnopqrstuvwxyz{|~ 4 ;     \ Last character different

: test+definition-names
." TESTING definition names" cr
\ should support {1..31} graphical characters
T{ !"#$%&'()*+,-./0123456789:;<=>? -> 1 }T
T{ @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^ -> 2 }T
T{ _`abcdefghijklmnopqrstuvwxyz{|} -> 3 }T
T{ _`abcdefghijklmnopqrstuvwxyz{|~ -> 4 }T
T{ _`abcdefghijklmnopqrstuvwxyz{|} -> 3 }T ;

\ -----

T{ : UNS1 DUP 0 > IF 9 SWAP BEGIN 1+ DUP 3 > IF EXIT THEN REPEAT ; -> }T

: test+if-begin-repeat
." TESTING IF ... BEGIN ... REPEAT (unstructured)" cr
T{ -6 UNS1 -> -6 }T
T{  1 UNS1 -> 9 4 }T ;

\ -----

: MAKE-2CONST DOES> 2@ ;
CREATE 2K 3 , 2K , MAKE-2CONST

: test+does>
." TESTING DOES> doesn't cause a problem with a CREATEd address" cr
T{ 2K -> ['] 2K >BODY 3 }T
;

\ ----- testcoreext.fs

DECIMAL

: test.true-false
." TESTING TRUE FALSE" cr
T{ TRUE  -> 0 INVERT }T
T{ FALSE -> 0 }T ;

\ -----

: test.<>u>
." TESTING <> U>   (contributed by James Bowman)" cr

T{ 0 0 <> -> FALSE }T
T{ 1 1 <> -> FALSE }T
T{ -1 -1 <> -> FALSE }T
T{ 1 0 <> -> TRUE }T
T{ -1 0 <> -> TRUE }T
T{ 0 1 <> -> TRUE }T
T{ 0 -1 <> -> TRUE }T

T{ 0 1 U> -> FALSE }T
T{ 1 2 U> -> FALSE }T
T{ 0 MID-UINT U> -> FALSE }T
T{ 0 MAX-UINT U> -> FALSE }T
T{ MID-UINT MAX-UINT U> -> FALSE }T
T{ 0 0 U> -> FALSE }T
T{ 1 1 U> -> FALSE }T
T{ 1 0 U> -> TRUE }T
T{ 2 1 U> -> TRUE }T
T{ MID-UINT 0 U> -> TRUE }T
T{ MAX-UINT 0 U> -> TRUE }T
T{ MAX-UINT MID-UINT U> -> TRUE }T ;

\ -----

: test.0<>0>
." TESTING 0<> 0>   (contributed by James Bowman)" cr

T{ 0 0<> -> FALSE }T
T{ 1 0<> -> TRUE }T
T{ 2 0<> -> TRUE }T
T{ -1 0<> -> TRUE }T
T{ MAX-UINT 0<> -> TRUE }T
T{ MIN-INT 0<> -> TRUE }T
T{ MAX-INT 0<> -> TRUE }T

T{ 0 0> -> FALSE }T
T{ -1 0> -> FALSE }T
T{ MIN-INT 0> -> FALSE }T
T{ 1 0> -> TRUE }T
T{ MAX-INT 0> -> TRUE }T ;

\ -----

T{ : RO5 100 200 300 400 500 ; -> }T

: test.niptuckrollpick
." TESTING NIP TUCK ROLL PICK   (contributed by James Bowman)" cr

T{ 1 2 NIP -> 2 }T
T{ 1 2 3 NIP -> 1 3 }T

T{ 1 2 TUCK -> 2 1 2 }T
T{ 1 2 3 TUCK -> 1 3 2 3 }T

T{ RO5 3 ROLL -> 100 300 400 500 200 }T
T{ RO5 2 ROLL -> RO5 ROT }T
T{ RO5 1 ROLL -> RO5 SWAP }T
T{ RO5 0 ROLL -> RO5 }T

T{ RO5 2 PICK -> 100 200 300 400 500 300 }T
T{ RO5 1 PICK -> RO5 OVER }T
T{ RO5 0 PICK -> RO5 DUP }T ;

\ -----

T{ : RR0 2>R 100 R> R> ; -> }T
T{ : RR1 2>R 100 2R@ R> R> ; -> }T
T{ : RR2 2>R 100 2R> ; -> }T

: test.2>r2r@2r>
." TESTING 2>R 2R@ 2R>   (contributed by James Bowman)" cr

T{ 300 400 RR0 -> 100 400 300 }T
T{ 200 300 400 RR0 -> 200 100 400 300 }T

T{ 300 400 RR1 -> 100 300 400 400 300 }T
T{ 200 300 400 RR1 -> 200 100 300 400 400 300 }T

T{ 300 400 RR2 -> 100 300 400 }T
T{ 200 300 400 RR2 -> 200 100 300 400 }T ;

\ -----

: test.hex
." TESTING HEX   (contributed by James Bowman)" cr

T{ BASE @ HEX BASE @ DECIMAL BASE @ - SWAP BASE ! -> 6 }T ;

\ -----

: test.within
." TESTING WITHIN   (contributed by James Bowman)" cr

T{ 0 0 0 WITHIN -> FALSE }T
T{ 0 0 MID-UINT WITHIN -> TRUE }T
T{ 0 0 MID-UINT+1 WITHIN -> TRUE }T
T{ 0 0 MAX-UINT WITHIN -> TRUE }T
T{ 0 MID-UINT 0 WITHIN -> FALSE }T
T{ 0 MID-UINT MID-UINT WITHIN -> FALSE }T
T{ 0 MID-UINT MID-UINT+1 WITHIN -> FALSE }T
T{ 0 MID-UINT MAX-UINT WITHIN -> FALSE }T
T{ 0 MID-UINT+1 0 WITHIN -> FALSE }T
T{ 0 MID-UINT+1 MID-UINT WITHIN -> TRUE }T
T{ 0 MID-UINT+1 MID-UINT+1 WITHIN -> FALSE }T
T{ 0 MID-UINT+1 MAX-UINT WITHIN -> FALSE }T
T{ 0 MAX-UINT 0 WITHIN -> FALSE }T
T{ 0 MAX-UINT MID-UINT WITHIN -> TRUE }T
T{ 0 MAX-UINT MID-UINT+1 WITHIN -> TRUE }T
T{ 0 MAX-UINT MAX-UINT WITHIN -> FALSE }T
T{ MID-UINT 0 0 WITHIN -> FALSE }T
T{ MID-UINT 0 MID-UINT WITHIN -> FALSE }T
T{ MID-UINT 0 MID-UINT+1 WITHIN -> TRUE }T
T{ MID-UINT 0 MAX-UINT WITHIN -> TRUE }T
T{ MID-UINT MID-UINT 0 WITHIN -> TRUE }T
T{ MID-UINT MID-UINT MID-UINT WITHIN -> FALSE }T
T{ MID-UINT MID-UINT MID-UINT+1 WITHIN -> TRUE }T
T{ MID-UINT MID-UINT MAX-UINT WITHIN -> TRUE }T
T{ MID-UINT MID-UINT+1 0 WITHIN -> FALSE }T
T{ MID-UINT MID-UINT+1 MID-UINT WITHIN -> FALSE }T
T{ MID-UINT MID-UINT+1 MID-UINT+1 WITHIN -> FALSE }T
T{ MID-UINT MID-UINT+1 MAX-UINT WITHIN -> FALSE }T
T{ MID-UINT MAX-UINT 0 WITHIN -> FALSE }T
T{ MID-UINT MAX-UINT MID-UINT WITHIN -> FALSE }T
T{ MID-UINT MAX-UINT MID-UINT+1 WITHIN -> TRUE }T
T{ MID-UINT MAX-UINT MAX-UINT WITHIN -> FALSE }T
T{ MID-UINT+1 0 0 WITHIN -> FALSE }T
T{ MID-UINT+1 0 MID-UINT WITHIN -> FALSE }T
T{ MID-UINT+1 0 MID-UINT+1 WITHIN -> FALSE }T
T{ MID-UINT+1 0 MAX-UINT WITHIN -> TRUE }T
T{ MID-UINT+1 MID-UINT 0 WITHIN -> TRUE }T
T{ MID-UINT+1 MID-UINT MID-UINT WITHIN -> FALSE }T
T{ MID-UINT+1 MID-UINT MID-UINT+1 WITHIN -> FALSE }T
T{ MID-UINT+1 MID-UINT MAX-UINT WITHIN -> TRUE }T
T{ MID-UINT+1 MID-UINT+1 0 WITHIN -> TRUE }T
T{ MID-UINT+1 MID-UINT+1 MID-UINT WITHIN -> TRUE }T
T{ MID-UINT+1 MID-UINT+1 MID-UINT+1 WITHIN -> FALSE }T
T{ MID-UINT+1 MID-UINT+1 MAX-UINT WITHIN -> TRUE }T
T{ MID-UINT+1 MAX-UINT 0 WITHIN -> FALSE }T
T{ MID-UINT+1 MAX-UINT MID-UINT WITHIN -> FALSE }T
T{ MID-UINT+1 MAX-UINT MID-UINT+1 WITHIN -> FALSE }T
T{ MID-UINT+1 MAX-UINT MAX-UINT WITHIN -> FALSE }T
T{ MAX-UINT 0 0 WITHIN -> FALSE }T
T{ MAX-UINT 0 MID-UINT WITHIN -> FALSE }T
T{ MAX-UINT 0 MID-UINT+1 WITHIN -> FALSE }T
T{ MAX-UINT 0 MAX-UINT WITHIN -> FALSE }T
T{ MAX-UINT MID-UINT 0 WITHIN -> TRUE }T
T{ MAX-UINT MID-UINT MID-UINT WITHIN -> FALSE }T
T{ MAX-UINT MID-UINT MID-UINT+1 WITHIN -> FALSE }T
T{ MAX-UINT MID-UINT MAX-UINT WITHIN -> FALSE }T
T{ MAX-UINT MID-UINT+1 0 WITHIN -> TRUE }T
T{ MAX-UINT MID-UINT+1 MID-UINT WITHIN -> TRUE }T
T{ MAX-UINT MID-UINT+1 MID-UINT+1 WITHIN -> FALSE }T
T{ MAX-UINT MID-UINT+1 MAX-UINT WITHIN -> FALSE }T
T{ MAX-UINT MAX-UINT 0 WITHIN -> TRUE }T
T{ MAX-UINT MAX-UINT MID-UINT WITHIN -> TRUE }T
T{ MAX-UINT MAX-UINT MID-UINT+1 WITHIN -> TRUE }T
T{ MAX-UINT MAX-UINT MAX-UINT WITHIN -> FALSE }T

T{ MIN-INT MIN-INT MIN-INT WITHIN -> FALSE }T
T{ MIN-INT MIN-INT 0 WITHIN -> TRUE }T
T{ MIN-INT MIN-INT 1 WITHIN -> TRUE }T
T{ MIN-INT MIN-INT MAX-INT WITHIN -> TRUE }T
T{ MIN-INT 0 MIN-INT WITHIN -> FALSE }T
T{ MIN-INT 0 0 WITHIN -> FALSE }T
T{ MIN-INT 0 1 WITHIN -> FALSE }T
T{ MIN-INT 0 MAX-INT WITHIN -> FALSE }T
T{ MIN-INT 1 MIN-INT WITHIN -> FALSE }T
T{ MIN-INT 1 0 WITHIN -> TRUE }T
T{ MIN-INT 1 1 WITHIN -> FALSE }T
T{ MIN-INT 1 MAX-INT WITHIN -> FALSE }T
T{ MIN-INT MAX-INT MIN-INT WITHIN -> FALSE }T
T{ MIN-INT MAX-INT 0 WITHIN -> TRUE }T
T{ MIN-INT MAX-INT 1 WITHIN -> TRUE }T
T{ MIN-INT MAX-INT MAX-INT WITHIN -> FALSE }T
T{ 0 MIN-INT MIN-INT WITHIN -> FALSE }T
T{ 0 MIN-INT 0 WITHIN -> FALSE }T
T{ 0 MIN-INT 1 WITHIN -> TRUE }T
T{ 0 MIN-INT MAX-INT WITHIN -> TRUE }T
T{ 0 0 MIN-INT WITHIN -> TRUE }T
T{ 0 0 0 WITHIN -> FALSE }T
T{ 0 0 1 WITHIN -> TRUE }T
T{ 0 0 MAX-INT WITHIN -> TRUE }T
T{ 0 1 MIN-INT WITHIN -> FALSE }T
T{ 0 1 0 WITHIN -> FALSE }T
T{ 0 1 1 WITHIN -> FALSE }T
T{ 0 1 MAX-INT WITHIN -> FALSE }T
T{ 0 MAX-INT MIN-INT WITHIN -> FALSE }T
T{ 0 MAX-INT 0 WITHIN -> FALSE }T
T{ 0 MAX-INT 1 WITHIN -> TRUE }T
T{ 0 MAX-INT MAX-INT WITHIN -> FALSE }T
T{ 1 MIN-INT MIN-INT WITHIN -> FALSE }T
T{ 1 MIN-INT 0 WITHIN -> FALSE }T
T{ 1 MIN-INT 1 WITHIN -> FALSE }T
T{ 1 MIN-INT MAX-INT WITHIN -> TRUE }T
T{ 1 0 MIN-INT WITHIN -> TRUE }T
T{ 1 0 0 WITHIN -> FALSE }T
T{ 1 0 1 WITHIN -> FALSE }T
T{ 1 0 MAX-INT WITHIN -> TRUE }T
T{ 1 1 MIN-INT WITHIN -> TRUE }T
T{ 1 1 0 WITHIN -> TRUE }T
T{ 1 1 1 WITHIN -> FALSE }T
T{ 1 1 MAX-INT WITHIN -> TRUE }T
T{ 1 MAX-INT MIN-INT WITHIN -> FALSE }T
T{ 1 MAX-INT 0 WITHIN -> FALSE }T
T{ 1 MAX-INT 1 WITHIN -> FALSE }T
T{ 1 MAX-INT MAX-INT WITHIN -> FALSE }T
T{ MAX-INT MIN-INT MIN-INT WITHIN -> FALSE }T
T{ MAX-INT MIN-INT 0 WITHIN -> FALSE }T
T{ MAX-INT MIN-INT 1 WITHIN -> FALSE }T
T{ MAX-INT MIN-INT MAX-INT WITHIN -> FALSE }T
T{ MAX-INT 0 MIN-INT WITHIN -> TRUE }T
T{ MAX-INT 0 0 WITHIN -> FALSE }T
T{ MAX-INT 0 1 WITHIN -> FALSE }T
T{ MAX-INT 0 MAX-INT WITHIN -> FALSE }T
T{ MAX-INT 1 MIN-INT WITHIN -> TRUE }T
T{ MAX-INT 1 0 WITHIN -> TRUE }T
T{ MAX-INT 1 1 WITHIN -> FALSE }T
T{ MAX-INT 1 MAX-INT WITHIN -> FALSE }T
T{ MAX-INT MAX-INT MIN-INT WITHIN -> TRUE }T
T{ MAX-INT MAX-INT 0 WITHIN -> TRUE }T
T{ MAX-INT MAX-INT 1 WITHIN -> TRUE }T
T{ MAX-INT MAX-INT MAX-INT WITHIN -> FALSE }T ;

\ -----

\ unused not tested, since HERE does not exist

\ -----

T{ : AG0 701 BEGIN DUP 7 MOD 0= IF EXIT THEN 1+ AGAIN ; -> }T
: test.again
." TESTING AGAIN   (contributed by James Bowman)" cr
T{ AG0 -> 707 }T ;

\ -----

: QD ?DO I LOOP ;
: QD1 ?DO I 10 +LOOP ;
: QD2 ?DO I 3 > IF LEAVE ELSE I THEN LOOP ;
: QD3 ?DO I 1 +LOOP ;
: QD4 ?DO I -1 +LOOP ;
: QD5 ?DO I -10 +LOOP ;
VARIABLE ITERS
VARIABLE INCRMNT
: QD6 ( limit start increment -- )
   INCRMNT !
   0 ITERS !
   ?DO
      1 ITERS +!
      I
      ITERS @  6 = IF LEAVE THEN
      INCRMNT @
   +LOOP ITERS @
;

: test.?do
." TESTING ?DO" cr

T{ 789 789 QD -> }T
T{ -9876 -9876 QD -> }T
T{ 5 0 QD -> 0 1 2 3 4 }T

T{ 50 1 QD1 -> 1 11 21 31 41 }T
T{ 50 0 QD1 -> 0 10 20 30 40 }T

T{ 5 -1 QD2 -> -1 0 1 2 3 }T

T{ 4  4 QD3 -> }T
T{ 4  1 QD3 -> 1 2 3 }T
T{ 2 -1 QD3 -> -1 0 1 }T

T{  4 4 QD4 -> }T
T{  1 4 QD4 -> 4 3 2 1 }T
T{ -1 2 QD4 -> 2 1 0 -1 }T

T{   1 50 QD5 -> 50 40 30 20 10 }T
T{   0 50 QD5 -> 50 40 30 20 10 0 }T
T{ -25 10 QD5 -> 10 0 -10 -20 }T

T{  4  4 -1 QD6 -> 0 }T
T{  1  4 -1 QD6 -> 4 3 2 1 4 }T
T{  4  1 -1 QD6 -> 1 0 -1 -2 -3 -4 6 }T
T{  4  1  0 QD6 -> 1 1 1 1 1 1 6 }T
T{  0  0  0 QD6 -> 0 }T
T{  1  4  0 QD6 -> 4 4 4 4 4 4 6 }T
T{  1  4  1 QD6 -> 4 5 6 7 8 9 6 }T
T{  4  1  1 QD6 -> 1 2 3 3 }T
T{  4  4  1 QD6 -> 0 }T
T{  2 -1 -1 QD6 -> -1 -2 -3 -4 -5 -6 6 }T
T{ -1  2 -1 QD6 -> 2 1 0 -1 4 }T
T{  2 -1  0 QD6 -> -1 -1 -1 -1 -1 -1 6 }T
T{ -1  2  0 QD6 -> 2 2 2 2 2 2 6 }T
T{ -1  2  1 QD6 -> 2 3 4 5 6 7 6 }T
T{  2 -1  1 QD6 -> -1 0 1 3 }T ;

\ -----

T{ 8 BUFFER: BUF:TEST -> }T

: test.buffer:
." TESTING BUFFER:" cr
T{ BUF:TEST DUP ALIGNED = -> TRUE }T
T{ 111 BUF:TEST ! 222 BUF:TEST CELL+ ! -> }T
T{ BUF:TEST @ BUF:TEST CELL+ @ -> 111 222 }T ;

\ -----

T{ 111 VALUE VAL1 -999 VALUE VAL2 -> }T
T{ : VD1 VAL1 ; -> }T
T{ : VD2 TO VAL2 ; -> }T
T{ 123 VALUE VAL3 IMMEDIATE VAL3 -> 123 }T
T{ : VD3 VAL3 LITERAL ; -> }T

: test.value-to
." TESTING VALUE TO" cr
T{ VAL1 -> 111 }T
T{ VAL2 -> -999 }T
T{ 222 TO VAL1 -> }T
T{ VAL1 -> 222 }T
T{ VD1 -> 222 }T
T{ VAL2 -> -999 }T
T{ -333 VD2 -> }T
T{ VAL2 -> -333 }T
T{ VAL1 -> 222 }T
T{ VD3 -> 123 }T ;

\ -----

: CS1 CASE 1 OF 111 ENDOF
           2 OF 222 ENDOF
           3 OF 333 ENDOF
           >R 999 R>
      ENDCASE ;

: CS2 >R CASE -1 OF CASE R@ 1 OF 100 ENDOF
                            2 OF 200 ENDOF
                           >R -300 R>
                    ENDCASE
                 ENDOF
              -2 OF CASE R@ 1 OF -99  ENDOF
                            >R -199 R>
                    ENDCASE
                 ENDOF
                 >R 299 R>
         ENDCASE R> DROP ;

: CS3  ( N1 -- N2 )
   CASE 1- FALSE OF 11 ENDOF
        1- FALSE OF 22 ENDOF
        1- FALSE OF 33 ENDOF
        44 SWAP
   ENDCASE ;

T{ : CS4 CASE ENDCASE ; -> }T
T{ : CS5 CASE 2 SWAP ENDCASE ; -> }T
T{ : CS6 CASE 1 OF ENDOF 2 ENDCASE ; -> }T
T{ : CS7 CASE 3 OF ENDOF 2 ENDCASE ; -> }T

: test.caseof
." TESTING CASE OF ENDOF ENDCASE" cr

T{ 1 CS1 -> 111 }T
T{ 2 CS1 -> 222 }T
T{ 3 CS1 -> 333 }T
T{ 4 CS1 -> 999 }T

\ Nested CASE's

T{ -1 1 CS2 ->  100 }T
T{ -1 2 CS2 ->  200 }T
T{ -1 3 CS2 -> -300 }T
T{ -2 1 CS2 -> -99  }T
T{ -2 2 CS2 -> -199 }T
T{  0 2 CS2 ->  299 }T

\ Boolean short circuiting using CASE

T{ 1 CS3 -> 11 }T
T{ 2 CS3 -> 22 }T
T{ 3 CS3 -> 33 }T
T{ 9 CS3 -> 44 }T

\ Empty CASE statements with/without default

T{ 1 CS4 -> }T
T{ 1 CS5 -> 2 }T
T{ 1 CS6 -> }T
T{ 1 CS7 -> 1 }T ;

\ -----

VARIABLE NN1
VARIABLE NN2
:NONAME 1234 ; NN1 !
:NONAME 9876 ; NN2 !

T{ :NONAME ( n -- 0,1,..n ) DUP IF DUP >R 1- RECURSE R> THEN ;
   CONSTANT RN1 -> }T

:NONAME  ( n -- n1 )    \ Multiple RECURSEs in one definition
   1- DUP
   CASE 0 OF EXIT ENDOF
        1 OF 11 SWAP RECURSE ENDOF
        2 OF 22 SWAP RECURSE ENDOF
        3 OF 33 SWAP RECURSE ENDOF
        DROP ABS RECURSE EXIT
   ENDCASE
; CONSTANT RN2

: test.noname-recurse
." TESTING :NONAME RECURSE" cr
T{ NN1 @ EXECUTE -> 1234 }T
T{ NN2 @ EXECUTE -> 9876 }T
T{ 0 RN1 EXECUTE -> 0 }T
T{ 4 RN1 EXECUTE -> 0 1 2 3 4 }T
T{  1 RN2 EXECUTE -> 0 }T
T{  2 RN2 EXECUTE -> 11 0 }T
T{  4 RN2 EXECUTE -> 33 22 11 0 }T
T{ 25 RN2 EXECUTE -> 33 22 11 0 }T ;

\ -----

T{ : CQ1 C" 123" ; -> }T
T{ : CQ2 C" " ; -> }T
T{ : CQ3 C" 2345"; -> }T

: test.cquote ." TESTING C" '"' emit cr
T{ CQ1 COUNT S" 123" S= -> TRUE }T
T{ CQ2 COUNT S" " S= -> TRUE }T
T{ CQ3 COUNT S" 2345" S= -> TRUE }T ;

\ -----

:NONAME DUP + ; CONSTANT DUP+
T{ : Q DUP+ COMPILE, ; -> }T
T{ : AS1 [ Q ] ; -> }T

: test.compile,
." TESTING COMPILE," cr
T{ 123 AS1 -> 246 }T ;

\ -----

\ Create some large integers just below/above MAX and Min INTs
MAX-INT 73 79 */ CONSTANT LI1
MIN-INT 71 73 */ CONSTANT LI2
LI1 0 <# #S #> NIP CONSTANT LENLI1
: (.R&U.R)  ( u1 u2 -- )  \ u1 <= string length, u2 is required indentation
   TUCK + >R
   LI1 OVER SPACES  . CR R@    LI1 SWAP  .R CR
   LI2 OVER SPACES  . CR R@ 1+ LI2 SWAP  .R CR
   LI1 OVER SPACES U. CR R@    LI1 SWAP U.R CR
   LI2 SWAP SPACES U. CR R>    LI2 SWAP U.R CR
;
: .R&U.R  ( -- )
   CR ." You should see lines duplicated:" CR
   ." indented by 0 spaces" CR 0      0 (.R&U.R) CR
   ." indented by 0 spaces" CR LENLI1 0 (.R&U.R) CR \ Just fits required width
   ." indented by 5 spaces" CR LENLI1 5 (.R&U.R) CR
;

: test.ru.r
." TESTING .R and U.R - has to handle different cell sizes" cr
CR CR ." Output from .R and U.R"
T{ .R&U.R -> }T ;

\ -----

84 CONSTANT CHARS/PAD      \ Minimum size of PAD in chars
CHARS/PAD CHARS CONSTANT AUS/PAD
: CHECKPAD  ( caddr u ch -- f )  \ f = TRUE if u chars = ch
   SWAP 0
   ?DO
      OVER I CHARS + C@ OVER <>
      IF 2DROP UNLOOP FALSE EXIT THEN
   LOOP
   2DROP TRUE ;
T{ 0 INVERT PAD C! -> }T
T{ PAD C@ CONSTANT MAXCHAR -> }T

: test.pad-erase
." TESTING PAD ERASE" cr
\ Must handle different size characters i.e. 1 CHARS >= 1

T{ PAD DROP -> }T
T{ PAD CHARS/PAD 2DUP MAXCHAR FILL MAXCHAR CHECKPAD -> TRUE }T
T{ PAD CHARS/PAD 2DUP CHARS ERASE 0 CHECKPAD -> TRUE }T
T{ PAD CHARS/PAD 2DUP MAXCHAR FILL PAD 0 ERASE MAXCHAR CHECKPAD -> TRUE }T
T{ PAD 43 CHARS + 9 CHARS ERASE -> }T
T{ PAD 43 MAXCHAR CHECKPAD -> TRUE }T
T{ PAD 43 CHARS + 9 0 CHECKPAD -> TRUE }T
T{ PAD 52 CHARS + CHARS/PAD 52 - MAXCHAR CHECKPAD -> TRUE }T

\ Check that use of WORD and pictured numeric output do not corrupt PAD
\ Minimum size of buffers for these are 33 chars and (2*n)+2 chars respectively
\ where n is number of bits per cell

PAD CHARS/PAD ERASE
2 BASE !
MAX-UINT MAX-UINT <# #S [CHAR] 1 DUP HOLD HOLD #> 2DROP
DECIMAL
\ BL WORD 12345678123456781234567812345678 DROP  <-- no WORD on target!
T{ PAD CHARS/PAD 0 CHECKPAD -> TRUE }T ;

\ -----

T{ DEFER DEFER1 -> }T
T{ : MY-DEFER DEFER ; -> }T
T{ : IS-DEFER1 IS DEFER1 ; -> }T
T{ : ACTION-DEFER1 ACTION-OF DEFER1 ; -> }T
T{ : DEF! DEFER! ; -> }T
T{ : DEF@ DEFER@ ; -> }T
T{ MY-DEFER DEFER2 -> }T

: test.defer
." TESTING DEFER DEFER@ DEFER! IS ACTION-OF (Forth 2012)" cr
\ Adapted from the Forth 200X RfD tests

T{ ['] * ['] DEFER1 DEFER! -> }T
T{ 2 3 DEFER1 -> 6 }T
T{ ['] DEFER1 DEFER@ -> ['] * }T
T{ ['] DEFER1 DEF@ -> ['] * }T
T{ ACTION-OF DEFER1 -> ['] * }T
T{ ACTION-DEFER1 -> ['] * }T
T{ ['] + IS DEFER1 -> }T
T{ 1 2 DEFER1 -> 3 }T
T{ ['] DEFER1 DEFER@ -> ['] + }T
T{ ['] DEFER1 DEF@ -> ['] + }T
T{ ACTION-OF DEFER1 -> ['] + }T
T{ ACTION-DEFER1 -> ['] + }T
T{ ['] - IS-DEFER1 -> }T
T{ 1 2 DEFER1 -> -1 }T
T{ ['] DEFER1 DEFER@ -> ['] - }T
T{ ['] DEFER1 DEF@ -> ['] - }T
T{ ACTION-OF DEFER1 -> ['] - }T
T{ ACTION-DEFER1 -> ['] - }T

T{ ['] DUP IS DEFER2 -> }T
T{ 1 DEFER2 -> 1 1 }T ;

\ ----

: HTEST S" Testing HOLDS" ;
: HTEST2 S" works" ;
: HTEST3 S" Testing HOLDS works 123" ;
T{ : HLD HOLDS ; -> }T

: test.holds
." TESTING HOLDS  (Forth 2012)" cr
T{ 0 0 <#  HTEST HOLDS #> HTEST S= -> TRUE }T
T{ 123 0 <# #S BL HOLD HTEST2 HOLDS BL HOLD HTEST HOLDS #>
   HTEST3 S= -> TRUE }T
T{ 0 0 <#  HTEST HLD #> HTEST S= -> TRUE }T ;

\ -----

T{ : SSQ1 S\" abc" S" abc" S= ; -> }T  \ No escapes
T{ : SSQ2 S\" " ; -> }T    \ Empty string
T{ : SSQ3 S\" \a\b\e\f\l\m\q\r\t\v\x0F0\x1Fa\xaBx\z\"\\" ; -> }T
T{ : SSQ4 S\" \nOne line...\nanotherLine\n" type ; -> }T
T{ : SSQ5 S\" abeflmnqrtvxz" S" abeflmnqrtvxz" S= ; -> }T
T{ : SSQ6 S\" a\""2DROP 1111 ; -> }T \ Parsing behaviour

: test.s\"
." TESTING S\" '"' emit ." (Forth 2012 compilation mode)" cr
\ Extended the Forth 200X RfD tests
\ Note this tests the Core Ext definition of S\" which has unedfined
\ interpretation semantics. S\" in interpretation mode is tested in the tests on
\ the File-Access word set

T{ SSQ1 -> TRUE }T
T{ SSQ2 SWAP DROP -> 0 }T    \ Empty string

T{ SSQ3 SWAP DROP          ->  20 }T    \ String length
T{ SSQ3 DROP            C@ ->   7 }T    \ \a   BEL  Bell
T{ SSQ3 DROP  1 CHARS + C@ ->   8 }T    \ \b   BS   Backspace
T{ SSQ3 DROP  2 CHARS + C@ ->  27 }T    \ \e   ESC  Escape
T{ SSQ3 DROP  3 CHARS + C@ ->  12 }T    \ \f   FF   Form feed
T{ SSQ3 DROP  4 CHARS + C@ ->  10 }T    \ \l   LF   Line feed
T{ SSQ3 DROP  5 CHARS + C@ ->  13 }T    \ \m        CR of CR/LF pair
T{ SSQ3 DROP  6 CHARS + C@ ->  10 }T    \           LF of CR/LF pair
T{ SSQ3 DROP  7 CHARS + C@ ->  34 }T    \ \q   "    Double Quote
T{ SSQ3 DROP  8 CHARS + C@ ->  13 }T    \ \r   CR   Carriage Return
T{ SSQ3 DROP  9 CHARS + C@ ->   9 }T    \ \t   TAB  Horizontal Tab
T{ SSQ3 DROP 10 CHARS + C@ ->  11 }T    \ \v   VT   Vertical Tab
T{ SSQ3 DROP 11 CHARS + C@ ->  15 }T    \ \x0F      Given Char
T{ SSQ3 DROP 12 CHARS + C@ ->  48 }T    \ 0    0    Digit follow on
T{ SSQ3 DROP 13 CHARS + C@ ->  31 }T    \ \x1F      Given Char
T{ SSQ3 DROP 14 CHARS + C@ ->  97 }T    \ a    a    Hex follow on
T{ SSQ3 DROP 15 CHARS + C@ -> 171 }T    \ \xaB      Insensitive Given Char
T{ SSQ3 DROP 16 CHARS + C@ -> 120 }T    \ x    x    Non hex follow on
T{ SSQ3 DROP 17 CHARS + C@ ->   0 }T    \ \z   NUL  No Character
T{ SSQ3 DROP 18 CHARS + C@ ->  34 }T    \ \"   "    Double Quote
T{ SSQ3 DROP 19 CHARS + C@ ->  92 }T    \ \\   \    Back Slash

\ The above does not test \n as this is a system dependent value.
\ Check it displays a new line
CR ." The next test should display:"
CR ." One line..."
CR ." another line"
T{ SSQ4 -> }T

\ Test bare escapable characters appear as themselves
T{ SSQ5 -> TRUE }T

T{ SSQ6 -> 1111 }T \ Parsing behaviour

\ T{ : SSQ7  S\" 111 : SSQ8 s\\\" 222\" EVALUATE ; SSQ8 333" EVALUATE ; -> }T
\ T{ SSQ7 -> 111 222 333 }T
\ T{ : SSQ9  S\" 11 : SSQ10 s\\\" \\x32\\x32\" EVALUATE ; SSQ10 33" EVALUATE ; -> }T
\ T{ SSQ9 -> 11 22 33 }T
;

\ -----

: target-test
#23 #53272 c! \ switch to upper/lower case mode
test-basic-assumptions
test-booleans
test-shift
test-comparisons
test-stack-ops
test-return-stack-ops
test-add-subtract
test-multiply
test-divide
test-here
test-char
test-tick
test-control
test-loop
test-defines
test-format
test-fill-move
OUTPUT-TEST
ACCEPT-TEST
CR ." End of Core word set tests" CR

test+doloop1
test+doloop-largesmall
test+doloop-maxmin
test+do+loop
test+multirecurse
test+melse
test+immediate
test+immediate-toggle
test+parse
test+number-prefixes
test+definition-names
test+if-begin-repeat
test+does>
CR ." End of additional Core tests" CR

test.true-false
test.<>u>
test.0<>0>
test.niptuckrollpick
test.2>r2r@2r>
test.hex
test.within
test.again
test.?do
test.buffer:
test.value-to
test.caseof
test.noname-recurse
test.cquote
test.compile,
test.ru.r
test.pad-erase
test.defer
test.holds
test.s\"
CR ." End of Core Extension word tests" CR ;

compile target-test
