# C64 Cross Compiler

import primitives
import re
import sys

label_counter = 1

OUT = None

word_hashes = []
def word_name_hash(word_name):
	if word_name not in word_hashes:
		word_hashes.append(word_name)
	return "WORD_" + str(word_hashes.index(word_name))

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
		export_word(words_to_export.pop())

	while primitives_to_add:
		add_primitive(primitives_to_add.pop())

	write_footer()

exported_words = set()
words_to_export = []

primitives_to_add = []
added_primitives = set()

def add_primitive_dependency(word_name):
	if word_name not in added_primitives:
		primitives_to_add.append(word_name)

def export_word(w):
	if w in exported_words:
		return
	exported_words.add(w)

	xt = w.xt

	if w.body != None:
		compile_forth_word(w)
	else:
		add_primitive_dependency(w.name)

def compile_forth_word(w):
	s = str(w.xt)
	if "COLON" in s:
		compile_colon_word(w)
	elif "CREATE" in s:
		compile_create_word(w)
	elif "CONSTANT" in s:
		compile_constant_word(w)
	else:
		sys.exit("Unknown xt " + str(w.xt))

def compile_constant_word(w):
	OUT.write(word_name_hash(w.name) + "\t; " + w.name + "\n")
	OUT.write("\tldy\t#" + str(w.constant_value >> 8) + "\n")
	OUT.write("\tlda\t#" + str(w.constant_value & 0xff) + "\n")
	OUT.write("\tjmp\t" + word_name_hash("pushya") + "\t; pushya\n")
	add_primitive_dependency("pushya")

def compile_create_word(w):
	OUT.write(word_name_hash(w.name) + "\t; " + w.name + "\n")
	OUT.write("\tldy\t#>IP_" + str(w.body) + "\n")
	OUT.write("\tlda\t#<IP_" + str(w.body) + "\n")
	OUT.write("\tjmp\t" + word_name_hash("pushya") + "\t; pushya\n")
	OUT.write("IP_" + str(w.body) + '\n')
	add_primitive_dependency("pushya")

	for i in range(w.body, w.body_end):
		if heap[i] == None:
			heap[i] = 0
		OUT.write("!word\t" + str(heap[i]) + '\n')

	OUT.write('\n')

def compile_colon_word(w):
	OUT.write(word_name_hash(w.name) + "\n")
	ip = w.body
	while ip < w.body_end:
		OUT.write("IP_" + str(ip) + ":\n")
		cell = heap[ip]
		if callable(cell):
			cell_word = xt_words[cell]
			ip = compile_call(cell_word, ip)
		elif type(cell) == type(0):
			compile_number(cell)
		elif cell == None:
			compile_number(0)
		else:
			sys.exit("Unknown cell type " + str(cell))
		ip += 1

def compile_number(n):
	if n and 0xff00:
		add_primitive_dependency("lit")
		OUT.write("\tjsr " + word_name_hash("lit") + "\t; lit\n")
		OUT.write("\t!word " + str(n) + "\n")
	else:
		add_primitive_dependency("litc")
		OUT.write("\tjsr " + word_name_hash("litc") + "\t; litc\n")
		OUT.write("\t!byte " + str(n) + "\n")

def compile_jsr(callee):
	if callee not in words_to_export:
		words_to_export.append(callee)
	OUT.write("\tjsr " + word_name_hash(callee.name) + "\t; " + callee.name + "\n")

def compile_call(callee, ip):
	if callee.name == "exit":
		OUT.write("\trts\n\n")
	elif callee.name == "branch":
		ip += 1
		OUT.write("\tjmp IP_" + str(heap[ip]) + "\t\t; branch\n")
	elif callee.name == "0branch" or callee.name == "(loop)":
		compile_jsr(callee)
		ip += 1
		OUT.write("\t!word\tIP_" + str(heap[ip]) + "\n")
	elif callee.name == "drop":
		OUT.write("\tinx\t\t\t; drop\n")
	elif callee.name == "2drop":
		OUT.write("\tinx\t\t\t; 2drop\n")
		OUT.write("\tinx\n")
	else:
		compile_jsr(callee)
	return ip

def add_primitive(word_name):
	if word_name in added_primitives:
		return
	added_primitives.add(word_name)

	OUT.write(word_name_hash(word_name) + "\t; " + word_name + "\n")
	if word_name in primitives.asm:
		# Expands %FORTH_WORD% to the corresponding assembly label.
		pattern = re.compile("(.*)%(.*)%(.*)")
		for line in primitives.asm[word_name].split('\n'):
			m = pattern.match(line)
			if m:
				pre,word,post = m.groups()
				line = pre + word_name_hash(word) + post + "\t; " + word
				if word not in added_primitives:
					primitives_to_add.append(word)
			OUT.write(line + "\n")
		OUT.write("\n")
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
	OUT.write("""BYE
INIT_S = * + 1
	ldx	#0
	txs
	rts""")
