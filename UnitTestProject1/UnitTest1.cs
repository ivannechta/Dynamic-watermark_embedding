using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;




namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {

        [Ignore]
        [TestMethod]
        public void GetDWMSize_Correct()
        {
            /*DWM.DWMConstructor D = new DWM.DWMConstructor();
            D.SetBlockSize(10);
            D.SetSignSize(4);
            D.size = 1;
            Assert.AreEqual(10, D.GetDWMSize());
            D.size = 2;
            Assert.AreEqual(10, D.GetDWMSize());
            D.size = 3;
            Assert.AreEqual(20, D.GetDWMSize());
            D.size = 10;
            Assert.AreEqual(50, D.GetDWMSize());
            D.size = 11;
            Assert.AreEqual(60, D.GetDWMSize());*/
        }

        [Ignore]
        [TestMethod]
        public void GetDWMSize_Error()
        {
            /*DWM.DWMConstructor D = new DWM.DWMConstructor();
            D.SetBlockSize(10);
            D.SetSignSize(5);
            D.size = 1;
            Assert.AreEqual(0, D.GetDWMSize());
            D.SetBlockSize(10);
            D.SetSignSize(6);
            D.size = 1;
            Assert.AreEqual(0, D.GetDWMSize());*/
        }

        [TestMethod]
        public void Sources_constructor()
        {
            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };

            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);
            Assert.AreEqual(10, s.RunLenCnt);
            for (int i = 0; i < s.RunLenCnt; i++)
            {
                Assert.AreEqual(1, s.GetRunLenSize(i));
            }
        }
        [TestMethod]
        public void Sources_MergeBegin()
        {
            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);
            Assert.AreEqual(1, s.GetRunLenSize(0));

            for (int i = 0; i < 8; i++)
            {
                s.MakeMerge(0);
                Assert.AreEqual(i + 2, s.GetRunLenSize(0));
            }
        }

        [TestMethod]
        public void Sources_MergeMiddle1()
        {
            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);

            s.MakeMerge(8);
            Assert.AreEqual(2, s.GetRunLenSize(8));
        }

        [TestMethod]
        public void Sources_MergeMiddle2()
        {
            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);

            s.MakeMerge(8);
            Assert.AreEqual(1, s.GetRunLenSize(9));
        }

        [TestMethod]
        public void Sources_MergeEnd()
        {
            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);

            s.MakeMerge(9);
            Assert.AreEqual(1, s.GetRunLenSize(9));
        }

        [TestMethod]
        public void Sources_GetRunLenCnt1()
        {
            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);

            Assert.AreEqual(10, s.GetRunLenCnt());

            s.MakeMerge(0);
            Assert.AreEqual(9, s.GetRunLenCnt());

            s.MakeMerge(3);
            Assert.AreEqual(8, s.GetRunLenCnt());

            s.MakeMerge(6);
            Assert.AreEqual(7, s.GetRunLenCnt());

            s.MakeMerge(8);
            Assert.AreEqual(6, s.GetRunLenCnt());
        }

        [TestMethod]
        public void Sources_GetRunLenCnt2()
        {

            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);

            Assert.AreEqual(10, s.GetRunLenCnt());

            s.MakeMerge(1);
            s.MakeMerge(2);

            Assert.AreEqual(8, s.GetRunLenCnt());

            s.MakeMerge(0);
            Assert.AreEqual(7, s.GetRunLenCnt());

            s.MakeMerge(9);
            Assert.AreEqual(7, s.GetRunLenCnt());

            s.MakeMerge(9);
            Assert.AreEqual(7, s.GetRunLenCnt());
        }

        [TestMethod]
        public void Sources_GetRunLenCnt3()
        {

            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);

            Assert.AreEqual(10, s.GetRunLenCnt());

            s.MakeMerge(1);
            s.MakeMerge(2);
            s.MakeMerge(2);

            Assert.AreEqual(7, s.GetRunLenCnt());            
        }



        [TestMethod]
        public void Sources_MakeRunLen2()
        {
            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);

            s.RunLenMax = 2;
            s.MakeRunLen();
            Assert.AreEqual(s.RunLenMax, s.GetRunLenCnt());
        }

        [TestMethod]
        public void Sources_MakeRunLen6()
        {
            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);

            s.RunLenMax = 6;
            s.MakeRunLen();
            Assert.AreEqual(s.RunLenMax, s.GetRunLenCnt());
        }

        [TestMethod]
        public void Sources_MakeRunLen12()
        {
            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);

            s.RunLenMax = 12;
            s.MakeRunLen();
            Assert.AreEqual(10, s.GetRunLenCnt());
        }


        [TestMethod]
        public void Sources_GetNextRunLen()
        {
            int DWMSize = 30;
            byte[] DWMBuffer = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 0, 0, 0 };
            DWM.Sources s = new DWM.Sources("", DWMBuffer, DWMSize, 3,2);

            Assert.AreEqual(1, s.GetNextRunLen(0));
            Assert.AreEqual(-1, s.GetNextRunLen(10));

            s.MakeMerge(1);
            s.MakeMerge(2);

            Assert.AreEqual(4, s.GetNextRunLen(1));
        }
    }
}
