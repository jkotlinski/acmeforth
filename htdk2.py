#!/usr/bin/python3

DEBUG = 0

import ctypes
import sys

class Word:
	def __init__(self, name, xt, immediate):
		self.name = name
		self.xt = xt
		self.immediate = immediate

	def __repr__(self):
		return self.name

heap = [None] * 200
base = 10
words = {}
stack = []
control_stack = []
return_stack = []
tib_count = 0
latest = None
ip = 0

# Forth variable space.
to_in_addr = 0
state_addr = 1
tib = 2

digits = "0123456789abcdefghijklmnopqrstuvwxyz"

def STATE():
	stack.append(state_addr)

def TO_IN():
	stack.append(to_in_addr)

def add_word(name, xt, immediate = False):
	words[name] = Word(name, xt, immediate)

def is_number(word):
	if word[0] == '-':
		word = word[1:]
		if not word:
			return False
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
			COMMA()
		else:
			sys.exit("unknown word '" + word + "'")

def interpret():
	global tib
	while True:
		word = read_word().lower()
		if DEBUG:
			print(word)
		if heap[state_addr]:
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
	TWODROP()

def TWOSTORE():
	a = stack.pop()
	heap[a] = stack.pop()
	heap[a + 1] = stack.pop()

def PLUSSTORE():
	heap[stack.pop()] += stack.pop()

def docol(ip_):
	global ip
	return_stack.append(ip)
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
	CREATE()
	words[latest].xt = lambda ip=len(heap) : docol(ip)
	heap[state_addr] = True

def SEMICOLON():
	heap.append(words["exit"])
	heap[state_addr] = False

def DROP():
	stack.pop()

def TWODROP():
	stack.pop()
	stack.pop()

def DUP():
	stack.append(stack[-1])

def TWODUP():
	stack.append(stack[-2])
	stack.append(stack[-2])

def OVER():
	stack.append(stack[-2])

def TWOOVER():
	stack.append(stack[-4])
	stack.append(stack[-4])

def ROT():
	t = stack[-3]
	stack[-3] = stack[-2]
	stack[-2] = stack[-1]
	stack[-1] = t

def SWAP():
	t = stack[-1]
	stack[-1] = stack[-2]
	stack[-2] = t

def TWOSWAP():
	t = stack[-1]
	stack[-1] = stack[-3]
	stack[-3] = t
	t = stack[-2]
	stack[-2] = stack[-4]
	stack[-4] = t

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
	INVERT()
	ONEPLUS()

def QUIT():
	while True:
		REFILL()
		interpret()

def _DO():
	TO_R() # index
	TO_R() # limit

def I():
	stack.append(return_stack[-2])

def J():
	stack.append(return_stack[-4])

def DO():
	heap.append(words["(do)"])
	control_stack.append(len(heap))

def LOOP():
	heap.append(words["(loop)"])
	heap.append(control_stack[-1])
	control_stack.pop()

def _LOOP():
	global ip
	return_stack[-2] = ctypes.c_int(return_stack[-2] + 1).value
	if return_stack[-2] == return_stack[-1]:
		return_stack.pop()
		return_stack.pop()
		ip += 1
	else:
		ip = heap[ip]

# forth-standard.org
def WITHIN(): # ( test lower upper -- flag )
	OVER()
	MINUS()
	TO_R()
	MINUS()
	R_TO()
	U_LESS()

def PLUSLOOP():
	heap.append(words["(+loop)"])
	heap.append(control_stack[-1])
	control_stack.pop()

def _PLUSLOOP():
	global ip
	pre_increment = return_stack[-2]
	return_stack[-2] = ctypes.c_int(return_stack[-2] + stack.pop()).value
	post_increment = return_stack[-2]
	test_value = return_stack[-1] - 1
	stack.append(test_value)
	stack.append(post_increment)
	stack.append(pre_increment)
	WITHIN()
	if stack.pop():
		return_stack.pop()
		return_stack.pop()
		ip += 1
	else:
		ip = heap[ip]

def CR():
	print()

def CELLS():
	pass

def ALLOT():
	for i in range(stack[-1]):
		heap.append(None)
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
		heap.append(ord(c))

def SOURCE():
	stack.append(tib)
	stack.append(tib_count)

def FETCH():
	stack[-1] = heap[stack[-1]]

def TWOFETCH():
	a = stack[-1]
	stack[-1] = heap[a + 1]
	stack.append(heap[a])

def TO_R():
	return_stack.append(stack.pop())

def R_TO():
	stack.append(return_stack.pop())

def R_FETCH():
	stack.append(return_stack[-1])

def TYPE():
	l = heap[stack[-2] : stack[-2] + stack[-1]]
	for i in range(len(l)):
		if type(l[i]) == int:
			l[i] = chr(l[i])
	print("".join(l), end='', flush=True)
	TWODROP()

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

def ONEPLUS():
	l = ctypes.c_int(stack[-1])
	l.value += 1
	stack[-1] = l.value

def ONEMINUS():
	l = ctypes.c_int(stack[-1])
	l.value -= 1
	stack[-1] = l.value

