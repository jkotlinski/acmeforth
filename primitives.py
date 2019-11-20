asm = {}

F_NO_TAIL_CALL_ELIMINATION = 1

def define(name, code, flags = 0):
	asm[name] = code

define("r>",
"""	pla
	sta W
	pla
	sta W+1
	inc W
	bne +
	inc W+1
+
	dex
	pla
	sta LSB,x
	pla
	sta MSB,x
	jmp (W)""",
	flags = F_NO_TAIL_CALL_ELIMINATION)

define("0branch",
"""	inx
	lda LSB-1, x
	ora MSB-1, x
	beq %branch%

	; skip offset
	pla
	clc
	adc #2
	bcc +
	tay
	pla
	adc #0
	pha
	tya
+	pha
	rts""")

define("!",
"""	lda LSB, x
	sta W
	lda MSB, x
	sta W + 1

	ldy #0
	lda LSB+1, x
	sta (W), y
	iny
	lda MSB+1, x
	sta (W), y

	inx
	inx
	rts""")

define("negate",
"""	jsr %invert%
	jmp %1+%""")

define("0<",
"""	lda	MSB,x
	and	#$80
	beq	+
	lda	#$ff
+	sta	LSB,x
	sta	MSB,x
	rts""")

define("dup",
"""	dex
	lda MSB + 1, x
	sta MSB, x
	lda LSB + 1, x
	sta LSB, x
	rts""")

define("type",
"""-	lda LSB,x
	ora MSB,x
	bne +
	inx
	inx
	rts
+	jsr %over%
	jsr %c@%
	jsr %emit%
	jsr %1%
	jsr %/string%
	jmp -""")

define("depth",
"""	txa
	eor #$ff
	tay
	iny
	dex
	sty LSB,x
	lda #0
	sta MSB,x
	rts""")

define("@",
"""	lda LSB,x
	sta W
	lda MSB,x
	sta W+1

	ldy #0
	lda (W),y
	sta LSB,x
	iny
	lda (W),y
	sta MSB,x
	rts""")

define("=",
"""	ldy #0
	lda LSB, x
	cmp LSB + 1, x
	bne +
	lda MSB, x
	cmp MSB + 1, x
	bne +
	dey
+	inx
	sty MSB, x
	sty LSB, x
	rts""")

define("(do)",
"""	pla
	sta	W
	pla
	tay

	lda	MSB+1,x
	pha
	lda	LSB+1,x
	pha

	lda	MSB,x
	pha
	lda	LSB,x
	pha

	inx
	inx

	tya
	pha
	lda	W
	pha
	rts""")
