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
}