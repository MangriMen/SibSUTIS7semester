#include "pch.h"
#include "framework.h"
#include "lab2.h"

long long modExp(long long x, long long y, long long N)
{
	if (y == 0) {
		return 1;
	}

	long long z = modExp(x, y / 2, N);

	if (y % 2 == 0) {
		return (z * z) % N;
	}
	return (x * z * z) % N;
}

bool miillerTest(long long d, long long n)
{
	// Pick a random number in [2..n-2]
	// Corner cases make sure that n > 4
	long long a = 2 + rand() % (n - 4);

	// Compute a^d % n
	long long x = modExp(a, d, n);

	if (x == 1 || x == n - 1)
		return true;

	// Keep squaring x while one of the following doesn't
	// happen
	// (i)   d does not reach n-1
	// (ii)  (x^2) % n is not 1
	// (iii) (x^2) % n is not n-1
	while (d != n - 1)
	{
		x = (x * x) % n;
		d *= 2;

		if (x == 1)      return false;
		if (x == n - 1)    return true;
	}

	// Return composite
	return false;
}

bool isPrime(long long n, long long k)
{
	// Corner cases
	if (n <= 1 || n == 4)  return false;
	if (n <= 3) return true;

	// Find r such that n = 2^d * r + 1 for some r >= 1
	long long d = n - 1;
	while (d % 2 == 0)
		d /= 2;

	// Iterate given number of 'k' times
	for (long long i = 0; i < k; i++)
		if (!miillerTest(d, n))
			return false;

	return true;
}

std::array<long, 3> getExtendedGCD(int a, int n) {
	long tempN = n;

	long x1 = 1;
	long x2 = 0;

	long y1 = 0;
	long y2 = 1;

	while (n != 0) {
		long qoutient = a / n;
		long remainder = a % n;

		a = n;
		n = remainder;

		long tempX = x1 - x2 * qoutient;
		x1 = x2;
		x2 = tempX;

		long tempY = y1 - y2 * qoutient;
		y1 = y2;
		y2 = tempY;
	}

	return std::array<long, 3>{a, x1 < 0 ? x1 + tempN : x1, y1};
}

class Shamir {
private:
	long long P;
	long long C;
	long long D;

public:
	static long long generatePublicP() {
		std::random_device dev;
		std::mt19937_64 rng(dev());
		std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, INT_MAX - 4);

		long P;
		do {
			P = dist(rng) + 2;
		} while (!isPrime(P, 4)); // || !DiffieHellman.testFerma(P, 100)
		return P;
	}

	Shamir(long P) {
		this->P = P;

		std::random_device dev;
		std::mt19937_64 rng(dev());
		std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, INT_MAX - 4);
		std::array<long, 3> EuclidResult;
		do {
			do {
				this->C = dist(rng) + 2;
				EuclidResult = getExtendedGCD(C, P - 1);
			} while (EuclidResult[0] != 1);
			this->D = EuclidResult[2] + (P - 1);
		} while (C * D % (P - 1) != 1);
	}
};