\ https://boomlin.de/software/sierp.fs
hex
variable x variable y

: orc dup c@ rot or swap c! ;
: rnd d012 c@ ;
: init 8 d018 orc 20 d011 orc ;
: clear 2000 dup 0 fill 400 dup 70 fill ;
: calcx ( x -- offset ) fff8 and ;
: calcy ( y -- offset ) dup 2/ 2/ 2/ 140 * swap 7 and + ;
: store ( v x y -- ) calcy swap calcx + 2000 + orc ;
: calcbit 1 swap 7 and 7 xor lshift ;
: plot ( x y ) over calcbit rot rot store ;
: avg ( x1 y1 x2 y2 -- x y ) rot + 2/ rot rot + 2/ swap ;

decimal
: p1 160 20 ;
: p2 60 180 ;
: p3 260 180 ;

: sierp
p1 y ! x ! init clear
begin
rnd 3 and case
0 of p1 x @ y @ avg y ! x ! endof
1 of p2 x @ y @ avg y ! x ! endof
2 of p3 x @ y @ avg y ! x ! endof
endcase
x @ y @ plot again ;

compile sierp
