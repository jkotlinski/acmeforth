#!/usr/bin/python3

import ctypes
import sys

DEBUG = False

util = ctypes.cdll.LoadLibrary("./libutil.so")

class Word:
	def __init__(self, name, xt, immediate):
		self.name = name
		self.xt = xt
		self.immediate = immediate

	def __repr__(self):
		return self.name

heap = [None] * 200
base = 10
state = False
words = {}
stack = []
control_stack = []
return_stack = []
tib = 1
here = 0
tib_count = 0
latest = None
ip = 0
to_in_addr = 0

digits = "0123456789abcdefghijklmnopqrstuvwxyz"

def TO_IN():
	stack.append(to_in_addr)

def add_word(name, xt, immediate = False):
	words[name] = Word(name, xt, immediate)

def is_number(word):
	if word[0] == '-':
		word = word[1:]
	for d in word:
		if d not in digits:
			return False
		if digits.index(d) >= base:
			return False
	return True

def evaluate_number(word):
	global stack
	number = 0
	negate = word[0] == '-'
	if negate:
		word = word[1:]
	for d in word:
		number *= base
		number += digits.index(d)
	if negate:
		number = -number
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
	global tib_count
	tib_count = 0
	for c in input():
		heap[tib + tib_count] = c
		tib_count += 1
	heap[to_in_addr] = 0

def read_word():
	global tib

	while True:
		# skips leading whitespace
		while heap[to_in_addr] < tib_count:
			if heap[tib + heap[to_in_addr]] == ' ':
				heap[to_in_addr] += 1
			else:
				break

		# reads the word
		word = ""
		while heap[to_in_addr] < tib_count:
			c = heap[tib + heap[to_in_addr]]
			heap[to_in_addr] += 1
			if c == ' ':
				break
			word += c

		if word:
			return word

		# word not found, refill and try again
		REFILL()

def VARIABLE():
	CREATE()
	words[latest].xt = lambda l=len(heap) : stack.append(l)
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
	while True:
		word = read_word().lower()
		if DEBUG:
			print(word)
		if state:
			compile(word)
		else:
			evaluate(word)
		if DEBUG:
			print(stack)

def HEX():
	global base
	base = 16

def STORE():
	heap[stack[-1]] = stack[-2]
	DROP()
	DROP()

def docol(ip_):
	global ip
	ip = ip_
	while ip:
		code = heap[ip]
		ip += 1
		if type(code) == Word:
			if DEBUG:
				print("exec " + code.name)
			code.xt()
			if DEBUG:
				print(stack)
		else:
			stack.append(code)

def CREATE():
	global latest
	latest = read_word().lower()
	words[latest] = Word(latest, lambda i=len(heap) : stack.append(i), False)

def DEPTH():
	stack.append(len(stack))

def COLON():
	global state
	CREATE()
	words[latest].xt = lambda ip=len(heap) : docol(ip)
	state = True

def SEMICOLON():
	global state
	heap.append(words["exit"])
	state = False

def DROP():
	stack.pop()

def DUP():
	stack.append(stack[-1])

def QDUP():
	if stack[-1]:
		DUP()

def BRANCH():
	global ip
	ip = heap[ip]

def ZBRANCH():
	global ip
	if stack.pop():
		ip += 1
	else:
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
	stack[-1] = -1 if stack[-1] < 0 else 0

def NEGATE():
	stack[-1] = -stack[-1]

def QUIT():
	while True:
		REFILL()
		interpret()

def _DO():
	TO_R() # index
	TO_R() # limit

def I():
	stack.append(return_stack[-2])

def DO():
	heap.append(words["(do)"])
	control_stack.append(len(heap))

def _LOOP():
	global ip
	return_stack[-2] += 1
	if return_stack[-2] == return_stack[-1]:
		return_stack.pop()
		return_stack.pop()
		ip += 1
	else:
		ip = heap[ip]

def LOOP():
	heap.append(words["(loop)"])
	heap.append(control_stack[-1])
	control_stack.pop()

def CR():
	print()

def CELLS():
	pass

def ALLOT():
	for i in range(stack[-1]):
		heap.append(0)
	stack.pop()

