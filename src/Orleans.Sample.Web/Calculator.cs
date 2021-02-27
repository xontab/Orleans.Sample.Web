namespace Orleans.Sample.Web
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            if (a == 0)
            {
                return b;
            }

            if (b == 0)
            {
                return a;
            }

            return a + b;
        }

        public int Subtract(int a, int b)
        {
            if (a == 0)
            {
                return b;
            }

            if (b == 0)
            {
                return a;
            }

            return a - b;
        }
    }
}