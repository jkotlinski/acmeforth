# C64 Cross Compiler

def export(words_, heap_, wordname):
	global words
	global heap
	words = words_
	heap = heap_

	export_word(words[wordname])

def export_word(w):
	print(w.xt)
	for ip in range(w.body, w.body_end):
		print(heap[ip])
