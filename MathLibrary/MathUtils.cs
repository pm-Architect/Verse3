using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public static class MathUtils
    {
        public static double GetFactorial(double num)
        {
            double number = Math.Abs(Math.Round(num));
            double result = 1;
            while (number > 1)
            {
                result *= number;
                number--;
            }
            return result;
        }

        public static double Permutate(double n, double r)
        {
            double number = Math.Abs(Math.Round(n));
            double selection = Math.Abs(Math.Round(r));
            double result;
            if (number > selection)
            {
                result = GetFactorial(number) / GetFactorial(number - selection);
            }
            else
            {
                result = 1;
            }
            return result;
        }

        public static double Combinate(double n, double r)
        {
            double number = Math.Abs(Math.Round(n));
            double selection = Math.Abs(Math.Round(r));
            double result;
            if (number > selection)
            {
                result = GetFactorial(number) / (GetFactorial(selection) * GetFactorial(number - selection));
            }
            else
            {
                result = 1;
            }
            return result;
        }

    }
}
