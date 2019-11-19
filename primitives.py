asm = {}

def define(name, code, deps = []):
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
"""	inc LSB, x
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
	bne	3
	inc	$104,x

	lda	$103,x	; lsb check
	cmp	$105,x
	beq	+
-			; not done, branch back
	ldx	w	; restore x
	jmp	branch
+
	lda	$104,x	; msb check
	cmp	$106,x
	bne	-

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
	"branch")
