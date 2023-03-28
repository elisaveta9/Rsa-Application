using MathGmp.Native;
using System;
using System.Text;

namespace Rsa_Application.Cryptography
{
    internal static class Primitives
    {
        internal static byte[] I2OSP(int x, int size)
        {
            mpz_t set = new mpz_t(); gmp_lib.mpz_init_set_si(set, x);
            return I2OSP(set, size);
        }
        internal static byte[] I2OSP(string x, int size)
        {
            mpz_t set = new mpz_t(); gmp_lib.mpz_init_set_str(set, new char_ptr(x), 10);
            return I2OSP(set, size);
        }
        internal static byte[] I2OSP(mpz_t x, int size)
        {
            mpz_t @base = new mpz_t(); gmp_lib.mpz_init_set_si(@base, 256);
            mpz_t opt = new mpz_t(); gmp_lib.mpz_init(opt);
            gmp_lib.mpz_pow_ui(opt, @base, (uint)size);
            if (gmp_lib.mpz_cmp(x, opt) >= 0)
                throw new ArgumentException("Число x слишком большое");
            byte[] result = new byte[size];
            gmp_lib.mpz_set(opt, x);
            for (int i = 0; i < size; i++)
            {
                mpz_t num = new mpz_t(); gmp_lib.mpz_init(num);
                gmp_lib.mpz_mod_ui(num, opt, 256);
                result[i] = byte.Parse(num.ToString());
                gmp_lib.mpz_fdiv_q(opt, opt, @base);
            }
            Array.Reverse(result);
            return result;
        }

        internal static mpz_t OS2IP(byte[] x)
        {
            mpz_t result = new mpz_t(); gmp_lib.mpz_init(result);
            int pow = x.Length - 1;
            for (uint i = 0; i < x.Length; ++i, --pow)
            {
                mpz_t num = new mpz_t(); gmp_lib.mpz_init_set_ui(num, 256);
                gmp_lib.mpz_pow_ui(num, num, (uint)pow);
                gmp_lib.mpz_mul_ui(num, num, x[i]);
                gmp_lib.mpz_add(result, result, num);
            }
            return result;
        }

        internal static byte[] RSAEP(Key rsa, byte[] source)
        {
            int len = gmp_lib.mpz_get_str(char_ptr.Zero, 16, rsa.KeyN).ToString().Length, index = 0;
            string[] _res = new string[source.Length];
            byte[] result = new byte[len * source.Length];
            mpz_t _char = new mpz_t(); gmp_lib.mpz_init(_char);

            for (int i = 0; i < source.Length; ++i)
            {
                gmp_lib.mpz_set_si(_char, source[i]);
                gmp_lib.mpz_powm(_char, _char, rsa.KeyE, rsa.KeyN);
                _res[i] = gmp_lib.mpz_get_str(char_ptr.Zero, 16, _char).ToString();
            }

            foreach (string num in _res)
            {
                var bnum = Encoding.ASCII.GetBytes(num);
                Array.Copy(bnum, 0, result, index + len - bnum.Length, bnum.Length);
                index += len;
            }

            return result;
        }

        internal static mpz_t RSAEP(Key rsa, mpz_t source)
        {
            if (gmp_lib.mpz_cmp(source, rsa.KeyN) >= 0)
                throw new Exception("Невозможно зашифровать данным ключом. Шифр будет неодназначным.");
            mpz_t result = new mpz_t(); gmp_lib.mpz_init(result);
            gmp_lib.mpz_powm(result, source, rsa.KeyE, rsa.KeyN);
            return result;
        }

        internal static byte[] RSADP(Key rsa, byte[] crypto)
        {
            int len = gmp_lib.mpz_get_str(char_ptr.Zero, 16, rsa.KeyN).ToString().Length;
            string[] cripttext = new string[crypto.Length / len];
            byte[] source = new byte[cripttext.Length];

            for (int i = 0, j = 0; i < crypto.Length; i += len)
            {
                int index = i;
                while (crypto[index++] == 0) ;
                index--;
                byte[] num = new byte[len - index + i];
                Array.Copy(crypto, index, num, 0, num.Length);
                cripttext[j++] = Encoding.ASCII.GetString(num);
            }

            mpz_t _char = new mpz_t(); gmp_lib.mpz_init(_char);
            int s = 0;
            foreach (string cryptoNum in cripttext)
            {
                gmp_lib.mpz_set_str(_char, new char_ptr(cryptoNum), 16);
                gmp_lib.mpz_powm(_char, _char, rsa.KeyD, rsa.KeyN);
                source[s++] = byte.Parse(_char.ToString());
            }

            return source;
        }

        internal static mpz_t RSADP(Key rsa, mpz_t crypto)
        {
            if (gmp_lib.mpz_cmp(crypto, rsa.KeyN) >= 0)
                throw new Exception("Текст был зашифрован другим ключом");
            mpz_t result = new mpz_t(); gmp_lib.mpz_init(result);
            gmp_lib.mpz_powm(result, crypto, rsa.KeyD, rsa.KeyN);
            return result;
        }

        internal static byte[] Concate(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length + b.Length];
            Array.Copy(a, 0, result, 0, a.Length);
            Array.Copy(b, 0, result, a.Length, b.Length);
            return result;
        }

        internal static byte[] AddByte(byte[] a, byte b)
        {
            byte[] result = new byte[a.Length + 1];
            Array.Copy(a, 0, result, 0, a.Length);
            result[a.Length] = b;
            return result;
        }

        internal static byte[] AddByte(byte b, byte[] a)
        {
            byte[] result = new byte[a.Length + 1];
            Array.Copy(a, 0, result, 1, a.Length);
            result[0] = b;
            return result;
        }

        internal static byte[] Xor(byte[] a, byte[] b)
        {
            byte[] result;
            if (a.Length == b.Length)
            {
                result = new byte[a.Length];
                for (int i = 0; i < result.Length; ++i)
                    result[i] = (byte)(a[i] ^ b[i]);
            }
            else
            {
                byte[] max, min;
                if (b.Length > a.Length)
                {
                    max = b; min = new byte[max.Length];
                    Array.Copy(a, 0, min, b.Length - a.Length, a.Length);
                }
                else
                {
                    max = a; min = new byte[max.Length];
                    Array.Copy(b, 0, min, a.Length - b.Length, b.Length);
                }
                result = new byte[max.Length];
                for (int i = 0; i < result.Length; ++i)
                    result[i] = (byte)(max[i] ^ min[i]);
            }
            return result;
        }

        internal static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        internal static string RandomNumber(Random random, int bits)
        {
            StringBuilder number = new StringBuilder("1");
            for (int i = 1; i < bits; i++)
                number.Append(random.Next(0, 2));
            return number.ToString();
        }
    }
}
