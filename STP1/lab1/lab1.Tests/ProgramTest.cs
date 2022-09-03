using lab1;
using System.Diagnostics;

namespace lab1Test
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void TestGetEvenProduct()
        {
            float[] sequence = new[] { 1.5f, 2.5f, 4.2f, 6.3f };

            var actualSequence = Modules.GetEvenProduct(sequence);

            Assert.AreEqual(6.3f, actualSequence, 0.000001);
        }

        [TestMethod]
        public void TestShiftSequence()
        {
            int[] sequence = new[] { 1, 2, 3, 4, 5 };

            var actualSequence = Modules.ShiftSequence(sequence, 2);

            CollectionAssert.AreEqual(new[] { 4, 5, 1, 2, 3 }, actualSequence);
        }

        [TestMethod]
        public void TestGetMaxEvenAndIndex()
        {
            int[] sequence = new[] { 1, 2, 3, 4, 5 };

            var answer = Modules.GetMaxEvenAndIndex(sequence);

            Assert.AreEqual((5, 4), answer);
        }
    }
}