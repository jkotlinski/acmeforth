class Dictionary:
	def __init__(self):
		self.words = {}
		self.xt_words = {}
		self.code_words = {}
		self.latest = None

	def copy(self):
		d = Dictionary()
		d.words = self.words.copy()
		d.xt_words = self.xt_words.copy()
		d.code_words = self.code_words.copy()
		d.latest = self.latest
		return d
