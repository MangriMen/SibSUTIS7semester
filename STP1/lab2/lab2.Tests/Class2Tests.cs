using lab2;
namespace lab2.Tests
{
    [TestClass]
    public class Class2Tests
    {
        [TestMethod]
        public void TestFindMax()
        {
            var expected = 5;
            var actual = Class2.FindMax(5, 3);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TestElementsSum()
        {
            var expected = 6;
            var actual = Class2.ElementsSum(new float[,] { { 1, 2 }, { 3, 4 } });
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TestMinElement()
        {
            var expected = 1;
            var actual = Class2.MinElement(new float[,] { { 1, 2 }, { 3, 4 } });
            Assert.AreEqual(expected, actual);
        }
    }
}