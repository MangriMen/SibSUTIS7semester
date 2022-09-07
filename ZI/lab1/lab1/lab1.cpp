#include "pch.h"
#include "framework.h"
#include "lab1.h"

long long modExp(long long a, long long x, long long p) {
	long long y = 1;
	long long s = a;

	std::vector<long long> xVec;
	long long tempX = x;
	while (tempX) {
		xVec.push_back(tempX /= 2);
	}

	for (long long i = xVec.size() - 1; i >= 0; i--)
	{
		if (xVec[i] == 1) {
			y = (y * s) % p;
		}
		s = (s * s) % p;
	}

	return y;
}

std::array<long long, 3> getExtendedGCD(long long a, long long n) {
	long long tempN = n;

	long long x1 = 1;
	long long x2 = 0;

	long long y1 = 0;
	long long y2 = 1;

	while (n != 0) {
		long long qoutient = a / n;
		long long remainder = a % n;

		a = n;
		n = remainder;

		long long tempX = x1 - x2 * qoutient;
		x1 = x2;
		x2 = tempX;

		long long tempY = y1 - y2 * qoutient;
		y1 = y2;
		y2 = tempY;
	}

	return std::array<long long, 3>{a, x1 < 0 ? x1 + tempN : x1, y1};
}

bool isPrime(long long p)
{
	if (p <= 1) return false;

	long long b = static_cast<long long>(pow(p, 0.5));

	for (long long i = 2; i <= b; ++i)
	{
		if ((p % i) == 0) return false;
	}

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
	long long leftBound = 1;
	long long rightBound = static_cast<long long>(1e9);

	std::random_device dev;
	std::mt19937_64 rng(dev());
	std::uniform_int_distribution<std::mt19937_64::result_type> dist(leftBound, rightBound);

	long long q = 0;
	long long P = 0;
	while (!isPrime(q) || !isPrime(P)) {
		q = dist(rng);
		P = 2 * q + 1;
	}
	long long g = generator(P);

	std::uniform_int_distribution<std::mt19937_64::result_type> distPrivateKey(1, P-1);

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