using MathGmp.Native;

namespace Rsa_Application.Cryptography
{
    public class Key
    {
        #region Поля
        public int Bits { get; set; }
        public mpz_t KeyE { get => keyE; }
        public mpz_t KeyN { get => keyN; }
        public mpz_t KeyD { get => keyD; }
        private mpz_t keyE { get; set; }
        private mpz_t keyN { get; set; }
        private mpz_t keyD { get; set; }
        #endregion

        #region Конструкторы класса
        public Key()
        {
            Bits = 0;
            keyE = new mpz_t(); gmp_lib.mpz_init(keyE);
            keyN = new mpz_t(); gmp_lib.mpz_init(keyN);
            keyD = new mpz_t(); gmp_lib.mpz_init(keyD);
        }
        public Key(string keyE, string keyN)
        {
            if (String.IsNullOrEmpty(keyE) || String.IsNullOrEmpty(keyN))
                throw new ArgumentNullException();  
            this.keyE = new mpz_t(); gmp_lib.mpz_init(this.keyE);
            this.keyN = new mpz_t(); gmp_lib.mpz_init(this.keyN);
            gmp_lib.mpz_set_str(this.keyE, new char_ptr(keyE), 10);
            gmp_lib.mpz_set_str(this.keyN, new char_ptr(keyN), 10);
            Bits = gmp_lib.mpz_size(keyN);
            keyD = new mpz_t(); gmp_lib.mpz_init(keyD);
        }
        public Key(string keyE, string keyN, int bits)
        {
            if (String.IsNullOrEmpty(keyE) || String.IsNullOrEmpty(keyN))
                throw new ArgumentNullException();
            Bits = bits;
            this.keyE = new mpz_t(); gmp_lib.mpz_init(this.keyE);
            this.keyN = new mpz_t(); gmp_lib.mpz_init(this.keyN);
            gmp_lib.mpz_set_str(this.keyE, new char_ptr(keyE), 10);
            gmp_lib.mpz_set_str(this.keyN, new char_ptr(keyN), 10);
            keyD = new mpz_t(); gmp_lib.mpz_init(keyD);
        }
        public Key(string keyE, string keyN, string keyD)
        {
            if (String.IsNullOrEmpty(keyE) || String.IsNullOrEmpty(keyN))
                throw new ArgumentNullException();
            this.keyE = new mpz_t(); gmp_lib.mpz_init(this.keyE);
            this.keyN = new mpz_t(); gmp_lib.mpz_init(this.keyN);
            this.keyD = new mpz_t(); gmp_lib.mpz_init(this.keyD);
            gmp_lib.mpz_set_str(this.keyE, new char_ptr(keyE), 10);
            gmp_lib.mpz_set_str(this.keyN, new char_ptr(keyN), 10);
            gmp_lib.mpz_set_str(this.keyD, new char_ptr(keyD), 10);
        }
        public Key(string keyE, string keyN, string keyD, int bits)
        {
            if (String.IsNullOrEmpty(keyE) || String.IsNullOrEmpty(keyN))
                throw new ArgumentNullException();
            Bits = bits;
            this.keyE = new mpz_t(); gmp_lib.mpz_init(this.keyE);
            this.keyN = new mpz_t(); gmp_lib.mpz_init(this.keyN);
            this.keyD = new mpz_t(); gmp_lib.mpz_init(this.keyD);
            gmp_lib.mpz_set_str(this.keyE, new char_ptr(keyE), 10);
            gmp_lib.mpz_set_str(this.keyN, new char_ptr(keyN), 10);
            gmp_lib.mpz_set_str(this.keyD, new char_ptr(keyD), 10);
        }
        public Key(int bits)
        {
            Bits = bits;
            keyE = new mpz_t(); gmp_lib.mpz_init(keyE);
            keyN = new mpz_t(); gmp_lib.mpz_init(keyN);
            keyD = new mpz_t(); gmp_lib.mpz_init(keyD);
        }
        public Key(int bits, string keyE, string keyN, string keyD)
        {
            Bits = bits;
            if (String.IsNullOrEmpty(keyE) || String.IsNullOrEmpty(keyN) || String.IsNullOrEmpty(keyD))
                throw new ArgumentNullException();
            this.keyE = new mpz_t(); gmp_lib.mpz_init(this.keyE);
            this.keyN = new mpz_t(); gmp_lib.mpz_init(this.keyN);
            this.keyD = new mpz_t(); gmp_lib.mpz_init(this.keyD);
            gmp_lib.mpz_set_str(this.keyE, new char_ptr(keyE), 10);
            gmp_lib.mpz_set_str(this.keyN, new char_ptr(keyN), 10);
            gmp_lib.mpz_set_str(this.keyD, new char_ptr(keyD), 10);
        }
        #endregion