def PLUS():
	l = ctypes.c_int(stack[-2])
	l.value += stack[-1]
	stack.pop()
	stack[-1] = l.value

def MINUS():
	l = ctypes.c_int(stack[-2])
	l.value -= stack[-1]
	stack.pop()
	stack[-1] = l.value

def ABS():
	l = ctypes.c_int(stack[-1])
	l.value = abs(l.value)
	stack[-1] = l.value

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

def U_LESS():
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

def S_TO_D():
	stack.append(-1 if stack[-1] < 0 else 0)

def MULTIPLY():
	v = ctypes.c_int(stack[-2])
	v.value *= stack[-1]
	stack.pop()
	stack[-1] = v.value

def M_MULTIPLY():
	s = stack[-2] * stack[-1]
	stack[-1] = ctypes.c_int(s >> 32).value
	stack[-2] = ctypes.c_int(s).value

def UM_MULTIPLY():
	s = ctypes.c_uint(stack[-2]).value * ctypes.c_uint(stack[-1]).value
	stack[-1] = ctypes.c_int(s >> 32).value
	stack[-2] = ctypes.c_int(s).value

def NIP(): # ( a b c -- a c )
	SWAP()
	DROP()

def TUCK(): # ( b a -- a b a )
	SWAP()
	OVER()

