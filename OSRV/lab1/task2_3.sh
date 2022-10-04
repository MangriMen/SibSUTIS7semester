gcc $1.c -o $1.out

if [ $? -ne 0 ]; then
	echo -n "Press any key to enter the editor..."
	read
	vi $1.c
	exit 1;
fi

./$1.out
