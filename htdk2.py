#!/usr/bin/python3

import sys

ram = []

class Word:
	def __init__(self, fn, immediate = False):
		self.fn = fn
		self.immediate = immediate
		self.bytecode = []

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
	words[read_word().lower()] = Word(lambda : stack.append(l))
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
			print("append " + word)
			print(bytecode)
	else:
		if is_number(word):
			evaluate_number(word)
			bytecode.append(stack[-1])
			stack.pop()
		else:
			sys.exit("unknown word '" + word + "'")
	

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
	stack.pop()
	stack.pop()

def docol(word):
	print("DOCOL")

def create():
	global latest
	latest = read_word().lower()
	w = Word(latest)
	w.fn = lambda : docol(w)
	words[latest] = w

def DEPTH():
	return len(stack)

def COLON():
	global state
	create()
	state = True

def DUP():
	stack.push(stack[-1])

def QDUP():
	if stack[-1]:
		DUP()

def ZBRANCH():
	global ip
	ip += 1
	if not stack[-1]:
		ip += bytecode[ip]

def IF():
	b = words[latest].bytecode
	b.append(words["0branch"])
	b.append(0)
	control_stack.append(len(b))

def ZLESS():
	return stack[-1] < 0

def NEGATE():
	stack[-1] = -stack[-1]

def QUIT():
	while True:
		REFILL()
		interpret()

words["\\"] = Word(REFILL, True)
words["hex"] = Word(HEX)
words["variable"] = Word(VARIABLE)
words["!"] = Word(STORE)
words[":"] = Word(COLON)
words["depth"] = Word(DEPTH)
words["?dup"] = Word(QDUP)
words["dup"] = Word(DUP)
words["0<"] = Word(ZLESS)
words["0branch"] = Word(ZBRANCH)
words["negate"] = Word(NEGATE)
words["if"] = Word(IF, True)

QUIT()
