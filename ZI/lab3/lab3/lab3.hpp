#pragma once
#include <iostream>
#include <vector>
#include "../../lab2/lab2/lab2.hpp"
#include "Utils.h"

namespace sign {
	class RSA {
	public:
		static std::vector<unsigned long long> signMessage(const std::vector<unsigned long long>& publicKey, const std::vector<char>& message) {
			auto messageHash = SignUtils::getHashFromMessage(message);

			encryption::RSA rsa;
			rsa.setPublicKey(publicKey);

			auto encryptedHash = rsa.encryptMessage(messageHash);
			return encryptedHash;
		}

		static bool validateSign(const std::vector<uint64_t>& publicKey, uint64_t privateKey, const std::vector<char>& message, const std::vector<uint64_t>& sign) {
			encryption::RSA rsa;
			rsa.setPublicKey(publicKey);
			rsa.setPrivateKey(privateKey);

			auto decryptedHash = rsa.decryptMessage(sign);
			auto messageHash = SignUtils::getHashFromMessage(message);

			return decryptedHash == messageHash;
		}
	};

	class ElGamal {
	public:
		static std::vector<unsigned long long> signMessage(encryption::ElGamal& A, encryption::ElGamal& B, const std::vector<char>& message) {
			auto messageHash = SignUtils::getHashFromMessage(message);

			auto encryptedHash = A.encryptMessage(messageHash, B.getD());
			return encryptedHash;
		}

		static bool validateSign(encryption::ElGamal& A, encryption::ElGamal& B, const std::vector<char>& message, const std::vector<uint64_t>& sign) {
			encryption::ElGamal A_ = A;
			encryption::ElGamal B_ = B;

			auto decryptedHash = B_.decryptMessage(sign, A_.getR());
			auto messageHash = SignUtils::getHashFromMessage(message);

			return decryptedHash == messageHash;
		}
	};

	class GOST_P34_10_94 {
	private:
		uint64_t _p;
		uint64_t _q;
		uint64_t _a;

		uint64_t publicKey;
		uint64_t privateKey;
	public:
		std::tuple<std::vector<unsigned long long>, uint64_t> signMessage(const std::vector<char>& message) {
			auto messageHash = SignUtils::getHashFromMessage(message);

			std::mt19937_64 rng((std::random_device())());
			std::uniform_int_distribution<std::mt19937_64::result_type> dist(1, _q - 1);

			uint64_t k;
			uint64_t r;
			bool hasNullValue = false;
			std::vector<unsigned long long> encryptedHash(message.size());
			do {
				k = dist(rng);
				r = crypto::FastMath::modExp(
					crypto::FastMath::modExp(_a, k, _p),
					1,
					_p
				);
				hasNullValue = false;
				for (size_t i = 0; i < message.size(); i++)
				{
					auto letter = static_cast<unsigned char>(message[i]);
					encryptedHash[i] = (k * letter + privateKey * r);
					if (encryptedHash[i] == 0) {
						hasNullValue = true;
					}
				}
			} while (r == 0 || hasNullValue == 0);

			return { encryptedHash, r };
		}

		bool validateSign(uint64_t r, const std::vector<char>& message, const std::vector<uint64_t>& sign) {
			if (r <= 0 || r >= _q) {
				return false;
			}



			auto messageHash = SignUtils::getHashFromMessage(message);
		}

		static uint64_t getMSB(uint64_t x) {
			uint64_t k = 63;
			uint64_t t = 1ull << 63ull;
			while (x < t) {
				k--;
				t >>= 1;
			}
			return k;
		}

		static std::tuple<uint64_t, uint64_t, uint64_t> generateParameters() {
			std::mt19937_64 rng((std::random_device())());
			std::uniform_int_distribution<std::mt19937_64::result_type> dist(1, static_cast<uint64_t>(1e4));

			uint64_t p;
			uint64_t q;
			do {
				p = dist(rng);
				q = dist(rng);
			} while ((2 * q + 1) != p);

			uint64_t a;
			do {
				a = dist(rng);
			} while (crypto::FastMath::modExp(a, q, p) != 1);

			return { p, q, a };
		}

		GOST_P34_10_94(uint64_t p, uint64_t q, uint64_t a) : _p(p), _q(q), _a(a)
		{
			std::mt19937_64 rng((std::random_device())());
			std::uniform_int_distribution<std::mt19937_64::result_type> dist(1, q - 1);

			privateKey = dist(rng);
			publicKey = crypto::FastMath::modExp(a, privateKey, p);
		};

		uint64_t getPublicKey() {
			return publicKey;
		}

		uint64_t getPrivateKey() {
			return privateKey;
		}
	};
}