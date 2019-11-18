# C64 Cross Compiler

def export(_words, wordname):
	global words
	words = _words
	export_word(words[wordname])

def export_word(w):
	print(w.xt)
	print(w.body)
