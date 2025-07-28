namespace TestEnvironment_CS
{
    class A
    {
        public int X { get; set; }
    }

    class B
    {
        public int Y { get; private set; }
    }

    public interface ITestInterface
    {
        int X { get; set; }
        int Y { get; }
    }

    public class TestClass : ITestInterface
    {
        public int X { get; set; }
        public int Y { get; set; }

        public TestClass(int x, int y)
        {
            X = x;
            Y = y;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            // A a = new A { X = 10 };
            // B b = new B { Y = 20 };

            TestClass test = new TestClass(5, 10);
        }
    }

}


