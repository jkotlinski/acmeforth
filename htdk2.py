#!/usr/bin/python3

import sys

ram = []

class Word:
	def __init__(self, name, fn, immediate):
		self.name = name
		self.fn = fn
		self.immediate = immediate
		self.bytecode = []

	def __repr__(self):
		return self.name

base = 10
state = False
words = {}
stack = []
control_stack = []
tib = None
to_in = 0
latest = None
ip = 0

digits = "0123456789abcdefghijklmnopqrstuvwxyz"

def add_word(name, fn, immediate = False):
	words[name] = Word(name, fn, immediate)

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
		words[word].fn()
	else:
		if is_number(word):
			evaluate_number(word)
		else:
			sys.exit("unknown word '" + word + "'")

def REFILL():
	global tib
	global to_in
	tib = input()
	to_in = 0

def read_word():
	global to_in
	global tib

	while True:
		# skips leading whitespace
		while to_in < len(tib):
			if tib[to_in] == ' ':
				to_in += 1
			else:
				break

		# reads the word
		word = ""
		while to_in < len(tib):
			c = tib[to_in]
			if c == ' ':
				break
			word += c
			to_in += 1

		if word:
			return word

		# word not found, refill and try again
		REFILL()

def VARIABLE():
	l = len(ram)
	name = read_word().lower()
	words[name] = Word(name, lambda : stack.append(l), False)
	ram.append(None)

def compile(word):
	print("COMPILE " + word)
	global stack
	bytecode = words[latest].bytecode
	if word in words:
		if words[word].immediate:
			words[word].fn()
		else:
			bytecode.append(words[word])
	else:
		if is_number(word):
			evaluate_number(word)
			bytecode.append(stack[-1])
			DROP()
		else:
			sys.exit("unknown word '" + word + "'")
	print(bytecode)
	

def interpret():
	global tib
	global to_in
	while True:
		word = read_word().lower()
		if state:
			compile(word)
		else:
			evaluate(word)

def HEX():
	base = 16

def STORE():
	ram[stack[-1]] = stack[-2]
	DROP()
	DROP()

def docol(word):
	print("DOCOL")

def create():
	global latest
	latest = read_word().lower()
	words[latest] = Word(latest, lambda : docol(w), False)

def DEPTH():
	return len(stack)

def COLON():
	global state
	create()
	state = True

def SEMICOLON():
	global state
	words[latest].bytecode.append(words["exit"])
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
	ip = bytecode[ip + 1]

def ZBRANCH():
	global ip
	if stack[-1]:
		ip += 1
	else:
		BRANCH()

def IF():
	b = words[latest].bytecode
	b.append(words["0branch"])
	control_stack.append(len(b))
	b.append(0)

def ELSE():
	b = words[latest].bytecode
	b[control_stack[-1]] = len(b)
	b.append(words["branch"])
	control_stack[-1] = len(b)
	b.append(0)

def THEN():
	b = words[latest].bytecode
	b[control_stack[-1]] = len(b)
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
	b = words[latest].bytecode
	b.append(words["(do)"])
	control_stack.append(len(b))

def _LOOP():
	sys.exit("(loop)")

def LOOP():
	b = words[latest].bytecode
	b.append(words["(loop)"])
	b.append(control_stack[-1])
	control_stack.pop()

def CR():
	print()

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
add_word("do", DO, True)
add_word("cr", CR)
add_word("(do)", _DO)
add_word("loop", LOOP, True)
add_word("(loop)", _LOOP)
add_word("exit", lambda : sys.exit("exit"))

QUIT()
