hex

: forth begin key? 0= while d020 c@ 1+ d020 c! repeat key drop ;

code asm
-	inc	$d020
	lda	$c6
	beq	-
	lda	#0
	sta	$c6
	rts
;code

: colorcycle
cr ." space=toggle asm/forth"
begin asm cr ." forth" forth cr ." asm" again ;

compile colorcycle
