#pragma once
#include <iostream>
#include <vector>

namespace crypto {
	class FastMath {
	public:
		static unsigned long long modExp(unsigned long long a, unsigned long long x, unsigned long long p);
	};

	class DiffieHellman {
	public:
		static bool isPrime(long long p);
		static long long generator(long long p);
		static std::tuple<long long, long long, long long> generateKey();
	};

	class Euclid {
	public:
		static std::array<long long, 3> getExtendedGCD(long long a, long long n);
	};

	class BabyGiantStep {
	public:
		static std::vector<unsigned long long> calculate(unsigned long long a, unsigned long long p, unsigned long long y);
	};
} // namespace crypto