# C64 Cross Compiler

import os
import re
import sys

code_words = {}
OUT = None

class Ref:
	def __init__(self, addr):
		self.addr = addr

	def __index__(self):
		return self.addr

	def __int__(self):
		return self.addr

	# ?
	def __sub__(self, other):
		return self.addr - other
	def __rsub__(self, other):
		return other - self.addr
	def __add__(self, other):
		return other + self.addr
	def __radd__(self, other):
		return other + self.addr
	def __lt__(self, other):
		return self.addr < other
	def __eq__(self, other):
		return self.addr == other

word_hashes = []
def word_name_hash(word_name):
	if word_name not in word_hashes:
		word_hashes.append(word_name)
	return "WORD_" + str(word_hashes.index(word_name))

def compile(words_, xt_words_, heap_, start_word, outfile):
	global words
	global xt_words
	global heap
	global OUT

	OUT = open(outfile, "w")

	words = words_
	xt_words = xt_words_
	heap = heap_

	words_to_export.append(start_word)

	write_header()

	while True:
		if words_to_export:
			export_word(words_to_export.pop())
			continue
		if primitives_to_add:
			add_primitive(primitives_to_add.pop())
			continue
		if doers_to_export:
			export_doer(doers_to_export.pop())
			continue
		break

words_to_export = []
exported_words = set()

doers_to_export = []
exported_doers = set()

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
	elif "DOES_TO" in s:
		compile_does_word(w)
	else:
		sys.exit("Unknown xt " + str(w.xt))

def compile_constant_word(w):
	OUT.write(word_name_hash(w.name) + "\t; " + w.name + "\n")
	OUT.write("\tldy\t#" + str((w.constant_value >> 8) & 0xff) + "\n")
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
		OUT.write("!byte\t" + str(heap[i]) + '\n')

	OUT.write('\n')

def compile_colon_word(w):
	OUT.write(word_name_hash(w.name) + "\t; " + w.name + "\n")
	compile_body(w)

def compile_body(w, start_ip = -1):
	ip = w.body if start_ip == -1 else start_ip
	while ip < w.body_end:
		OUT.write("IP_" + str(ip) + ":\n")
		cell = heap[ip]
		if callable(cell):
			cell_word = xt_words[cell]
			ip = compile_call(cell_word, ip)
		elif type(cell) == Ref:
			OUT.write("\t!word IP_" + str(cell.addr) + "\n")
			ip += 1
		elif type(cell) == type(0):
			compile_byte(cell)
		elif cell == None:
			compile_byte(0)
		else:
			sys.exit("Unknown cell type " + str(cell))
		ip += 1

def compile_does_word(w):
	add_primitive_dependency("dodoes")
	OUT.write(word_name_hash(w.name) + "\t; " + w.name + "\n")
	OUT.write("\tjsr " + word_name_hash("dodoes") + "\t; dodoes\n")
	OUT.write("\t!word IP_" + str(w.xt_ip) + "\n")
	compile_body(w)
	doers_to_export.append(w.xt_ip)

def compile_byte(cell):
	OUT.write("\t!byte " + str(cell) + "\n")

def compile_jsr(callee):
	if callee not in words_to_export:
		words_to_export.append(callee)
	OUT.write("\tjsr " + word_name_hash(callee.name) + "\t; " + callee.name + "\n")

def compile_call(callee, ip):
	if callee.name == "exit":
		OUT.write("\trts\n\n")
	elif callee.name == "branch":
		addr = heap[ip + 1] + (heap[ip + 2] << 8)
		ip += 2
		OUT.write("\tjmp IP_" + str(addr) + "\t\t; branch\n")
	elif callee.name == "0branch" or callee.name == "(loop)" or callee.name == "(+loop)":
		addr = heap[ip + 1] + (heap[ip + 2] << 8)
		compile_jsr(callee)
		ip += 2
		OUT.write("\t!word\tIP_" + str(addr) + "\n")
	elif callee.name == "drop":
		OUT.write("\tinx\t\t\t; drop\n")
	elif callee.name == "2drop":
		OUT.write("\tinx\t\t\t; 2drop\n")
		OUT.write("\tinx\n")
	elif callee.name == "litc":
		compile_jsr(callee)
		ip += 1
		compile_byte(heap[ip])
	elif callee.name == "lit":
		compile_jsr(callee)
		ip += 1
		if callable(heap[ip]):
			word = xt_words[heap[ip]]
			if word not in words_to_export:
				words_to_export.append(word)
			OUT.write("\t!word " + word_name_hash(word.name) + "\t; " + word.name + "\n")
			ip += 1
		else:
			compile_byte(heap[ip])
			ip += 1
			compile_byte(heap[ip])
	elif callee.name == "sliteral":
		compile_jsr(callee)
		ip += 1
		strlen = heap[ip]
		OUT.write("\t!byte\t" + str(strlen) + '\n')
		OUT.write('\t!text\t"')
		for i in range(strlen):
			ip += 1
			OUT.write(chr(heap[ip]))
		OUT.write('"\n')
	else:
		compile_jsr(callee)
	return ip

def add_primitive(word_name):
	if word_name in added_primitives:
		return
	added_primitives.add(word_name)

	if word_name in code_words:
		OUT.write(word_name_hash(word_name) + "\t; " + word_name + "\n")
		# Expands %FORTH_WORD% to the corresponding assembly label.
		pattern = re.compile("(.*)%(.*)%(.*)")
		for line in code_words[word_name].split('\n'):
			m = pattern.match(line)
			if m:
				pre,word,post = m.groups()
				line = pre + word_name_hash(word) + post + "\t; " + word
				if word not in added_primitives:
					primitives_to_add.append(word)
			OUT.write(line + "\n")
		OUT.write("\n")
	else:
		for w in words.values():
			if w.name == word_name and w.body:
				export_word(w)
				return
		sys.exit("Missing >>>" + word_name + "<<<")

def write_header():
	location = os.path.realpath(os.path.join(os.getcwd(), os.path.dirname(__file__)))
	asm_header_path = os.path.join(location, "src/header.asm")
	OUT.write(open(asm_header_path, "r").read())

def export_doer(ip):
	for w in words.values():
		if w.body and w.body_end and w.body <= ip and ip < w.body_end:
			OUT.write("\t;doer " + w.name + "\n")
			compile_body(w, ip)
			return
	assert False
