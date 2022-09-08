using lab4;

namespace lab4.Tests
{
    [TestClass]
    public class MatrixTest
    {
        [TestMethod]
        [ExpectedException(typeof(MyException))]
        public void TestConstructorZeroSize()
        {
            _ = new Matrix(0, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(MyException))]
        public void TestConstructorNegativeSize()
        {
            _ = new Matrix(2, -1);
        }

        [TestMethod]
        public void TestConstructorDefault()
        {
            _ = new Matrix(2, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(MyException))]
        public void TestIndexOperatorGetBigI()
        {
            Matrix testMatrix = new(2, 2);
            int wrongValue = testMatrix[3, 1];
        }

        [TestMethod]
        [ExpectedException(typeof(MyException))]
        public void TestIndexOperatorSetBigI()
        {
            Matrix testMatrix = new(2, 2);
            testMatrix[3, 1] = 2;
        }

        [TestMethod]
        [ExpectedException(typeof(MyException))]
        public void TestIndexOperatorGetBigJ()
        {
            Matrix testMatrix = new(2, 2);
            int wrongValue = testMatrix[1, 3];
        }

        [TestMethod]
        [ExpectedException(typeof(MyException))]
        public void TestIndexOperatorSetBigJ()
        {
            Matrix testMatrix = new(2, 2);
            testMatrix[1, 3] = 2;
        }

        [TestMethod]
        public void TestIndexOperatorGet()
        {
            Matrix testMatrix = new(2, 2);
            int wrongValue = testMatrix[1, 1];
        }

        [TestMethod]
        public void TestIndexOperatorSet()
        {
            Matrix testMatrix = new(2, 2);
            testMatrix[1, 1] = 2;
        }

        [TestMethod]
        public void TestSumOperator()
        {
            Matrix firstMatrix = new((new int[,] { { 1, 1 }, { 1, 1 } }), 2, 2);
            Matrix secondMatrix = new((new int[,] { { 2, 2 }, { 2, 2 } }), 2, 2);

            Matrix expectedValue = new((new int[,] { { 3, 3 }, { 3, 3 } }), 2, 2);
            Matrix actualValue = firstMatrix + secondMatrix;

            // Assert.myBalls.XD.fuckoff();
        }

        // [TestMethod]
        // public void Equel()
        // {
        //     //arrange(обеспечить)
        //     Matrix a = new(2, 2);
        //     a[0, 0] = 1;
        //     a[0, 1] = 1;
        //     a[1, 0] = 1;
        //     a[1, 1] = 1;
        //     Matrix b = new(2, 2);
        //     b[0, 0] = 1;
        //     b[0, 1] = 1;
        //     b[1, 0] = 1;
        //     b[1, 1] = 1;
        //     //act (выполнить)
        //     //bool r = a == b;
        //     //assert(доказать)
        //     //Assert.IsTrue(r);
        //     Assert.AreEqual(a, b);
        // }

        // [TestMethod]
        // public void Summa()
        // {
        //     //arrange(обеспечить)
        //     Matrix a = new(2, 2);
        //     a[0, 0] = 1;
        //     a[0, 1] = 1;
        //     a[1, 0] = 1;
        //     a[1, 1] = 1;
        //     Matrix b = new(2, 2);
        //     b[0, 0] = 2;
        //     b[0, 1] = 2;
        //     b[1, 0] = 2;
        //     b[1, 1] = 2;
        //     Matrix expected = new(2, 2);
        //     expected[0, 0] = 3;
        //     expected[0, 1] = 3;
        //     expected[1, 0] = 3;
        //     expected[1, 1] = 3;
        //     Matrix actual = new(2, 2);
        //     //act (выполнить)
        //     actual = a + b;
        //     //assert(доказать)
        //     Assert.IsTrue(actual == expected); //Оракул
        // }
    }
}
