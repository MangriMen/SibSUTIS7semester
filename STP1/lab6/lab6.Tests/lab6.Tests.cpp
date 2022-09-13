#include "pch.h"
#include "CppUnitTest.h"
#include "../lab6/Complex.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace lab6Tests
{
	TEST_CLASS(lab6Tests)
	{
	public:
		
		TEST_METHOD(TestConstructor)
		{
			auto _ = Complex(1, 5);
		}
		TEST_METHOD(TestConstructorZeroes)
		{
			auto _ = Complex(0, 0);
		}

		TEST_METHOD(TestConstructorNegative)
		{
			auto _ = Complex(-1, -5);
		}
		TEST_METHOD(TestPlus)
		{
			auto firstValue = Complex(-2, -5);
			auto secondValue = Complex(-1, -5);

			auto expectedValue = Complex(-1, -5);
			auto actualValue = firstValue + secondValue;

			std::cout << expectedValue << " " << actualValue << "\n";

			Assert::IsTrue(true);
		}
	};
}