def SLITERAL():
	global ip
	stack.append(ip + 1)
	stack.append(heap[ip])
	ip += heap[ip] + 1

def SQUOTE():
	s = ""
	while heap[to_in_addr] < tib_count:
		c = heap[tib + heap[to_in_addr]]
		heap[to_in_addr] += 1
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

def TO_R():
	return_stack.append(stack.pop())

def R_TO():
	stack.append(return_stack.pop())

def TYPE():
	print("".join(heap[stack[-2]:stack[-2]+stack[-1]]), end='', flush=True)
	DROP()
	DROP()

def EXIT():
	global ip
	if return_stack:
		ip = return_stack.pop()
	else:
		ip = None

def LPAREN():
	while True:
		while heap[to_in_addr] < tib_count:
			if heap[tib + heap[to_in_addr]] == ')':
				heap[to_in_addr] += 1
				return
			heap[to_in_addr] += 1
		REFILL()

def EQUALS():
	stack[-2] = -1 if stack[-1] == stack[-2] else 0
	stack.pop()

def PLUS():
	stack[-2] += stack[-1]
	stack.pop()

def ZEQUAL():
	stack[-1] = 0 if stack[-1] else -1

def AND():
	stack[-2] &= stack[-1]
	stack.pop()

def OR():
	stack[-2] |= stack[-1]
	stack.pop()

def XOR():
	stack[-2] ^= stack[-1]
	stack.pop()

def RSHIFT():
	l = ctypes.c_uint(stack[-2])
	l.value >>= stack[-1]
	stack.pop()
	stack[-1] = l.value

def LSHIFT():
	l = ctypes.c_int(stack[-2])
	l.value <<= stack[-1]
	stack[-2] = l.value
	stack.pop()

def INVERT():
	stack[-1] = ~stack[-1]

def CONSTANT():
	CREATE()
	words[latest].xt = lambda v=stack.pop() : stack.append(v)

def TWOMUL():
	stack.append(1)
	LSHIFT()

def TWODIV():
	stack[-1] >>= 1

def U_LESS_THAN():
	if stack[-1] < 0:
		stack[-1] += 2 ** 32
	if stack[-2] < 0:
		stack[-2] += 2 ** 32
	stack[-2] = -1 if stack[-2] < stack[-1] else 0
	stack.pop()

def LESS_THAN():
	stack[-2] = -1 if stack[-2] < stack[-1] else 0
	stack.pop()

def GREATER_THAN():
	stack[-2] = -1 if stack[-2] > stack[-1] else 0
	stack.pop()

def MIN():
	stack[-2] = min(stack[-2], stack[-1])
	stack.pop()

def MAX():
	stack[-2] = max(stack[-2], stack[-1])
	stack.pop()

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
add_word("i", I)
add_word("=", EQUALS)
add_word("0=", ZEQUAL)
add_word('s"', SQUOTE, True)
add_word("do", DO, True)
add_word("(do)", _DO)
add_word("loop", LOOP, True)
add_word("(loop)", _LOOP)
add_word("exit", EXIT)
add_word("type", TYPE)
add_word("source", SOURCE)
add_word("@", FETCH)
add_word("+", PLUS)
add_word("cells", CELLS)
add_word("quit", QUIT)
add_word("create", CREATE)
add_word("allot", ALLOT)
add_word("sliteral", SLITERAL)
add_word("leave", lambda : sys.exit("leave"))
add_word(">r", TO_R)
add_word("r>", R_TO)
add_word(">in", TO_IN)
add_word("[char]", lambda : sys.exit("[char]"))
add_word("*", lambda : sys.exit("*"))
add_word("emit", lambda : sys.exit("emit"))
add_word("(", LPAREN, True)
add_word("and", AND)
add_word("or", OR)
add_word("xor", XOR)
add_word("lshift", LSHIFT)
add_word("rshift", RSHIFT)
add_word("2*", TWOMUL)
add_word("2/", TWODIV)
add_word("invert", INVERT)
add_word("constant", CONSTANT)
add_word("<", LESS_THAN)
add_word(">", GREATER_THAN)
add_word("u<", U_LESS_THAN)
add_word("min", MIN)
add_word("max", MAX)

QUIT()
