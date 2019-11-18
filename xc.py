# C64 Cross Compiler

import asm
import sys

label_counter = 1

def add_label(comment):
	global label_counter
	print("WORD_" + label_counter + ":\t; " + comment)
	label_counter += 1

def compile(xt_words_, heap_, start_word):
	global xt_words
	global heap
	xt_words = xt_words_
	heap = heap_

	words_to_export.append(start_word)

	print_header(start_word.name)

	while words_to_export:
		word = words_to_export.pop()
		export_word(word)

	print_footer()

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
		include_assembly(w)

def compile_forth_word(w):
	ip = w.body
	while ip < w.body_end:
		print("IP_" + str(ip) + ":")
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
		print("\tjsr LIT")
		print("\t!word " + str(n))
	else:
		print("\tjsr LITC")
		print("\t!byte " + str(n))

def compile_call(callee, ip):
	if callee.name == "exit":
		print("\trts\n")
	elif callee.name == "branch":
		ip += 1
		print("\tjmp IP_" + str(heap[ip]))
	else:
		words_to_export.append(callee)
		print("\tjsr W" + callee.hash() + " ; " + callee.name)
	return ip

def include_assembly(w):
	print("W" + w.hash() + "\t; " + w.name)
	if w.name in asm.asm:
		print(asm.asm[w.name])
		print()
	else:
		sys.exit("Missing 6510 assembly definition for '" + w.name + "'")
	
def print_header(start_word_name):
	print('!to "' + start_word_name + '.prg", cbm   ; set output file and format')
	print("""; Compile with ACME assembler

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

def print_footer():
	print("""
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
