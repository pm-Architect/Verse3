using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using MS.WindowsAPICodePack.Internal;
using Newtonsoft.Json;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public static class MathUtils
    {
        //TODO: Data Structure Traversal Utilities
        //    DataStructure<double> result = new DataStructure<double>();
        //    if (aDS.Count > 0)
        //    {
        //        foreach (IDataGoo<double> goo in aDS)
        //        {
        //            if (bDS.Count > 0)
        //            {
        //                foreach (IDataGoo<double> goo2 in bDS)
        //                {
        //                    result.Add(goo.Data + goo2.Data);
        //                }
        //            }
        //            else if (bDS.Data is double b)
        //            {
        //                result.Add(goo.Data + b);
        //            }
        //        }
        //    }
        //    else if (aDS.Data is double a)
        //    {
        //        if (bDS.Count > 0)
        //        {
        //            foreach (IDataGoo<double> goo2 in bDS)
        //            {
        //                result.Add(a + goo2.Data);
        //            }
        //        }
        //        else if (bDS.Data is double b)
        //        {
        //            result.Add(a + b);
        //        }
        //    }

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

        public static bool IsPrime(double num)
        {
            int number = (int)Math.Round(num);
            bool result = true;
            if (number <= 0)
            {
                return false;
            }

            if (number > 2)
            {
                int k = 2;
                while (k <= (number - 1))
                {
                    if (number % k == 0)
                    {
                        result = false;
                        break;
                    }
                    k++;
                }
            }

            return result;

        }

        public static bool IsOdd(double num)
        {
            int number = (int)Math.Round(num);
            if (number % 2 == 0)
            { return false; }
            else
            { return true; }
        }

        public static DataStructure<double> FibonacciSequence(int num)
        {
           
            double iterator = 1;
            DataStructure<double> result = new DataStructure<double>();
            result.Add(iterator);

            if (num > 1)
            {
                result.Add(iterator);

                for (int i = 1; i < num - 1; i++)
                {
                    iterator = (double)result[i].Data + (double)result[i - 1].Data;
                    result.Add(iterator);
                }              
            }
            return result;
        }

        public static DataStructure FlattenDataStructure(DataStructure data)
        {

            DataStructure result = new DataStructure();
            

            //for (int i = 0; i < data.Count; i++)
            //{
            //    if (data[i] is DataStructure ds)
            //    {
            //        DataStructure flattened = FlattenDataStructure(ds);
            //        result.Add(flattened);
            //    }
            //}

            return result;

        }

        public static DataStructure SortData(DataStructure input, bool desc)
        {
            DataStructure result = new DataStructure();
            IEnumerable<object> sortedData;

            if (desc)
            {
                sortedData = from inputItem in input orderby inputItem.Data descending select inputItem;
            }
            else
            {
                sortedData = from inputItem in input orderby inputItem.Data select inputItem;
            }


            if (sortedData != null)
            {
                foreach (var item in sortedData)
                {
                    result.Add(item);
                }
            }

            return result;
        }


        public static double? Median(DataStructure source)
        {

            int count = source.Count();
            
            SortData(source, false);

            int midpoint = count / 2;

            if (source[0].Data is double)
            {
                if (count % 2 == 0)
                    return (Convert.ToDouble(source.ElementAt(midpoint - 1).Data) + Convert.ToDouble(source.ElementAt(midpoint).Data)) / 2.0;
                else
                    return Convert.ToDouble(source.ElementAt(midpoint).Data);
            }

            else return null;

        }

        public static double? SampleStandardDeviation(DataStructure source)
        {

            if (source is null || source[0].Data is not double) return null;

            int size = source.Count;
            double result = 0;
            double sum = 0;
            for (int i = 0; i < source.Count; i++)
            {
                    sum += (double)source[i].Data;                
            }
            double mean = sum / size;

            double squaresum = 0;
            for (int i = 0; i < source.Count; i++)
            {
                squaresum += Math.Pow(((double)source[i].Data - mean),2);
            }
            result = Math.Sqrt(squaresum / (size - 1));

            return result;

        }

        public class JsonDict : Dictionary<string, object>, IDisposable
        {
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }

            public void Dispose()
            {
                Clear();
            }
        }
    }

}
