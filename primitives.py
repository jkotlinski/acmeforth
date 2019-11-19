asm = {}

F_NO_TAIL_CALL_ELIMINATION = 1

def define(name, code, deps = [], flags = 0):
	asm[name] = code

define("c@",
"""	lda LSB,x
	sta + + 1
	lda MSB,x
	sta + + 2
+	lda $cafe
	sta LSB,x
	lda #0
	sta MSB,x
	rts""")

define("c!",
"""	lda LSB,x
	sta + + 1
	lda MSB,x
	sta + + 2
	lda LSB+1,x
+	sta $1234
	inx
	inx
	rts""")

define("1+",
"""ONEPLUS
	inc LSB, x
	bne +
	inc MSB, x
+	rts""")

define("litc",
"""	dex

	; load IP
	pla
	sta W
	pla
	sta W + 1

	inc W
	bne +
	inc W + 1
+
	; copy literal to stack
	ldy #0
	lda (W), y
	sta LSB, x
	sty MSB, x

	inc W
	bne +
	inc W + 1
+	jmp (W)""")

define("lit",
"""	dex

	; load IP
	pla
	sta W
	pla
	sta W + 1

	; copy literal to stack
	ldy #1
	lda (W), y
	sta LSB, x
	iny
	lda (W), y
	sta MSB, x

	lda W
	clc
	adc #3
	sta + + 1
	lda W + 1
	adc #0
	sta + + 2
+	jmp $1234""")

define("(loop)",
"""	stx	w	; x = stack pointer
	tsx

	inc	$103,x	; i++
	bne	+
	inc	$104,x
+
	lda	$103,x	; lsb check
	cmp	$105,x
	beq	.check_msb

.continue_loop
	ldx	w	; restore x
	jmp	BRANCH

.check_msb
	lda	$104,x
	cmp	$106,x
	bne	.continue_loop

	pla		; loop done - skip branch address
	clc
	adc	#3
	sta	w2

	pla
	adc	#0
	sta	w2 + 1

	txa		; sp += 6
	clc
	adc	#6
	tax
	txs

	ldx	w	; restore x
	jmp	(w2)""",
	deps = ["branch"])

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
	beq BRANCH

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
	rts""",
	deps = ["branch"])

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
"""	jsr INVERT
	jmp ONEPLUS""",
	deps = ["invert", "1+"])

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
+	jsr OVER
	jsr FETCHBYTE
	jsr EMIT
	jsr ONE
	jsr SLASH_STRING
	jmp -""")
