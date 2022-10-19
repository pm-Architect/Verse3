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

        public static string BinomialEquation(double a, double n)
        {
            double number = a;
            double exponent = Math.Abs(Math.Round(n));
            string result = $"(x+{a})^{n} = ";
            int k = 0;

            if (exponent <= 18)
            {
                while (k <= exponent)
                {
                    double numericpart = Combinate(exponent, k) * (Math.Pow(a, n - k));
                    string textpart;
                    if (k < number)
                    {
                        textpart = $"x^{k} + ";
                    }
                    else
                    {
                        textpart = $"x^{k}";
                    }

                    result += numericpart + textpart;
                    k++;
                }
            }
            else
            {
                result = "Please try to keep the exponent n <= 18.";
            }


            return result;
        }

    }
}
