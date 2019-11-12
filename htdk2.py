#!/usr/bin/python3

import sys

class Word:
	def __init__(self, name, xt, immediate, ip):
		self.name = name
		self.ip = ip
		self.xt = xt
		self.immediate = immediate

	def __repr__(self):
		return self.name

heap = [0] * 80
base = 10
state = False
words = {}
stack = []
control_stack = []
return_stack = []
tib = 0
here = 0
to_in = 0
tib_count = 0
latest = None
ip = 0

digits = "0123456789abcdefghijklmnopqrstuvwxyz"

def add_word(name, xt, immediate = False):
	words[name] = Word(name, xt, immediate, -1)

def is_number(word):
	for d in word:
		if d not in digits:
			return False
		if digits.index(d) >= base:
			return False
	return True

def evaluate_number(word):
	global stack
	number = 0
	for d in word:
		number *= base
		number += digits.index(d)
	stack += [number]

def evaluate(word):
	if word in words:
		words[word].xt()
	else:
		if is_number(word):
			evaluate_number(word)
		else:
			sys.exit("unknown word '" + word + "'")

def REFILL():
	global tib
	global to_in
	global tib_count
	tib_count = 0
	for c in input():
		heap[tib + tib_count] = c
		tib_count += 1
	to_in = 0

def read_word():
	global to_in
	global tib

	while True:
		# skips leading whitespace
		while to_in < tib_count:
			if heap[tib + to_in] == ' ':
				to_in += 1
			else:
				break

		# reads the word
		word = ""
		while to_in < tib_count:
			c = heap[tib + to_in]
			to_in += 1
			if c == ' ':
				break
			word += c

		if word:
			return word

		# word not found, refill and try again
		REFILL()

def VARIABLE():
	l = len(heap)
	name = read_word().lower()
	words[name] = Word(name, lambda : stack.append(l), False, len(heap))
	heap.append(None)

def compile(word):
	global stack
	if word in words:
		if words[word].immediate:
			words[word].xt()
		else:
			heap.append(words[word])
	else:
		if is_number(word):
			evaluate_number(word)
			heap.append(stack[-1])
			DROP()
		else:
			sys.exit("unknown word '" + word + "'")

def interpret():
	global tib
	global to_in
	while True:
		word = read_word().lower()
		print(word)
		if state:
			compile(word)
		else:
			evaluate(word)

def HEX():
	base = 16

def STORE():
	heap[stack[-1]] = stack[-2]
	DROP()
	DROP()

def docol(word):
	print("DOCOL")
	global ip
	ip = word.ip
	while True:
		code = heap[ip]
		if type(code) == Word:
			print("exec " + code.name)
			code.xt()
		else:
			print(code)
			sys.exit("What?")
		ip += 1
	sys.exit("DOCOL")

def CREATE():
	global latest
	latest = read_word().lower()
	words[latest] = Word(latest, lambda : docol(words[latest]), False, len(heap))

def DEPTH():
	return len(stack)

def COLON():
	global state
	CREATE()
	state = True

def SEMICOLON():
	global state
	heap.append(words["exit"])
	state = False

def DROP():
	stack.pop()

def DUP():
	stack.push(stack[-1])

def QDUP():
	if stack[-1]:
		DUP()

def BRANCH():
	global ip
	ip = heap[ip + 1]
	print(ip)

def ZBRANCH():
	global ip
	if stack[-1]:
		print("..skip")
		ip += 1
	else:
		print("..branch")
		BRANCH()

def IF():
	heap.append(words["0branch"])
	control_stack.append(len(heap))
	heap.append(0)

def ELSE():
	heap.append(words["branch"])
	heap.append(0)
	heap[control_stack[-1]] = len(heap)
	control_stack[-1] = len(heap) - 1

def THEN():
	heap[control_stack[-1]] = len(heap)
	control_stack.pop()

def ZLESS():
	return stack[-1] < 0

def NEGATE():
	stack[-1] = -stack[-1]

def QUIT():
	while True:
		REFILL()
		interpret()

def _DO():
	sys.exit("(do)")

def DO():
	heap.append(words["(do)"])
	control_stack.append(len(heap))

def _LOOP():
	sys.exit("(loop)")

def LOOP():
	heap.append(words["(loop)"])
	heap.append(control_stack[-1])
	control_stack.pop()

def CR():
	print()

def CELLS():
	stack.append(1)

def ALLOT():
	for i in range(stack[-1]):
		heap.append(0)
	stack.pop()

def SQUOTE():
	global to_in
	s = ""
	while to_in < tib_count:
		c = heap[tib + to_in]
		to_in += 1
		if c == '"':
			break
		s += c
	heap.append(words["sliteral"])
	heap.append(len(s))
	for c in s:
		heap.append(c)

def SOURCE():
	stack.append(tib)
	stack.append(tib_count)

def FETCH():
	stack[-1] = heap[stack[-1]]

add_word("\\", REFILL, True)
add_word("hex", HEX)
add_word("variable", VARIABLE)
add_word("!", STORE)
add_word(":", COLON)
add_word(";", SEMICOLON, True)
add_word("depth", DEPTH)
add_word("?dup", QDUP)
add_word("dup", DUP)
add_word("drop", DROP)
add_word("0<", ZLESS)
add_word("0branch", ZBRANCH)
add_word("branch", BRANCH)
add_word("negate", NEGATE)
add_word("if", IF, True)
add_word("else", ELSE, True)
add_word("then", THEN, True)
add_word("cr", CR)
add_word("i", lambda : sys.exit("i"))
add_word("=", lambda : sys.exit("="))
add_word("0=", lambda : sys.exit("0="))
add_word('s"', SQUOTE, True)
add_word("do", DO, True)
add_word("(do)", _DO)
add_word("loop", LOOP, True)
add_word("(loop)", _LOOP)
add_word("exit", lambda : sys.exit("exit"))
add_word("type", lambda : sys.exit("type"))
add_word("source", SOURCE)
add_word("@", FETCH)
add_word("+", lambda : sys.exit("+"))
add_word("cells", CELLS)
add_word("quit", QUIT)
add_word("create", CREATE)
add_word("allot", ALLOT)
add_word("sliteral", lambda : sys.exit("sliteral"))
add_word("leave", lambda : sys.exit("leave"))
add_word(">r", lambda : sys.exit(">r"))
add_word("r>", lambda : sys.exit("r>"))
add_word(">in", lambda : sys.exit(">in"))
add_word("[char]", lambda : sys.exit("[char]"))
add_word("*", lambda : sys.exit("*"))
add_word("emit", lambda : sys.exit("emit"))

QUIT()
