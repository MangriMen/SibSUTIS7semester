#include "pch.h"
#include "framework.h"
#include "lab1.h"

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

long long generator(long long p) {
	std::vector<long long> fact;
	long long phi = p - 1, n = phi;
	for (long long i = 2; i * i <= n; ++i)
		if (n % i == 0) {
			fact.push_back(i);
			while (n % i == 0)
				n /= i;
		}
	if (n > 1)
		fact.push_back(n);

	for (long long res = 2; res <= p; ++res) {
		bool ok = true;
		for (size_t i = 0; i < fact.size() && ok; ++i)
			ok &= modExp(res, phi / fact[i], p) != 1;
		if (ok)  return res;
	}
	return -1;
}


long long generateDiffieHellmanKey() {
	long long leftBound = 300;
	long long rightBound = 6500;
	int accuracy = 4;

	std::random_device dev;
	std::mt19937_64 rng(dev());
	std::uniform_int_distribution<std::mt19937_64::result_type> dist(leftBound, rightBound);

	long long q = 0;
	long long P = 0;
	while (!isPrime(q, accuracy) || !isPrime(P, accuracy)) {
		q = dist(rng);
		P = 2 * q + 1;
	}
	long long g = generator(P);

	std::uniform_int_distribution<std::mt19937_64::result_type> distPrivateKey(1, P);

	long long Xa = distPrivateKey(rng);
	long long Xb = distPrivateKey(rng);

	long long Ya = modExp(g, Xa, P);
	long long Yb = modExp(g, Xb, P);

	long long Zab = modExp(Yb, Xa, P);
	long long Zba = modExp(Ya, Xb, P);

	if (Zab == Zba) {
		return Zab;
	}

	throw std::runtime_error("Keys are not equal");
}

long long babyGiantStep(long long a, long long p, long long y) {
	std::random_device dev;
	std::mt19937 rng(dev());
	std::uniform_int_distribution<std::mt19937::result_type> dist(0, 1);

	int ji[2] = { -1, -1 };

	long long m = 0;
	long long k = 0;

	do {
		m = static_cast<long long>(sqrt(p) + 1);
		k = static_cast<long long>(sqrt(p) + 1);
	} while (m * k <= p);

	std::map<long long, int> rowY;

	for (int j = 0; j < m; j++) {
		if (!rowY.count(static_cast<long long>(pow(a, j) * y) % p)) {
			rowY[static_cast<long long>(pow(a, j) * y) % p] = j;
		}
	}

	for (int i = 1; i <= k; i++) {
		if (rowY.count(static_cast<long long>(pow(a, i * m)) % p)) {
			ji[0] = rowY.at(static_cast<long long>(pow(a, i * m)) % p);
			ji[1] = i;
			break;
		}
	}

	if (ji[0] == -1 && (ji[1] == -1)) {
		return -1;
	}

	return ji[1] * m - ji[0];
}