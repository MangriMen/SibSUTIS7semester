namespace lab2.Test
{
    [TestClass]
    public class Class1Test
    {
        [TestMethod]
        public void TestMax()
        {
            Assert.AreEqual((50, 38), lab2.Class1.SortDecrement(38, 50));
        }

        [TestMethod]
        public void TestGetEvenProductMatrix()
        {
            int[,] array = new[,] { { 1, 8 }, { 3, 4 }, { 5, 6 } };

            Assert.AreEqual(192, lab2.Class1.GetEvenProductMatrix(array));
        }

        [TestMethod]
        public void TestGetEvenSumLeftTopTriangleMatrix()
        {
            int[,] array = new[,] { { 1, 28, 3 }, { 5, 4, 6 }, { 7, 8, 9 } };

            Assert.AreEqual(32, lab2.Class1.GetEvenSumLeftTopTriangleMatrix(array));
        }
    }
}