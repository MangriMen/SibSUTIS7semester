#pragma once
#include <iostream>
#include <vector>
#include <string>
#include <sstream>
#include "Utils.h"

const double PI = 3.141592653589793238463;

class Complex
{
private:
	long double _real;
	long double _img;

public:
	Complex(long double a = 0, long double b = 0) : _real(a), _img(b) {}

	Complex(const std::string f) {
		auto complexStr = Utils::split(f, ' ');

		_real = std::stold(complexStr[0]);
		_img = std::stold(Utils::split(complexStr[2], '*')[1]);
	}

	Complex(const Complex& rhs) {
		_real = rhs._real;
		_img = rhs._img;
	}

	Complex operator+(const Complex& rhs) {
		Complex complex(_real + rhs._real, _img + rhs._img);
		return complex;
	}

	Complex operator-() {
		Complex complex = Complex(0, 0) - (*this);
		return complex;
	}

	Complex operator-(const Complex& rhs) {
		Complex complex(_real - rhs._real, _img - rhs._img);
		return complex;
	}

	Complex operator*(const Complex& rhs) {
		Complex complex(_real * rhs._real - _img * rhs._img, _real * _img + rhs._real * _img);
		return complex;
	}

	Complex operator/(const Complex& rhs) {
		long double denominator = rhs._real * rhs._real + rhs._img * rhs._img;
		long double nominatorLeft = _real * rhs._real + _img * rhs._img;
		long double nominatorRight = rhs._real * _img + _real * rhs._img;

		Complex complex(nominatorLeft / denominator, nominatorRight / denominator);
		return complex;
	}

	Complex pow(const Complex& complex_, long long n = 2)
	{
		long double result1 = std::atan2(complex_._img, complex_._real);
		long double result2 = std::sqrtl(complex_._real * complex_._real + complex_._img * complex_._img);

		long double step = std::pow(result2, n);
		long double answer = n * result1;

		long double X = step * std::cos(answer);
		long double Y = step * std::sin(answer);

		Complex complex(X, Y);
		return complex;
	}

	long double abs() {
		return std::sqrtl(_real * _real + _img * _img);
	}

	long double angleRad() {
		if (_real > 0) {
			return std::atan(_img / _real);
		}
		else if (_real < 0) {
			return std::atan(_img / _real) + PI / 2;

		}
		else if (_real == 0 && _img > 0) {
			return PI / 2;

		}
		else if (_real == 0 && _img < 0) {
			return -PI / 2;
		}
	}

	long double angleDeg() {
		return angleRad() * 180 / PI;
	}

	Complex power(long long n) {
		return Complex(_real * _real, 0) * Complex(std::cos(n * angleRad()), std::sin(n * angleRad()));
	}

	Complex root(long long n, long long i) {
		std::vector<Complex> roots(n);
		long double phiSqrt = std::pow(abs(), 1 / n);
		for (size_t k = 0; k < roots.size(); k++)
		{
			long double coeff = 2 * PI * k;
			roots[k] = Complex(phiSqrt, 0) * Complex(std::cos((angleRad() + coeff) / n), std::sin((angleRad() + coeff) / n));
		}
		return roots[i];
	}

	long double real() {
		return _real;
	}

	long double img() {
		return _img;
	}

	std::string realString() const {
		return std::to_string(_real);
	}

	std::string imgString() const {
		return std::to_string(_img);
	}

	std::string toString() const {
		return realString() + "+i*" + imgString();
	}

	friend std::ostream& operator <<(std::ostream&, const Complex&);
};

