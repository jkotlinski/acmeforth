# C64 Cross Compiler

import os
import re
import sys

NEWLINE = 256

OUT = None

refs = {}

to_petscii = [
	0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0a,0x0b,0x0c,0x0d,0x0e,0x0f,
	0x10,0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,0x1a,0x1b,0x1c,0x1d,0x1e,0x1f,
	0x20,0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,0x2a,0x2b,0x2c,0x2d,0x2e,0x2f,
	0x30,0x31,0x32,0x33,0x34,0x35,0x36,0x37,0x38,0x39,0x3a,0x3b,0x3c,0x3d,0x3e,0x3f,
	0x40,0xc1,0xc2,0xc3,0xc4,0xc5,0xc6,0xc7,0xc8,0xc9,0xca,0xcb,0xcc,0xcd,0xce,0xcf,
	0xd0,0xd1,0xd2,0xd3,0xd4,0xd5,0xd6,0xd7,0xd8,0xd9,0xda,0x5b,0x5c,0x5d,0x5e,0x5f,
	0xc0,0x41,0x42,0x43,0x44,0x45,0x46,0x47,0x48,0x49,0x4a,0x4b,0x4c,0x4d,0x4e,0x4f,
	0x50,0x51,0x52,0x53,0x54,0x55,0x56,0x57,0x58,0x59,0x5a,0xdb,0xdc,0xdd,0xde,0xdf,
	0x80,0x81,0x82,0x83,0x84,0x85,0x86,0x87,0x88,0x89,0x8a,0x8b,0x8c,0x8d,0x8e,0x8f,
	0x90,0x91,0x92,0x0c,0x94,0x95,0x96,0x97,0x98,0x99,0x9a,0x9b,0x9c,0x9d,0x9e,0x9f,
	0xa0,0xa1,0xa2,0xa3,0xa4,0xa5,0xa6,0xa7,0xa8,0xa9,0xaa,0xab,0xac,0xad,0xae,0xaf,
	0xb0,0xb1,0xb2,0xb3,0xb4,0xb5,0xb6,0xb7,0xb8,0xb9,0xba,0xbb,0xbc,0xbd,0xbe,0xbf,
	0x60,0x61,0x62,0x63,0x64,0x65,0x66,0x67,0x68,0x69,0x6a,0x6b,0x6c,0x6d,0x6e,0x6f,
	0x70,0x71,0x72,0x73,0x74,0x75,0x76,0x77,0x78,0x79,0x7a,0x7b,0x7c,0x7d,0x7e,0x7f,
	0xe0,0xe1,0xe2,0xe3,0xe4,0xe5,0xe6,0xe7,0xe8,0xe9,0xea,0xeb,0xec,0xed,0xee,0xef,
	0xf0,0xf1,0xf2,0xf3,0xf4,0xf5,0xf6,0xf7,0xf8,0xf9,0xfa,0xfb,0xfc,0xfd,0xfe,0xff,
	0xd # \n
]

class Ref:
	def __init__(self, addr, word = None):
		self.addr = addr
		self.word = word
		if word:
			if not addr in refs:
				refs[addr] = []
			refs[addr].append(word)

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
		if type(other) == Ref:
			return self.addr < other.addr
		else:
			return self.addr < other
	def __eq__(self, other):
		return self.addr == other

word_hashes = []
def word_name_hash(word_name):
	if word_name not in word_hashes:
		word_hashes.append(word_name)
	return "WORD_" + str(word_hashes.index(word_name))

def compile(dictionary_, heap_, start_word_name, outfile):
	global dictionary
	global heap
	global OUT

	OUT = open(outfile, "w")

	dictionary = dictionary_
	heap = heap_

	words_to_export.append(dictionary.words[start_word_name])

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
	elif "HERE" in s:
		OUT.write("; raw data area\n")
		compile_body(w)
	else:
		sys.exit("Unknown xt " + str(w.xt))

def compile_constant_word(w):
	OUT.write(word_name_hash(w.name) + "\t; " + w.name + "\n")
	if type(w.constant_value) == Ref:
		OUT.write("\tldy\t#>REF_" + str(w.constant_value.addr) + "_W_" + str(w.constant_value.word.body) + "\n")
		OUT.write("\tlda\t#<REF_" + str(w.constant_value.addr) + "_W_" + str(w.constant_value.word.body) + "\n")
		if w.constant_value.word:
			if w.constant_value.word not in words_to_export:
				words_to_export.append(w.constant_value.word)
	elif type(w.constant_value) == type(0):
		OUT.write("\tldy\t#" + str((w.constant_value >> 8) & 0xff) + "\n")
		OUT.write("\tlda\t#" + str(w.constant_value & 0xff) + "\n")
	elif callable(w.constant_value):
		word = dictionary.xt_words[w.constant_value]
		if word not in words_to_export:
			words_to_export.append(word)
		OUT.write("\tldy\t#>" + word_name_hash(word.name) + "\t; " + word.name + "\n")
		OUT.write("\tlda\t#<" + word_name_hash(word.name) + "\t; " + word.name + "\n")
	else:
		print(w.constant_value)
		assert False
	OUT.write("\tjmp\t" + word_name_hash("pushya") + "\t; pushya\n\n")
	add_primitive_dependency("pushya")

def compile_create_word(w):
	OUT.write(word_name_hash(w.name) + "\t; " + w.name + "\n")
	OUT.write("\tldy\t#>IP_" + str(w.body) + "\n")
	OUT.write("\tlda\t#<IP_" + str(w.body) + "\n")
	OUT.write("\tjmp\t" + word_name_hash("pushya") + "\t; pushya\n")
	OUT.write("IP_" + str(w.body) + '\n')
	add_primitive_dependency("pushya")

	for i in range(w.body, w.body_end):
		if type(heap[i]) == type(0):
			OUT.write("\t!byte\t" + str(heap[i]) + '\n')
		elif callable(heap[i]):
			word = dictionary.xt_words[heap[i]]
			if word not in words_to_export:
				words_to_export.append(word)
			OUT.write("\t!word " + word_name_hash(word.name) + "\t; " + word.name + "\n")
		else:
			assert False

	OUT.write('\n')

