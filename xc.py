# C64 Cross Compiler

def export(xt_words_, heap_, start_word):
	global xt_words
	global heap
	xt_words = xt_words_
	heap = heap_

	export_word(start_word)

def export_word(w):
	print(w.xt)
	for ip in range(w.body, w.body_end):
		cell = heap[ip]
		if callable(cell):
			print(xt_words[cell])
		elif type(cell) == type(0):
			print(cell)
		else:
			sys.exit("Unknown cell type", cell)
