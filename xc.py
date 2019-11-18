# C64 Cross Compiler

def export(xt_words_, heap_, start_word):
	global xt_words
	global heap
	xt_words = xt_words_
	heap = heap_

	words_to_export.append(start_word)

	while words_to_export:
		word = words_to_export.pop()
		export_word(word)

exported_words = []
words_to_export = []

def export_word(w):
	if w in exported_words:
		return
	exported_words.append(w)

	xt = w.xt

	if w.body:
		export_forth_word(w)
	else:
		export_python_word(w)

def export_forth_word(w):
	print("export_forth_word", w)
	for ip in range(w.body, w.body_end):
		cell = heap[ip]
		if callable(cell):
			cell_word = xt_words[cell]
			words_to_export.append(cell_word)
			print(xt_words[cell])
		elif type(cell) == type(0):
			print(cell)
		else:
			sys.exit("Unknown cell type", cell)

def export_python_word(w):
	print("export_python_word", w)
	pass
	
