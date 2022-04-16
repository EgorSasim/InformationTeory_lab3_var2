using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{

    

    public  class ElGamal
    {

        public BigInteger p;
        public BigInteger g;
        public BigInteger x;
        public BigInteger y;
        public BigInteger k;
        public BigInteger a;
        public BigInteger b;
        // Generate the prime nums list from selected range; created for generating "p";
        public  List<long> getPrimes(long n)
        {

            bool[] is_prime = new bool[n + 1];
            for (long i = 2; i <= n; ++i)
                is_prime[i] = true;

            List<long> primes = new List<long>();

            for (long i = 2; i <= n; ++i)
                if (is_prime[i])
                {
                    primes.Add(i);
                    if (i * i <= n)
                        for (long j = i * i; j <= n; j += i)
                            is_prime[j] = false;
                }

            return primes;
        }

        // Return random value from passed arg(list<long>); need to generate P value;
        public long getRandPrimeNum(List<long> primes, int p_min)
        {
            return primes[new Random().Next(p_min, primes.Count)];
        }



        //CHECK FOR PRIMAL NUMBER: БЫСТРОЕ ВОЗВЕДЕНИЕ В СТЕПЕНЬ ПО МОДУЛЮ;

        // a - что возводим;
        // b - во что возводим;
        // c - модуль;
        public  BigInteger FastExprModulo(BigInteger a, BigInteger b, BigInteger c)
        {
            BigInteger x = 1;
            while (b != 0)
            {
                while (b % 2 == 0)
                {
                    b = BigInteger.Divide(b, 2);
                    a = BigInteger.Multiply(a, a);
                    a = BigInteger.Remainder(a, c);
                }
                b = BigInteger.Subtract(b, 1);
                x = BigInteger.Multiply(x, a);
                x = BigInteger.Remainder(x, c);
            }
            return x;
        }
        public  bool IsPrime(BigInteger n)
        {
            int k = 100;
            if (n <= 100)
                k = (int)n - 1;
            for (int i = 2; i < k; i++)
            {
                BigInteger a = FastExprModulo(i, n - 1, n);
                if(a != 1)
                    return false;
            }
            return true;
        }


        public BigInteger FastExprModuloB(BigInteger a, BigInteger b, BigInteger c, BigInteger m)
        {
            BigInteger res = new BigInteger(1);

            res = BigInteger.Multiply( BigInteger.Pow(a, (int)b), m ) % c;

            return res;

        }


        // FIND PRIME ROOTS

        public List<BigInteger> findPrimeRoots(BigInteger p)
        {
            List<BigInteger> primeRoots = new List<BigInteger>();
               
            bool isPrimeRoot;
            for (BigInteger g = 1; g <= p - 1; g++)
            {
               
                if (FastExprModulo(g, p-1,p) == 1)
                {
                    isPrimeRoot = true;
                    for(BigInteger l = 1; l <= p - 2; l++)
                    {
                        if ( FastExprModulo(g, l, p) == 1)
                        {
                            isPrimeRoot=false;
                            break;
                        }
                    }
                    if (!isPrimeRoot)
                        continue;
                }
                else
                {
                    continue;
                }
                
                
                if(isPrimeRoot)
                    primeRoots.Add(g);
            }
          
            return primeRoots;
        }


        public BigInteger generatePrimeRoot(BigInteger num)
        {
            bool isPrimeRoot;
            do
            {
                isPrimeRoot = true;
                Random rnd = new Random();

                BigInteger g = getRandomBigInt( 2,  num - 1);

                for (BigInteger l = 1; l <= num - 2; l++)
                {
                    if (FastExprModulo(g, l, num) == 1)
                    {
                        isPrimeRoot = false;
                        break;
                    }
                }

                if (isPrimeRoot == true)
                    return g;

            } while (isPrimeRoot == false);

            return 0;
        }

        // GENERATE RANDOM BIGINTEGER


        //public BigInteger getRandomBigInt(BigInteger start,  BigInteger finish)
        //{
        //    return new Random().Next( (int)start, (finish > 2147483647) ? 2147483647 : (int)finish);


        //}


        //public BigInteger getRandomBigInt(this Random random,
        //BigInteger minValue, BigInteger maxValue)
        //{
        //    if (minValue > maxValue) throw new ArgumentException();
        //    if (minValue == maxValue) return minValue;
        //    BigInteger zeroBasedUpperBound = maxValue - 1 - minValue; // Inclusive
        //    Debug.Assert(zeroBasedUpperBound.Sign >= 0);
        //    byte[] bytes = zeroBasedUpperBound.ToByteArray();
        //    Debug.Assert(bytes.Length > 0);
        //    Debug.Assert((bytes[bytes.Length - 1] & 0b10000000) == 0);

        //    // Search for the most significant non-zero bit
        //    byte lastByteMask = 0b11111111;
        //    for (byte mask = 0b10000000; mask > 0; mask >>= 1, lastByteMask >>= 1)
        //    {
        //        if ((bytes[bytes.Length - 1] & mask) == mask) break; // We found it
        //    }

        //    while (true)
        //    {
        //        random.NextBytes(bytes);
        //        bytes[bytes.Length - 1] &= lastByteMask;
        //        var result = new BigInteger(bytes);
        //        Debug.Assert(result.Sign >= 0);
        //        if (result <= zeroBasedUpperBound) return result + minValue;
        //    }
        //}

        public BigInteger getRandomBigInt(BigInteger min, BigInteger max)
        {
            Random rnd = new Random();
            string numeratorString, denominatorString;
            double fraction = rnd.NextDouble();
            BigInteger inRange;

            //Maintain all 17 digits of precision, 
            //but remove the leading zero and the decimal point;
            numeratorString = fraction.ToString("G17").Remove(0, 2);

            //Use the length instead of 17 in case the random
            //fraction ends with one or more zeros
            denominatorString = string.Format("1E{0}", numeratorString.Length);

            inRange = (max - min) * BigInteger.Parse(numeratorString) /
               BigInteger.Parse(denominatorString,
               System.Globalization.NumberStyles.AllowExponent)
               + min;
            return inRange;
        }

        //

        // GET NOD
        public BigInteger GetNOD(BigInteger val1, BigInteger val2)
        {
            while ((val1 != 0) && (val2 != 0))
            {
                if ( BigInteger.Compare(val1, val2) > 0)
                   val1 %= val2;
                else
                    val2 %= val1;
            }
            return (BigInteger.Compare(val1, val2) > 0 ? val1 : val2); //Math.Max((int)val1, (int)val2);
        }
    }
}
