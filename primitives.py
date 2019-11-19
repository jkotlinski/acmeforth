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

