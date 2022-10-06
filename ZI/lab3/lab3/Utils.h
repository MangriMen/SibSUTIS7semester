#pragma once
#include <iostream>
#include <vector>
#include <memory>
#include "SHA256.h"
#include "../../lab2/lab2/Utils.h"

namespace sign {
	class SignUtils : public Utils {
	public:
		static std::vector<char> getHashFromMessage(const std::vector<char>& message) {
			std::string messageString(message.begin(), message.end());

			SHA256 sha;
			sha.update(messageString);
			uint8_t* digest = sha.digest();

			auto hash = sha.toString(digest);
			std::vector<char> messageHash(hash.begin(), hash.end());

			delete digest;

			return messageHash;
		}
	};
}