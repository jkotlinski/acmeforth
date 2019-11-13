#include <stdio.h>

/* It would be great if this could be rewritten in Python. */

int lshift(int i, int n) {
	return (int)((unsigned int)i << n);
}

int rshift(int i, int n) {
	return (int)((unsigned int)i >> n);
}
