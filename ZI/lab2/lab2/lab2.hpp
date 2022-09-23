#pragma once
#include <iostream>
#include <random>
#include <array>
#include <numeric>
#include "../../lab1/lab1/lab1.hpp"


namespace encryption {
	class Shamir {
	private:
		long long P;
		long long C;
		long long D;

	public:
		Shamir(long long P);

		static long long generatePublicP();

		std::vector<long long> ShamirCalcIteration(const std::string& path, std::vector<long long> prevX, int numOfIteration);

		static void encryptFile(const std::string&);

		long long getC();
		long long getD();
	};
}