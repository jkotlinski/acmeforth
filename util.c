#include <stdio.h>

int lshift(int i, int n) {
	return (int)((unsigned int)i << n);
}

int rshift(int i, int n) {
	return (int)((unsigned int)i >> n);
}
