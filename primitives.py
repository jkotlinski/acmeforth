asm = {}

def define(name, code):
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
