#pragma once
#include <iostream>
#include <vector>
#include <algorithm>
#include "SimplePoly.h"

class Poly
{
private:
	std::vector<SimplePoly> pol;
public:
	Poly(long long coeff = 0, long long degree = 0) {
		pol.push_back(SimplePoly(coeff, degree));
	}

	long long degree() {
		return (*std::max_element(pol.begin(), pol.end(),
			[](SimplePoly& a, SimplePoly& b)
			{
				return a.getDegree() > b.getDegree();
			}
		)).getDegree();
	}

	long long coeff(long long degree) {
		auto el = std::find(pol.begin(), pol.end(),
			[](SimplePoly& a, SimplePoly& b)
			{
				return a.getDegree() > b.getDegree();
			}
		);

		return el != pol.end() ? (*el).getCoeff() : 0;
	}

	void clear() {
		pol.clear();
	}

	//Poly operator+(const Poly& rhs) {
	//	size_t length = std::min(pol.size(), rhs.pol.size());

	//	bool isLHSPol = false;
	//	if (pol.size() > rhs.pol.size()) {
	//		isLHSPol = true;
	//	}
	//	else {
	//		isLHSPol = false;
	//	}

	//	auto newPoly = Poly();
	//	newPoly.pol = isLHSPol ? pol : rhs.pol;
	//	for (size_t i = 0; i < length; i++) {
	//		if (isLHSPol) {
	//			newPoly.pol[i] += pol[i];
	//		}
	//		else {
	//			newPoly.pol[i] += rhs.pol[i];
	//		}
	//	}
	//}

	bool operator==(const Poly& rhs) {
		return pol == rhs.pol;
	}
};

