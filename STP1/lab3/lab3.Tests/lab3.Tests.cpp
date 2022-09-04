#include "pch.h"
#include "CppUnitTest.h"
#include "../lab3/Class1.h"
#include <vector>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace lab3Tests
{
	TEST_CLASS(lab3Tests)
	{
	public:

		TEST_METHOD(TestShiftInt)
		{
			Assert::AreEqual(561234, Class1::shiftInt(123456, 2, Class1::ShiftDirection::right));
		}

		TEST_METHOD(TestFibNumber)
		{
			Assert::AreEqual(3, Class1::fibNumber(4));
		}

		TEST_METHOD(TestDelDecimalInt)
		{
			Assert::AreEqual(1256, Class1::delDecimalInt(123456, 2, 2));
		}

		TEST_METHOD(TestgetEvenSumTopAndSecondaryDiagonalMatrix)
		{
			std::vector<std::vector<int>> matrix{ {1, 2, 3, 32, 3}, { 1,2,3,32,3 }, { 1,2,3,32,3 }, { 1,2,3,32,3 }, { 1,2,3,32,3 } };
			Assert::AreEqual(5, Class1::getEvenSumTopAndSecondaryDiagonalMatrix(matrix));
		}
	};
}
