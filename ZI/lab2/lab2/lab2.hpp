#pragma once
#include <iostream>
#include <random>
#include <array>
#include <numeric>
#include <list>
#include <fstream>
#include "../../lab1/lab1/lab1.hpp"
#include "Utils.h"
#include <cassert>
#include <variant>

namespace encryption {
	class Shamir {
	private:
		unsigned long long P;
		unsigned long long C;
		unsigned long long D;

	public:
		static unsigned long long generatePublicP();

		Shamir(unsigned long long P);

		std::variant<std::vector<unsigned long long>, std::vector<char>> ShamirCalcIteration(const std::vector<char>& message, const std::vector<unsigned long long>& prevX, int numOfIteration);

		unsigned long long getC();
		unsigned long long getD();

		void setCD(const std::vector<unsigned long long>& keys);
	};

	class ElGamal {
	private:
		static unsigned long long P;
		static unsigned long long G;

		unsigned long long C;
		unsigned long long D;

		unsigned long long R;

	public:
		static void generateParameters();

		static void setP(unsigned long long p);
		static void setG(unsigned long long g);

		static unsigned long long getP();
		static unsigned long long getG();

		ElGamal();

		std::vector<unsigned long long> encryptMessage(const std::vector<char>& message, unsigned long long D);
		std::vector<char> decryptMessage(std::vector<unsigned long long> message, unsigned long long R);

		std::vector<unsigned long long> encryptFile(const std::string& filename, unsigned long long D);
		std::vector<char> decryptFile(const std::string& filename, unsigned long long R);

		unsigned long long getC();
		unsigned long long getD();
		unsigned long long getR();

		void setCD(const std::vector<unsigned long long>& keys);
	};

	class Vernam {
	private:
		std::vector<char> secretKey;

	public:
		std::vector<char> encryptMessage(const std::vector<char>& message);
		std::vector<char> decryptMessage(const std::vector<char>& message, const std::vector<char>& key);

		std::vector<char> encryptFile(const std::string& file);
		std::vector<char> decryptFile(const std::string& file, const std::vector<char>& key);

		std::vector<char> getSecretKey();
		void setSecretKey(const std::vector<char>& key);
	};

	class RSA {
	private:
		unsigned long long e;
		unsigned long long d;
		unsigned long long n;

	public:
		RSA();

		std::vector<unsigned long long> encryptMessage(const std::vector<char>& message);
		std::vector<char> decryptMessage(const std::vector<unsigned long long>& message);

		std::vector<unsigned long long> encryptFile(const std::string& file);
		std::vector<char> decryptFile(const std::string& file);

		std::vector<unsigned long long> getPublicKey();
		void setPublicKey(const std::vector<unsigned long long>& key);

		unsigned long long getPrivateKey();
		void setPrivateKey(unsigned long long key);
	};
}