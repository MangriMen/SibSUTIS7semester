#pragma once
#include <iostream>
#include <vector>

namespace crypto {
	class FastMath {
	public:
		static long long modExp(long long a, long long x, long long p);
	};

	class DiffieHellman {
	public:
		static bool isPrime(long long p);
		static long long generator(long long p);
		static long long generateKey();
	};

	class Euclid {
	public:
		static std::array<long long, 3> getExtendedGCD(long long a, long long n);
	};

	class BabyGiantStep {
	public:
		static std::vector<long long> calculate(long long a, long long p,
			long long y);
	};
} // namespace crypto