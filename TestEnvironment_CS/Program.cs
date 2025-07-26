


class A
{
    public int X { get; set; }
}

class B
{
    public int Y { get; private set; }
}


class Program
{
    static void Main(string[] args)
    {
        A a = new A{ X = 10 };
        B b = new B{ Y = 20 };
    }
}

