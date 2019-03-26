using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Security.Cryptography;

namespace MyRSA
{

    class NumberTheory
    {
        // calculate x^p % mod in 0(log(p))
        public BigInteger square_and_multiply(BigInteger x, BigInteger p, BigInteger mod)
        {
            BigInteger res = 1;
            while (p > 0)
            {
                if (p % 2 == 1)
                    res = (res * x) % mod;
                x = (x * x) % mod;
                p /= 2;
            }
            return res;
        }

        // calculate GCD of a and b
        public BigInteger myGcd(BigInteger a, BigInteger b)
        {
            if (b == 0) return a;
            return myGcd(b, a % b);
        }

        // calculate modular multiplicative inverse of a under modulo b
        // (xn * a) % b = 1
        // non-recursive Extended Euclidian Algorithm
        public BigInteger inverseModulo(BigInteger a, BigInteger b)
        {
            BigInteger x0 = 1, xn = 1, y0 = 0, yn = 0, x1 = 0, y1 = 1, f, r = a % b;
            BigInteger tmp = b;

            while (r > 0)
            {
                f = a / b;
                xn = x0 - f * x1;
                yn = y0 - f * y1;

                x0 = x1;
                y0 = y1;
                x1 = xn;
                y1 = yn;
                a = b;
                b = r;
                r = a % b;
            }
            return ((xn % tmp) + tmp) % tmp;
        }

    }

    class PrimeGenerator : NumberTheory
    {
        public List<int> primes;

        public PrimeGenerator()
        {
            primes = new List<int>();
            for (int i = 2; primes.Count() < 1e4; i++)
                if (isPrime(i))
                    primes.Add(i);
        }

        private bool isPrime(int x)
        {
            for (int i = 2; i * i <= x; i++)
                if (x % i == 0)
                    return false;
            return true;
        }

        private BigInteger getRandom()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] randomNumber = new byte[64];
            rng.GetBytes(randomNumber);
            BigInteger result = new BigInteger(randomNumber);
            result = BigInteger.Abs(result);
            if (result % 2 == 0)
                result = result - 1;
            return result;
        }

        private bool fermatTest(BigInteger x, int lim = 100)
        {
            for (int i = 0; i < primes.Count(); i++)
                if (x % primes[i] == 0)
                    return false;

            for (int i = 0; i < lim; i++)
            {
                BigInteger curRandom = getRandom() % (x - 2) + 2;
                if (myGcd(curRandom, x) != 1)
                    return false;
                if (square_and_multiply(curRandom, x - 1, x) != 1)
                    return false;
            }
            return true;
        }

        public BigInteger genPrime()
        {
            BigInteger x = getRandom();
            while (fermatTest(x) == false)
            {
                x = getRandom();
                for (int i = 0; i < 700; i++)
                {
                    if (fermatTest(x) == true)
                        break;
                    x = x + 2;
                }
            }
            return x;
        }
    }



    class KeysPair : NumberTheory
    {
        BigInteger p, q, d, exp, n, phi;
        BigInteger dP, dQ, qInv;

        public KeysPair(BigInteger _p, BigInteger _q, BigInteger _exp)
        {
            p = _p;
            q = _q;
            if (p < q)
            {
                BigInteger tmp = q;
                q = p;
                p = tmp;
            }
            exp = _exp;
            phi = (p - 1) * (q - 1);
            n = p * q;
            d = inverseModulo(exp, phi);
            dP = inverseModulo(exp, p - 1);
            dQ = inverseModulo(exp, q - 1);
            qInv = inverseModulo(q, p);
        }

        public Tuple<BigInteger, BigInteger> getPrivateKey()
        {
            return Tuple.Create(d, n);
        }

        public Tuple<BigInteger, BigInteger> getPublicKey()
        {
            return Tuple.Create(exp, n);
        }

        public BigInteger encrypt(BigInteger text)
        {
            return square_and_multiply(text, exp, n);
        }

        public BigInteger decrypt(BigInteger cyphertext)
        {
            BigInteger m1 = square_and_multiply(cyphertext, dP, p);
            BigInteger m2 = square_and_multiply(cyphertext, dQ, q);
            BigInteger h = (((qInv * (m1 - m2)) % p) + p) % p;
            BigInteger text = m2 + h * q;
            return text;
            //return square_and_multiply(cyphertext, d, n);
        }

    }

    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}