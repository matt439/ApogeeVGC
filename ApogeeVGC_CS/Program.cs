namespace ApogeeVGC_CS
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Test1V1 test = new Test1V1();
            await test.RunTest();
        }
    }
}
