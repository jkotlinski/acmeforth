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

define("r@",
"""	txa
	tsx
	ldy $103,x
	sty W
	ldy $104,x
	tax
	dex
	sty MSB,x
	lda W
	sta LSB,x
	rts""",
	flags = F_NO_TAIL_CALL_ELIMINATION)

define(">r",
"""	pla
	sta W
	pla
	sta W+1
	inc W
	bne +
	inc W+1
+
	lda MSB,x
	pha
	lda LSB,x
	pha
	inx
	jmp (W)""",
	flags = F_NO_TAIL_CALL_ELIMINATION)
