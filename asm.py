asm = {}

def add_word(name, code):
	asm[name] = code

add_word("c@", 
"""	lda LSB,x
	sta + + 1
	lda MSB,x
	sta + + 2
+	lda $cafe
	sta LSB,x
	lda #0
	sta MSB,x
	rts""")

add_word("c!",
"""	lda LSB,x
	sta + + 1
	lda MSB,x
	sta + + 2
	lda LSB+1,x
+	sta $1234
	inx
	inx
	rts""")

add_word("1+",
"""	inc LSB, x
	bne +
	inc MSB, x
+	rts""")

