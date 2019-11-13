gcc -c -fPIC util.c -o util.o
gcc -shared -Wl,-soname,libutil.so -o libutil.so  util.o
