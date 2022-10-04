#include "framework.h"
#include "pch.h"
#include "lab1.hpp"
#include <random>
#include <map>
#include <array>

namespace crypto {
	unsigned long long FastMath::modExp(unsigned long long a, unsigned long long x, unsigned long long p) {
		if (x == 0)
			return 1;
		if (x % 2 == 1)
			return a * modExp(a, x - 1, p) % p;
		else
			return modExp(a * a % p, x / 2, p) % p;
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

	std::tuple<long long, long long, long long> DiffieHellman::generateKey() {
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
			return std::make_tuple(P, g, Zab);
		}

		throw std::runtime_error("Keys are not equal");
	}

	std::array<long long, 3> Euclid::getExtendedGCD(long long a, long long b) {
		if (a < b) {
			std::swap(a, b);
		}

		std::array<long long, 3>U = { a, 1, 0 };
		std::array<long long, 3>V = { b, 0, 1 };

		while (V[0] != 0) {
			long long q = U[0] / V[0];
			std::array<long long, 3> T{ U[0] % V[0], U[1] - q * V[1], U[2] - q * V[2] };
			U = V;
			V = T;
		}

		return U;
	}

	std::vector<unsigned long long> BabyGiantStep::calculate(unsigned long long a, unsigned long long y, unsigned long long p) {
		auto n = static_cast<unsigned long long>(ceil(sqrt(p - 1)));

		std::vector<unsigned long long> results;

		std::map<unsigned long long, std::vector<unsigned long long>> row;
		for (unsigned long long i = 0; i < n; i++) {
			row[FastMath::modExp(a, i, p)].push_back(i);
		}

		unsigned long long c = FastMath::modExp(a, n * (p - 2), p);

		for (unsigned long long i = 0; i < n; i++) {
			unsigned long long temp = (y * FastMath::modExp(c, i, p)) % p;
			if (row.count(temp))
			{
				for (const auto& j : row.at(temp)) {
					results.push_back(i * n + j);
				}
			}
		}

		return results;
	}
} // namespace crypto