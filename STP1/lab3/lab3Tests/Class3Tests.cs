using lab3;
namespace lab3Tests
{
    [TestClass]
    public class Class3Tests
    {
        [TestMethod]
        public void TestMaxOfThree()
        {
            var expected = 3;
            var actual = Class3.MaxOfThree(1, 2, 3);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TestEvenNumbers()
        {
            var expected = 42;
            var actual = Class3.EvenNumbers(12345);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TestMinNumber()
        {
            var expected = 2;
            var actual = Class3.MinNumber(62543);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TestSumOddNumbers()
        {
            var expected = 19;
            var actual = Class3.SumOddNumbers(new int[,] { { 1, 2, 3 }, { 3, 5, 6 }, { 7, 9, 9 } });
            Assert.AreEqual(expected, actual);

        }
    }
}