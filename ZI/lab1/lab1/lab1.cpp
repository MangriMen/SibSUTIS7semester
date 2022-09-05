#include "pch.h"
#include "framework.h"

int modexp(int x, int y, int N)
{
    if (y == 0) {
        return 1;
    }

    int z = modexp(x, y / 2, N);
    
    if (y % 2 == 0) {
        return (z * z) % N;
    }
    return (x * z * z) % N;
}

int gcd(int a, int b, int& x, int& y) {
	if (a == 0) {
		x = 0;
		y = 1;
		return b;
	}

	int x1, y1;
	int d = gcd(b % a, a, x1, y1);
	x = y1 - (b / a) * x1;
	y = x1;

	return d;
}