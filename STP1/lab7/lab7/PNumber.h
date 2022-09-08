#pragma once
#include <iostream>
#include <string>

class PNumber
{
	long double a = 0;
	long double b = 0;
	long double c = 0;

public:
	PNumber(long double a_ = 0, long double b_ = 0, long double c_ = 0) {
		if (b < 2 || b > 16) {
			throw std::invalid_argument("Base must be in range [2..16]");
		}

		a = a;
		b = b;
		c = c;
	}

	PNumber(const std::string& a_, const std::string& b_, const std::string& c_) {
		long double aParsed = std::stold(a_);
		long double bParsed = std::stold(b_);
		long double cParsed = std::stold(c_);

		if (bParsed < 2 || bParsed > 16) {
			throw std::invalid_argument("Base must be in range [2..16]");
		}

		a = aParsed;
		b = bParsed;
		c = cParsed;
	}

	PNumber(const PNumber& pnumber) {
		a = pnumber.a;
		b = pnumber.b;
		c = pnumber.c;
	}

	PNumber operator+(const PNumber& pnumber) {

	}
};

