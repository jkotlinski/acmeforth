\ Up, up and away!
\ Adapted from Commodore 64 Users Guide.

create sprite
0 c, 127 c, 0 c, 1 c, 255 c, 192 c, 3 c, 255 c, 224 c, 3 c, 231 c, 224 c,
7 c, 217 c, 240 c, 7 c, 223 c, 240 c, 7 c, 217 c, 240 c, 3 c, 231 c, 224 c,
3 c, 255 c, 224 c, 3 c, 255 c, 224 c, 2 c, 255 c, 160 c, 1 c, 127 c, 64 c,
1 c, 62 c, 64 c, 0 c, 156 c, 128 c, 0 c, 156 c, 128 c, 0 c, 73 c, 0 c, 0 c, 73 c, 0 c,
0 c, 62 c, 0 c, 0 c, 62 c, 0 c, 0 c, 62 c, 0 c, 0 c, 28 c, 0 c,

: vsync
begin $d011 c@ $80 and 0= until
begin $d011 c@ $80 and until ;

: balloon page
4 $d015 c! \ enable sprite 2
13 $7fa c! \ sprite 2 data from 13th block
sprite $340 63 move \ copy sprite to 13th block
begin 200 0 do vsync
i $d004 c! i $d005 c! \ move sprite
loop again ;

compile balloon
