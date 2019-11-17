: cells ;
: chars ;
: align ;
: aligned ;

: s>d dup 0< ;
: begin here ; immediate
: nip swap drop ;
: if postpone 0branch here 0 , ; immediate
: ?dup dup if dup then ;
: case 0 ; immediate
: endcase postpone drop begin ?dup while postpone then repeat ; immediate
: of postpone (of) here 0 , ; immediate
: endof postpone else ; immediate
: value create , does> @ ;
: 0<> 0= 0= ;
: 0> dup 0< 0= swap 0<> and ;
: <> = 0= ;
: buffer: create allot ;
: hex $10 base ! ;
: decimal #10 base ! ;
: true -1 ;
: false 0 ;
: . s>d swap over dabs <# #s rot sign #> type space ;
: u. 0 <# #s #> type space ;
: compile, , ;
: save-input >in @ 1 ;
: restore-input drop >in ! 0 ;
: .s depth 1+ 1 ?do depth i - pick . loop cr ;
: .r ( n1 n2 -- )
swap s>d swap over dabs <# #s rot sign #>
rot over - spaces type space ;
: u.r ( u n -- )
swap 0 <# #s #> rot over - spaces type space ;
: pad here $100 + ;
: erase 0 fill ;
: 2over 3 pick 3 pick ;
: 2swap >r rot rot r> rot rot ;
: [ 0 state ! ; immediate
: ] -1 state ! ;
: count dup 1+ swap @ ;
: /string dup >r - swap r> + swap ;
: abort depth 0 do drop loop quit ;
: \ refill 0= if source nip >in ! then ; immediate
: bl $20 ;

( from FIG UK )
: /mod >r s>d r> fm/mod ;
: / /mod nip ;
: mod /mod drop ;
: */mod >r m* r> fm/mod ;
: */ */mod nip ;
: ?negate 0< if negate then ;
: sm/rem 2dup xor >r over >r abs >r dabs r> um/mod swap r> ?negate swap r> ?negate ;

( from forth-standard.org )
: isspace? BL 1+ U< ;
: isnotspace? isspace? 0= ;
: xt-skip >R BEGIN DUP WHILE OVER C@ R@ EXECUTE WHILE 1 /STRING REPEAT THEN R> DROP ;
: parse-name SOURCE >IN @ /STRING ['] isspace? xt-skip OVER >R ['] isnotspace? xt-skip 2DUP 1 MIN + SOURCE DROP - >IN ! DROP R> TUCK - ;
: DEFER CREATE ['] ABORT , DOES> @ EXECUTE ;
: defer! >body ! ;
: defer@ >body @ ;
: ACTION-OF STATE @ IF POSTPONE ['] POSTPONE DEFER@ ELSE ' DEFER@ THEN ; IMMEDIATE
: IS STATE @ IF POSTPONE ['] POSTPONE DEFER! ELSE ' DEFER! THEN ; IMMEDIATE
: HOLDS BEGIN DUP WHILE 1- 2DUP + C@ HOLD REPEAT 2DROP ; 
