using MathGmp.Native;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Rsa_Application.Cryptography
{
    internal class Primes
    {
        private static readonly string BASE_URL = "https://2ton.com.au/getprimes/random/";

        /// <summary>
        /// Метод для получения двух простых безопасных чисел.
        /// </summary>
        /// <param name="bits">Длина простых чисел в бит.</param>
        /// <param name="p">Первое выходное простое число.</param>
        /// <param name="q">Второе выходное простое число.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void GetSafePrimes(int bits, ref mpz_t p, ref mpz_t q)
        {
            int b = bits % 2 == 0 ? bits / 2 : bits / 2 + 1;
            if (b == 3072 || b == 4096 || b == 8192)
                GetSafePrimesB(b, out p, out q);
            else if (b <= 2048)
                GetSafePrimesL(bits, out p, out q);
            else
                throw new ArgumentException("Неподходящее количество бит для генерации ключей.");
        }

        #region Генерация случайного числа длиной n бит
        private static string RandomNBitNumber(int n)
        {
            byte[] bytes = new byte[n];
            RandomNumberGenerator.Fill(bytes);
            string result = String.Empty;
            result += 1;
            for (int i = 1; i < n; ++i)
                result += bytes[i] % 2 == 0 ? "0" : "1";
            return result;
        }

        private static string RandomNBitNumber(int n, bool isEven)
        {
            byte[] bytes = new byte[n];
            RandomNumberGenerator.Fill(bytes);
            string result = String.Empty;
            result += 1;
            for (int i = 1; i < n - 1; ++i)
                result += bytes[i] % 2 == 0 ? "0" : "1";
            result += isEven ? "0" : "1";
            return result;
        }
        #endregion

        #region Проверка числа n на простоту
        private static void Get_d_s(mpz_t a, out mpz_t d, out mpz_t s)
        {
            d = new(); s = new();
            gmp_lib.mpz_init_set_si(s, 0);
            gmp_lib.mpz_init(d);
            gmp_lib.mpz_sub_ui(d, a, 1);
            while (gmp_lib.mpz_even_p(d) == 0)
            {
                gmp_lib.mpz_tdiv_q_ui(d, d, 2);
                gmp_lib.mpz_add_ui(s, s, 1);
            }
        }

        private static bool MillerRabin(mpz_t n)
        {
            if (gmp_lib.mpz_even_p(n) != 0)
                return false;
            Get_d_s(n, out mpz_t d, out mpz_t s);
            char_ptr number = gmp_lib.mpz_get_str(char_ptr.Zero, 10, n);
            var k = (int)BigInteger.Log(BigInteger.Parse(number.ToString()), 2.0);
            int bits = gmp_lib.mpz_get_str(char_ptr.Zero, 2, n).ToString().Length - 1;
            mpz_t maxval = new(); gmp_lib.mpz_init_set(maxval, n);
            gmp_lib.mpz_sub_ui(maxval, maxval, 1);
            for (int i = 0; i < k; ++i)
            {
                mpz_t a = new(); gmp_lib.mpz_init_set_str(a, new char_ptr(RandomNBitNumber(RandomNumberGenerator.GetInt32(2, bits))), 2);
                gmp_lib.mpz_powm(a, a, d, n);
                if (gmp_lib.mpz_cmp_si(a, 1) != 0 && gmp_lib.mpz_cmp(a, maxval) != 0)
                {
                    mpz_t j = new(); gmp_lib.mpz_init_set_si(j, 1);
                    while (gmp_lib.mpz_cmp(j, s) <= 0)
                    {
                        gmp_lib.mpz_powm_ui(a, a, 2, n);
                        if (gmp_lib.mpz_cmp_si(a, 1) == 0 || gmp_lib.mpz_cmp(a, maxval) == 0) goto l;
                    }
                    return false;
                l:;
                }
            }
            return true;
        }
        #endregion

        #region Проверка числа n на число специального вида
        private static bool IsPrimeOfType(mpz_t n) => IsMersennePrime(n) || IsSophieGermainPrime(n) ||
            IsProthPrime(n) || IsWoodallPrime(n);

        private static bool IsMersennePrime(mpz_t n)
        {
            mpz_t a = new(); gmp_lib.mpz_init_set(a, n);
            gmp_lib.mpz_add_ui(a, a, 1);
            return Regex.IsMatch(gmp_lib.mpz_get_str(char_ptr.Zero, 2, a).ToString(),
                $"^10{{1,}}$");
        }

        private static bool IsSophieGermainPrime(mpz_t n)
        {
            mpz_t a = new(); gmp_lib.mpz_init_set(a, n);
            gmp_lib.mpz_sub_ui(a, a, 1);
            gmp_lib.mpz_tdiv_q_ui(a, a, 2);
            return MillerRabin(a);
        }

        private static bool IsProthPrime(mpz_t n)
        {
            mpz_t a = new(), b = new(); gmp_lib.mpz_init_set(a, n);
            gmp_lib.mpz_init(b);
            gmp_lib.mpz_sub_ui(a, a, 1);
            uint exp = 0;
            while (gmp_lib.mpz_even_p(a) != 0)
            {
                gmp_lib.mpz_tdiv_q_ui(a, a, 2);
                exp++;
            }
            gmp_lib.mpz_ui_pow_ui(b, 2, exp);
            if (gmp_lib.mpz_cmp(a, b) > 0)
                return true;
            return false;
        }

        private static bool IsWoodallPrime(mpz_t n)
        {
            mpz_t a = new(); gmp_lib.mpz_init_set(a, n);
            gmp_lib.mpz_add_ui(a, a, 1);
            uint exp = 0;
            while (gmp_lib.mpz_even_p(a) != 0)
            {
                gmp_lib.mpz_tdiv_q_ui(a, a, 2);
                exp++;
            }
            if (gmp_lib.mpz_cmp_ui(a, exp) == 0)
                return true;
            return false;
        }
        #endregion

        #region Получение двух простых чисел от 128 до 2048 бит
        private static void GetPrimeNumber(int bits, ref mpz_t n)
        {
            do
            {
                do
                {
                    gmp_lib.mpz_set_str(n, new char_ptr(Primes.RandomNBitNumber(bits, false)), 2);
                } while (!MillerRabin(n));
            } while (!IsPrimeOfType(n));
        }

        private static void GetSafePrimesL(int bits, out mpz_t p, out mpz_t q)
        {
            p = new(); q = new();
            gmp_lib.mpz_init(p); gmp_lib.mpz_init(q);

            GetPrimeNumber(bits / 2, ref p);

            bits = bits % 2 == 1 ? bits / 2 + 1 : bits / 2;
            do
            {
                GetPrimeNumber(bits, ref q);
            } while (gmp_lib.mpz_cmp(p, q) == 0);
        }
        #endregion

        #region Получение двух простых чисел длиной 3072, 4096 или 8192 бит
        private static void GetSafePrimesB(int bits, out mpz_t p, out mpz_t q)
        {
            p = new mpz_t(); q = new mpz_t();
            gmp_lib.mpz_init(p); gmp_lib.mpz_init(q);
            string url = @$"{BASE_URL}{bits}";
            using WebClient webClient = new();
            webClient.BaseAddress = url;
            var nodes = webClient.DownloadString(url).Split(new char[] { ':', '"' }, StringSplitOptions.RemoveEmptyEntries);
            p = new mpz_t();
            q = new mpz_t();
            gmp_lib.mpz_init_set_str(p, new char_ptr(nodes[7]), 10);
            gmp_lib.mpz_init_set_str(q, new char_ptr(nodes[18]), 10);
        }
        #endregion
    }
}
