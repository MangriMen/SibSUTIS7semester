#pragma once
#include <iostream>
#include <vector>

class Class1 {
public:
	enum class ShiftDirection {
		left,
		right
	};

	static int getNumberRankCount(int number) {
		int rankCount = 0;
		while (number) {
			number /= 10;
			rankCount++;
		}
		return rankCount;
	}

	static int shiftLeft(int number, int shiftCount, int base = 10) {
		int rankCount = getNumberRankCount(number);
		int position = static_cast<unsigned int>(std::pow(base, rankCount - 1));

		for (int i = 0; i < shiftCount; i++) {
			int tmp = number / position;
			number %= position;
			number = number * base + tmp;
		}
		return number;
	}

	static int shiftRight(int number, int shiftCount, int base = 10) {
		int rankCount = getNumberRankCount(number);
		int position = static_cast<unsigned int>(std::pow(base, rankCount - 1));

		for (int i = 0; i < shiftCount; i++) {
			int tmp = number % base;
			number /= base;
			number = tmp * position + number;
		}
		return number;
	}

	// Tasks

	static int shiftInt(int number, int shiftCount, ShiftDirection direction) {
		switch (direction)
		{
		case ShiftDirection::left:
			return shiftLeft(number, shiftCount);
			break;
		case ShiftDirection::right:
			return shiftRight(number, shiftCount);
			break;
		default:
			throw new std::invalid_argument("Unknown direction");
			break;
		}
	}

	static int fibNumber(int n) {
		int a = 0;
		int b = 1;
		for (int i = 0; i < n; i++) {
			a = a + b;
			b = a - b;
		}
		return a;
	}

	static int delDecimalInt(int number, int position, int count) {
		int rankCount = getNumberRankCount(number);

		int zeroCount = static_cast<unsigned int>(std::pow(10, rankCount - position - count));

		int lastPart = number % zeroCount;
		int firstPart = number / static_cast<unsigned int>(std::pow(10, rankCount - position));

		return firstPart * zeroCount + lastPart;
	}

	static int getEvenSumTopAndSecondaryDiagonalMatrix(std::vector<std::vector<int>> matrix) {
		int sum = 0;
		for (int i = 0; i < matrix.size(); i++) {
			for (int j = 0; j < matrix[i].size() - i; j++) {
				if (i + j >= matrix.size() - 1) {
					continue;
				}
				if (!(i % 2) && !(j % 2)) {
					sum += matrix[i][j];
				}
			}
		}
		return sum;
	}
};

