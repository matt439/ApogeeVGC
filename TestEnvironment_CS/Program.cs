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

    public class Test2
    {
        public int? X { get; init; }
    }

    public abstract record AddPart;
    public record StringAddPart(string Value) : AddPart;
    public record IntAddPart(int Value) : AddPart;
    public record BoolAddPart(bool Value) : AddPart;

    class Program
    {
        static void Main(string[] args)
        {
            // A a = new A { X = 10 };
            // B b = new B { Y = 20 };

            //TestClass test = new TestClass(5, 10);
            //Test2 test2;
        }

        public void Add(params AddPart[] parts)
        {
            foreach (var part in parts)
            {
                switch (part)
                {
                    case StringAddPart(var str):
                        // Handle string
                        break;
                    case IntAddPart(var num):
                        // Handle int
                        break;
                    case BoolAddPart(var b):
                        break;
                }
            }
        }
    }

}


