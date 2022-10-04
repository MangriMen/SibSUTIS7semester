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

		std::variant<std::vector<unsigned long long>, std::vector<char>> ShamirCalcIteration(const std::vector<char>& message, std::vector<unsigned long long> prevX, int numOfIteration);

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

		ElGamal();

		std::vector<unsigned long long> encryptMessage(const std::vector<char>& message, unsigned long long D);
		std::vector<char> decryptMessage(std::vector<unsigned long long> message, unsigned long long R);

		std::vector<unsigned long long> encryptFile(std::string filename, unsigned long long D);
		std::vector<char> decryptFile(std::string filename, unsigned long long R);

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

		std::vector<char> encryptFile(std::string file);
		std::vector<char> decryptFile(std::string file, const std::vector<char>& key);

		std::vector<char> getSecretKey();
		void setSecretKey(const std::vector<char>& key);
	};

	class RSA {
	private:
		unsigned long long e;
		unsigned long long d;
		unsigned long long n;

	public:
		void RSA_Initialize()
		{
			std::mt19937_64 rng((std::random_device())());
			std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, static_cast<unsigned long long>(1e9));

			unsigned long long p;
			unsigned long long q;
			do {
				p = dist(rng);
				q = dist(rng);
			} while (p == q || !crypto::DiffieHellman::isPrime(p) || !crypto::DiffieHellman::isPrime(q));

			n = p * q;

			unsigned long long eulerN = (p - 1) * (q - 1);

			std::uniform_int_distribution<std::mt19937_64::result_type> distE(7, eulerN);

			do {
				e = distE(rng);
			} while (crypto::Euclid::getExtendedGCD(e, eulerN)[0] != 1 || !crypto::DiffieHellman::isPrime(e));

			d = crypto::Euclid::getExtendedGCD(e, eulerN)[1];
		};

		std::vector<unsigned long long> RSA_Encrypt(std::string file)
		{
			auto data = Utils::readFileAsBytes(file);
			std::vector<unsigned long long> message(data.size());
			for (size_t i = 0; i < message.size(); i++)
			{
				short a = (unsigned char)(data[i]);
				message[i] = crypto::FastMath::modExp(a, e, n);
			}
			return message;
		}

		std::vector<char> RSA_Decrypt(std::string file)
		{
			auto data = Utils::readFileAsBytes<unsigned long long>(file);
			std::vector<char> message(data.size());
			for (size_t i = 0; i < message.size(); i++)
			{
				message[i] = (char)crypto::FastMath::modExp(data[i], d, n);
			}
			return message;
		}
	};
}