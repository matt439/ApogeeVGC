namespace ApogeeVGC_CS.lib
{
    public static class Utilities
    {
        public static int ClampIntRange(int num, int? min = null, int? max = null)
        {
            if (num < min)
            {
                num = min.Value;
            }
            if (num > max)
            {
                num = max.Value;
            }
            return num;
        }
    }
}