def UM_MOD(): # ( lsw msw divisor -- rem quot )
	lsw = stack[-3]
	msw = stack[-2]
	n = (ctypes.c_uint(msw).value << 32) | ctypes.c_uint(lsw).value
	d = ctypes.c_uint(stack[-1]).value
	stack[-3] = ctypes.c_uint(n % stack[-1]).value
	stack[-2] = ctypes.c_int(n // d).value
	stack.pop()

def M_PLUS(): # ( d1 u -- d2 )
	lsw = stack[-3]
	msw = stack[-2]
	n = (msw << 32) | ctypes.c_uint(lsw).value
	n += stack[-1]
	stack[-3] = ctypes.c_int(n).value
	stack[-2] = ctypes.c_int(n >> 32).value
	stack.pop()

# from FIG UK
def D_NEGATE():
	INVERT()
	TO_R()
	INVERT()
	R_TO()
	stack.append(1)
	M_PLUS()

# from Gforth
def FM_MOD(): # ( d n -- rem quot )
	# dup >r
	DUP()
	TO_R()
	# dup 0< if negate >r dnegate r> then
	DUP()
	ZLESS()
	if stack.pop():
		NEGATE()
		TO_R()
		D_NEGATE()
		R_TO()
	# over 0< if tuck + swap then
	OVER()
	ZLESS()
	if stack.pop():
		TUCK()
		PLUS()
		SWAP()
	# um/mod
	UM_MOD()
	# r> 0< if swap negate swap then
	R_TO()
	ZLESS()
	if stack.pop():
		SWAP()
		NEGATE()
		SWAP()

# from FIG UK : ?dnegate 0< if dnegate then ;
def Q_D_NEGATE():
	ZLESS()
	if stack.pop():
		D_NEGATE()

# from FIG UK
def D_ABS():
	DUP()
	Q_D_NEGATE()

# from FIG UK : ?negate 0< if negate then ;
def Q_NEGATE():
	ZLESS()
	if stack.pop():
		NEGATE()

# from FIG UK
def SM_REM():
	TWODUP()
	XOR()
	TO_R()
	OVER()
	TO_R()
	ABS()
	TO_R()
	D_ABS()
	R_TO()
	UM_MOD()
	SWAP()
	R_TO()
	Q_NEGATE()
	SWAP()
	R_TO()
	Q_NEGATE()

# from FIG UK
def SLASH_MOD():
	TO_R()
	S_TO_D()
	R_TO()
	FM_MOD()

def MOD():
	SLASH_MOD()
	DROP()

# from FIG UK
def SLASH():
	SLASH_MOD()
	NIP()

# from FIG UK
def STAR_SLASH_MOD():
	TO_R()
	M_MULTIPLY()
	R_TO()
	FM_MOD()

# from FIG UK
def STAR_SLASH():
	STAR_SLASH_MOD()
	NIP()

def L_BRACKET():
	heap[state_addr] = False

def R_BRACKET():
	heap[state_addr] = True

def POSTPONE():
	name = read_word().lower()
	if words[name].immediate:
		# Compiles the word instead of executing it immediately.
		heap.append(words[name])
	else:
		# Instead of compiling the word, compile code that compiles the word.
		heap.append(words["lit"])
		heap.append(words[name])
		heap.append(words[","])

def HERE():
	stack.append(len(heap))

def COMMA():
	heap.append(stack.pop())

def BEGIN():
	dest = len(heap)
	control_stack.append(dest)

def WHILE():
	heap.append(words["0branch"])
	orig = len(heap)
	control_stack.insert(-1, orig)
	heap.append(None)

def REPEAT():
	heap.append(words["branch"])
	dest = control_stack.pop()
	heap.append(dest)
	orig = control_stack.pop()
	heap[orig] = len(heap)

def UNTIL():
	heap.append(words["0branch"])
	heap.append(control_stack.pop())

def BL():
	stack.append(ord(' '))

def CHAR():
	w = read_word()
	stack.append(ord(w[0]))

def COMPILE_CHAR():
	CHAR()
	COMMA()

def TICK():
	w = read_word().lower()
	xt = words[w].xt
	stack.append(xt)

def COMPILE_TICK():
	TICK()
	heap.append(stack.pop())

def IMMEDIATE():
	words[latest].immediate = True

def FIND(): # ( c-addr -- c-addr 0 | xt 1 | xt -1 )
	wordname = ""
	addr = stack[-1]
	for i in range(heap[addr]):
		wordname += chr(heap[addr + i + 1]).lower()
	if wordname in words:
		word = words[wordname]
		stack[-1] = word.xt
		stack.append(1 if word.immediate else -1)
	else:
		stack.append(0)

def EXECUTE():
	stack.pop()()

def COUNT():
	DUP()
	ONEPLUS()
	SWAP()
	FETCH()

def LIT():
	global ip
	stack.append(heap[ip])
	ip += 1

def RECURSE():
	heap.append(words[latest])

add_word("\\", REFILL, True)
add_word("hex", HEX)
add_word("variable", VARIABLE)
add_word("!", STORE)
add_word("2!", TWOSTORE)
add_word("+!", PLUSSTORE)
add_word(":", COLON)
add_word(";", SEMICOLON, True)
add_word("depth", DEPTH)
add_word("?dup", QDUP)
add_word("dup", DUP)
add_word("2dup", TWODUP)
add_word("over", OVER)
add_word("2over", TWOOVER)
add_word("rot", ROT)
add_word("swap", SWAP)
add_word("2swap", TWOSWAP)
add_word("drop", DROP)
add_word("2drop", TWODROP)
add_word("0<", ZLESS)
add_word("0branch", ZBRANCH)
add_word("branch", BRANCH)
add_word("negate", NEGATE)
add_word("if", IF, True)
add_word("else", ELSE, True)
add_word("then", THEN, True)
add_word("cr", CR)
add_word("i", I)
add_word("j", J)
add_word("=", EQUALS)
add_word("0=", ZEQUAL)
add_word('s"', SQUOTE, True)
add_word("do", DO, True)
add_word("(do)", _DO)
add_word("loop", LOOP, True)
add_word("(loop)", _LOOP)
add_word("+loop", PLUSLOOP, True)
add_word("(+loop)", _PLUSLOOP)
add_word("exit", EXIT)
add_word("type", TYPE)
add_word("source", SOURCE)
add_word("@", FETCH)
add_word("2@", TWOFETCH)
add_word("1+", ONEPLUS)
add_word("1-", ONEMINUS)
add_word("+", PLUS)
add_word("-", MINUS)
add_word("abs", ABS)
add_word("cells", CELLS)
add_word("quit", QUIT)
add_word("create", CREATE)
add_word("allot", ALLOT)
add_word("sliteral", SLITERAL)
add_word("leave", lambda : sys.exit("leave"))
add_word(">r", TO_R)
add_word("r>", R_TO)
add_word("r@", R_FETCH)
add_word(">in", TO_IN)
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
add_word("u<", U_LESS)
add_word("min", MIN)
add_word("max", MAX)
add_word("s>d", S_TO_D)
add_word("*", MULTIPLY)
add_word("m*", M_MULTIPLY)
add_word("um*", UM_MULTIPLY)
add_word("fm/mod", FM_MOD)
add_word("um/mod", UM_MOD)
add_word("sm/rem", SM_REM)
add_word("[", L_BRACKET, True)
add_word("]", R_BRACKET)
add_word("/", SLASH)
add_word("*/", STAR_SLASH)
add_word("nip", NIP)
add_word("tuck", TUCK)
add_word("literal", COMMA, True)
add_word("postpone", POSTPONE, True)
add_word("*/mod", STAR_SLASH_MOD)
add_word("/mod", SLASH_MOD)
add_word("mod", MOD)
add_word("here", HERE)
add_word("chars", lambda *nop:nop)
add_word("align", lambda *nop:nop)
add_word("aligned", lambda *nop:nop)
add_word(",", COMMA)
add_word("c,", COMMA)
add_word("cell+", ONEPLUS)
add_word("char+", ONEPLUS)
add_word("c@", FETCH)
add_word("c!", STORE)
add_word("begin", BEGIN, True)
add_word("while", WHILE, True)
add_word("repeat", REPEAT, True)
add_word("until", UNTIL, True)
add_word("bl", BL)
add_word("char", CHAR)
add_word("[char]", COMPILE_CHAR, True)
add_word("'", TICK)
add_word("execute", EXECUTE)
add_word("[']", COMPILE_TICK, True)
add_word("immediate", IMMEDIATE)
add_word("find", FIND)
add_word("count", COUNT)
add_word("lit", LIT)
add_word("state", STATE)
add_word("recurse", RECURSE, True)
add_word("within", WITHIN)

QUIT()
