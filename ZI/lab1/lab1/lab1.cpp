#include "framework.h"
#include "pch.h"
#include "lab1.hpp"
#include <random>
#include <map>
#include <array>

namespace crypto {
	long long FastMath::modExp(long long a, long long x, long long p) {
		a %= p;
		long long result = 1;
		while (x > 0) {
			if (x & 1) result = (result * a) % p;
			a = (a * a) % p;
			x >>= 1;
		}
		return result;
	}

	bool DiffieHellman::isPrime(long long p) {
		if (p <= 1)
			return false;

		long long b = static_cast<long long>(pow(p, 0.5));

		for (long long i = 2; i <= b; ++i) {
			if ((p % i) == 0)
				return false;
		}

		return true;
	}

	long long DiffieHellman::generator(long long p) {
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
				ok &= FastMath::modExp(res, phi / fact[i], p) != 1;
			if (ok)
				return res;
		}
		return -1;
	}

	long long DiffieHellman::generateKey() {
		long long leftBound = 1;
		long long rightBound = static_cast<long long>(1e9);

		std::random_device dev;
		std::mt19937_64 rng(dev());
		std::uniform_int_distribution<std::mt19937_64::result_type> dist(leftBound,
			rightBound);

		long long q = 0;
		long long P = 0;
		while (!isPrime(q) || !isPrime(P)) {
			q = dist(rng);
			P = 2 * q + 1;
		}
		long long g = generator(P);

		std::uniform_int_distribution<std::mt19937_64::result_type> distPrivateKey(
			1, P - 1);

		long long Xa = distPrivateKey(rng);
		long long Xb = distPrivateKey(rng);

		long long Ya = FastMath::modExp(g, Xa, P);
		long long Yb = FastMath::modExp(g, Xb, P);

		long long Zab = FastMath::modExp(Yb, Xa, P);
		long long Zba = FastMath::modExp(Ya, Xb, P);

		if (Zab == Zba) {
			return Zab;
		}

		throw std::runtime_error("Keys are not equal");
	}

	std::array<long long, 3> Euclid::getExtendedGCD(long long a, long long n) {
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

	std::vector<long long> BabyGiantStep::calculate(long long a, long long p, long long y) {
		long long m = 0;
		long long k = 0;
		do {
			m = static_cast<long long>(sqrt(p) + 1);
			k = static_cast<long long>(sqrt(p) + 1);
		} while (m * k <= p);

		std::vector<long long> results;
		std::map<long long, std::vector<long long>> rowY;

		for (long long j = 0; j < m; j++) {
			if (!rowY.count(static_cast<long long>(pow(a, j) * y) % p)) {
				rowY[static_cast<long long>(pow(a, j) * y) % p].push_back(j);
			}
		}

		for (long long i = 1; i <= k; i++) {
			if (rowY.count(static_cast<long long>(pow(a, i * m)) % p)) {
				for (const auto& x : rowY.at(static_cast<long long>(pow(a, i* m)) % p)) {
					results.push_back(i * m - x);
				}
			}
		}

		return results;
	}
} // namespace crypto