        #region Методы
        public void CreateKeys()
        {
            if (Bits < 128 || (Bits > 2048 && !(Bits == 3072 || Bits == 4096 || Bits == 8192 || Bits == 16384)))
                throw new ArgumentException("Длина ключей в бит не соотвествует требованиям");
            if (keyE.ToString().Equals("0") || keyN.ToString().Equals("0"))
            {
                var rand = new Random();
                mpz_t phi = new(), one = new(), t = new(), copy = new(), p = new(), q = new();
                gmp_lib.mpz_init(phi); gmp_lib.mpz_init(one); gmp_lib.mpz_set_si(one, 1); gmp_lib.mpz_init(t); gmp_lib.mpz_init(copy);
                gmp_lib.mpz_init(p); gmp_lib.mpz_init(q);                

                do
                {
                    Primes.GetSafePrimes(Bits, ref p, ref q);
                    gmp_lib.mpz_mul(keyN, p, q); gmp_lib.mpz_sub(p, p, one);
                    gmp_lib.mpz_sub(q, q, one); gmp_lib.mpz_mul(phi, p, q);
                } while (Bits <= 2048 && gmp_lib.mpz_get_str(char_ptr.Zero, 2, keyN).ToString().Length != Bits);


                gmp_randstate_t rs = new();
                gmp_lib.gmp_randinit_mt(rs);

                size_t maxBits = gmp_lib.mpz_sizeinbase(phi, 2);
                gmp_lib.mpz_rrandomb(keyE, rs, new mp_bitcnt_t((uint)rand.Next(4, (int)maxBits)));
                gmp_lib.mpz_set(copy, keyE);
                while (gmp_lib.mpz_cmp(FindNod(copy, phi, keyD, t), one) != 0
                    || gmp_lib.mpz_cmp(keyD, one) == 0
                    || gmp_lib.mpz_cmp(keyD, phi) >= 0)
                {
                    gmp_lib.mpz_rrandomb(keyE, rs, new mp_bitcnt_t((uint)rand.Next(16, (int)maxBits)));
                    gmp_lib.mpz_mul(phi, p, q); gmp_lib.mpz_set(copy, keyE);
                }
                gmp_lib.mpz_mul(phi, p, q);
                gmp_lib.mpz_add(keyD, phi, keyD);

                if (gmp_lib.mpz_cmp(keyE, keyD) >= 0)
                {
                    gmp_lib.mpz_set(copy, keyE);
                    gmp_lib.mpz_set(keyE, keyD);
                    gmp_lib.mpz_set(keyD, copy);
                }
            }
        }
        public void SetPrivateKey(string value)
        {
            if (keyD.ToString().Equals("0") && !keyE.ToString().Equals("0") && !keyN.ToString().Equals("0"))
                gmp_lib.mpz_set_str(keyD, new char_ptr(value), 10);
        }

        private static mpz_t FindNod(mpz_t a, mpz_t b, mpz_t x, mpz_t y) //Расширенный алгоритм
        {                                                             //Евклида для решения уравнения ax+by=d      
            mpz_t q = new(), r = new(), x1 = new(), x2 = new(), y1 = new(), y2 = new(), d = new();
            mpz_t zero = new(); mpz_t mul = new();
            gmp_lib.mpz_init(q); gmp_lib.mpz_init(r); gmp_lib.mpz_init(x1); gmp_lib.mpz_init(x2); gmp_lib.mpz_init(y1); gmp_lib.mpz_init(y2);
            gmp_lib.mpz_init(d); gmp_lib.mpz_init(zero); gmp_lib.mpz_init(mul);
            if (gmp_lib.mpz_cmp(b, zero) == 0)
            {
                d = a; gmp_lib.mpz_set_si(x, 1); gmp_lib.mpz_set_si(y, 0);
                return d;
            }
            gmp_lib.mpz_set_si(x2, 1); gmp_lib.mpz_set_si(x1, 0); gmp_lib.mpz_set_si(y2, 0); gmp_lib.mpz_set_si(y1, 1);
            while (gmp_lib.mpz_cmp(b, zero) > 0)
            {
                gmp_lib.mpz_tdiv_q(q, a, b); gmp_lib.mpz_mul(mul, q, b); gmp_lib.mpz_sub(r, a, mul);
                gmp_lib.mpz_mul(mul, q, x1); gmp_lib.mpz_sub(x, x2, mul);
                gmp_lib.mpz_mul(mul, q, y1); gmp_lib.mpz_sub(y, y2, mul);
                gmp_lib.mpz_set(a, b); gmp_lib.mpz_set(b, r);
                gmp_lib.mpz_set(x2, x1); gmp_lib.mpz_set(x1, x); gmp_lib.mpz_set(y2, y1); gmp_lib.mpz_set(y1, y);
            }
            gmp_lib.mpz_set(d, a); gmp_lib.mpz_set(x, x2); gmp_lib.mpz_set(y, y2);
            return d;
        }
        #endregion
    }
}
