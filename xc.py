# C64 Cross Compiler

import primitives
import sys

label_counter = 1

OUT = None

def add_label(comment):
	global label_counter
	OUT.write("WORD_" + label_counter + ":\t; " + comment + "\n")
	label_counter += 1

def compile(xt_words_, heap_, start_word, outfile):
	global xt_words
	global heap
	global OUT

	OUT = open(outfile, "w")

	xt_words = xt_words_
	heap = heap_

	words_to_export.append(start_word)

	write_header()

	while words_to_export:
		word = words_to_export.pop()
		export_word(word)

	write_footer()

exported_words = []
words_to_export = []

def export_word(w):
	if w in exported_words:
		return
	exported_words.append(w)

	xt = w.xt

	if w.body:
		compile_forth_word(w)
	else:
		add_primitive(w.name)

def compile_forth_word(w):
	ip = w.body
	while ip < w.body_end:
		OUT.write("IP_" + str(ip) + ":\n")
		cell = heap[ip]
		if callable(cell):
			cell_word = xt_words[cell]
			ip = compile_call(cell_word, ip)
		elif type(cell) == type(0):
			compile_number(cell)
		else:
			sys.exit("Unknown cell type", cell)
		ip += 1

def compile_number(n):
	if n and 0xff00:
		OUT.write("\tjsr LIT\n")
		OUT.write("\t!word " + str(n) + "\n")
	else:
		OUT.write("\tjsr LITC\n")
		OUT.write("\t!byte " + str(n) + "\n")

def compile_call(callee, ip):
	if callee.name == "exit":
		OUT.write("\trts\n\n")
	elif callee.name == "branch":
		ip += 1
		OUT.write("\tjmp IP_" + str(heap[ip]) + "\n")
	else:
		words_to_export.append(callee)
		OUT.write("\tjsr W" + word_name_hash(callee.name) + " ; " + callee.name + "\n")
	return ip

def word_name_hash(word_name):
	return hex(abs(hash(word_name)))[2:]

def add_primitive(word_name):
	OUT.write("W" + word_name_hash(word_name) + "\t; " + word_name + "\n")
	if word_name in primitives.asm:
		OUT.write(primitives.asm[word_name])
		OUT.write("\n\n")
	else:
		sys.exit("Missing 6510 assembly definition for '" + word_name + "'")
	
def write_header():
	OUT.write("""; Compile with ACME assembler

!cpu 6510

* = $801

!byte $b, $08, $a, 0, $9E, $32, $30, $36, $31, 0, 0, 0 ; basic header

; Parameter stack
; The x register contains the current stack depth.
; It is initially 0 and decrements when items are pushed.
; The parameter stack is placed in zeropage to save space.
; (E.g. lda $FF,x takes less space than lda $FFFF,x)
; We use a split stack that store low-byte and high-byte
; in separate ranges on the zeropage, so that popping and
; pushing gets faster (only one inx/dex operation).
X_INIT = 0
MSB = $73 ; high-byte stack placed in [$3b .. $72]
LSB = $3b ; low-byte stack placed in [3 .. $3a]

W = $8b ; rnd seed
W2 = $8d ; rnd seed
W3 = $9e ; tape error log

OP_JMP = $4c
OP_JSR = $20
OP_RTS = $60
OP_INX = $e8

PUTCHR = $ffd2 ; put char

K_RETURN = $d
K_CLRSCR = $93
K_SPACE = ' '

!ct pet

; -------- program start

    tsx
    stx INIT_S
    ldx #X_INIT

""")

def write_footer():
	OUT.write("""
LITC
    dex

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
+   jmp (W)

BYE
INIT_S = * + 1
	ldx	#0
	txs
	rts

LIT
    dex

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
+   jmp $1234""")