def compile_colon_word(w):
	OUT.write(word_name_hash(w.name) + "\t; " + w.name + "\n")
	compile_body(w)

def compile_body(w, start_ip = -1):
	ip = w.body if start_ip == -1 else start_ip
	while ip < w.body_end:
		if ip in refs:
			if w in refs[ip]:
				OUT.write("REF_" + str(ip) + "_W_" + str(w.body) + "\n")
		OUT.write("IP_" + str(ip) + "\n")
		cell = heap[ip]
		if callable(cell):
			cell_word = dictionary.xt_words[cell]
			ip = compile_call(cell_word, ip)
		elif type(cell) == Ref:
			OUT.write("\t!word IP_" + str(cell.addr) + "\n")
			ip += 1
		elif type(cell) == int:
			compile_byte(cell)
		else:
			sys.exit("Unknown cell type " + str(cell))
		ip += 1
	if ip in refs:
		if w in refs[ip]:
			OUT.write("REF_" + str(ip) + "_W_" + str(w.body) + "\n")
	OUT.write("\n")

def compile_does_word(w):
	add_primitive_dependency("dodoes")
	OUT.write(word_name_hash(w.name) + "\t; " + w.name + "\n")
	OUT.write("\tjsr " + word_name_hash("dodoes") + "\t; dodoes\n")
	OUT.write("\t!word IP_" + str(w.xt_ip) + "\n")
	compile_body(w)
	doers_to_export.append(w.xt_ip)

def compile_byte(cell):
	if type(cell) == str:
		OUT.write("\t!byte " + str(to_petscii[ord(cell)]) + "\n")
	else:
		OUT.write("\t!byte " + str(cell) + "\n")

def compile_jsr(callee):
	if callee not in words_to_export:
		words_to_export.append(callee)
	OUT.write("\tjsr " + word_name_hash(callee.name) + "\t; " + callee.name + "\n")

def compile_call(callee, ip):
	if callee.name == "exit":
		# TODO tail-call optimization
		OUT.write("\trts\n\n")
	elif callee.name == "branch":
		if type(heap[ip + 1]) == Ref:
			addr = heap[ip + 1].addr
		else:
			addr = heap[ip + 1] + (heap[ip + 2] << 8)
		ip += 2
		OUT.write("\tjmp IP_" + str(addr) + "\t\t; branch\n")
	elif callee.name == "0branch" or callee.name == "(loop)" or callee.name == "(+loop)":
		if type(heap[ip + 1]) == Ref:
			addr = heap[ip + 1].addr
		else:
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
		compile_byte(heap.getchar(ip))
	elif callee.name == "lit":
		compile_jsr(callee)
		ip += 1
		val = heap.getchar(ip)
		if callable(val):
			word = dictionary.xt_words[heap[ip]]
			if word not in words_to_export:
				words_to_export.append(word)
			OUT.write("\t!word " + word_name_hash(word.name) + "\t; " + word.name + "\n")
			ip += 1
		elif type(val) == Ref:
			ref = heap[ip]
			if ref.word and ref.word not in words_to_export:
				words_to_export.append(ref.word)
			OUT.write("\t!word IP_" + str(ref.addr) + "\t; " + str(ref.word) + "\n")
			ip += 1
		else:
			compile_byte(val)
			ip += 1
			compile_byte(heap[ip])
	elif callee.name == "sliteral":
		compile_jsr(callee)
		ip += 1
		strlen = heap[ip]
		OUT.write("\t!byte\t" + str(strlen) + '\n')
		for i in range(strlen):
			ip += 1
			write_char(heap.getchar(ip))
	else:
		compile_jsr(callee)
	return ip

def write_char(c):
	if type(c) == str:
		OUT.write("\t!byte\t" + str(to_petscii[ord(c)]) + "\n")
	else:
		OUT.write("\t!byte\t" + str(c) + "\n")

def add_primitive(word_name):
	if word_name in added_primitives:
		return
	added_primitives.add(word_name)

	if word_name in dictionary.code_words:
		OUT.write(word_name_hash(word_name) + "\t; " + word_name + "\n")
		# Expands %FORTH_WORD% to the corresponding assembly label.
		pattern = re.compile("(.*)%(.*)%(.*)")
		for line in dictionary.code_words[word_name].split('\n'):
			m = pattern.match(line)
			if m:
				pre,word,post = m.groups()
				line = pre + word_name_hash(word) + post + "\t; " + word
				if word not in added_primitives:
					primitives_to_add.append(word)
			OUT.write(line + "\n")
		OUT.write("\n")
	else:
		for w in dictionary.words.values():
			if w.name == word_name and w.body:
				export_word(w)
				return
		sys.exit("Missing >>>" + word_name + "<<<")

def write_header():
	location = os.path.realpath(os.path.join(os.getcwd(), os.path.dirname(__file__)))
	asm_header_path = os.path.join(location, "src/header.asm")
	OUT.write(open(asm_header_path, "r").read() + "\n")

def export_doer(ip):
	if ip in exported_doers:
		return
	exported_doers.add(ip)
	for w in dictionary.words.values():
		if w.body and w.body_end and w.body <= ip and ip < w.body_end:
			OUT.write("\t;doer " + w.name + "\n")
			compile_body(w, ip)
			return
	assert False
