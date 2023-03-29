using MathGmp.Native;
using System.Security.Cryptography;
using System.Text;

namespace Rsa_Application.Cryptography
{
    public enum Options
    {
        SimpleRSA,
        RSA_withAddBytes,
        RSA_withCompBytes,
        RSAES_OAEP
    }
    public class Rsa
    {
        private static readonly Random random = new();
        private static HashAlgorithm hash = new SHA256Managed();
        private static Key _rsaKey;
        #region Поля
        public Key RsaKey { set => _rsaKey = value; }
        public HashAlgorithm Hash { get => hash; set => hash = value; }
        public Options options = Options.RSAES_OAEP;
        #endregion

        #region Конструкторы
        public Rsa() => _rsaKey = new Key();
        public Rsa(int _bits) { _rsaKey = new Key(_bits); _rsaKey.CreateKeys(); }
        public Rsa(Key rsaKey) => _rsaKey = rsaKey;
        public Rsa(string _eKey, string _nKey) => _rsaKey = new Key(_eKey, _nKey);
        public Rsa(string _eKey, string _nKey, int _bits) => _rsaKey = new Key(_eKey, _nKey, _bits);
        public Rsa(string _eKey, string _nKey, string _dKey) => _rsaKey = new Key(_eKey, _nKey, _dKey);
        public Rsa(string _eKey, string _nKey, string _dKey, int _bits) => _rsaKey = new Key(_eKey, _nKey, _dKey, _bits);
        public Rsa(int _bits, Options _options) 
            { _rsaKey = new Key(_bits); _rsaKey.CreateKeys(); options = _options; }
        public Rsa(Key rsaKey, Options _options) 
            { _rsaKey = rsaKey; options = _options; }
        public Rsa(string _eKey, string _nKey, Options _options) 
            { _rsaKey = new Key(_eKey, _nKey); options = _options; }
        public Rsa(string _eKey, string _nKey, int _bits, Options _options) 
            { _rsaKey = new Key(_eKey, _nKey, _bits); options = _options; }
        public Rsa(string _eKey, string _nKey, string _dKey, Options _options) 
            { _rsaKey = new Key(_eKey, _nKey, _dKey); options = _options; }
        public Rsa(string _eKey, string _nKey, string _dKey, int _bits, Options _options) 
            { _rsaKey = new Key(_eKey, _nKey, _dKey, _bits); options = _options; }
        #endregion

        #region Методы
        public static void CreateKeys()
        {
            if (_rsaKey.Bits == 0)
                throw new ArgumentNullException("Не задано количество бит для ключей!");
            _rsaKey.CreateKeys();
        }
        public string Encrypt(string text)
        {
            if (text.Length == 0)
                return "";
            if (_rsaKey.KeyE.ToString().Equals("0") || _rsaKey.KeyN.ToString().Equals("0"))
                throw new ArgumentNullException();

            if (options == Options.SimpleRSA)
                return EncryptionSimpleRSA(Encoding.UTF8.GetBytes(text));
            if (options == Options.RSA_withAddBytes)
                return EncryptionRSAAdd(Encoding.UTF8.GetBytes(text));
            if (options == Options.RSA_withCompBytes)
                return EncryptionRSAConcat(Encoding.UTF8.GetBytes(text));
            if (options == Options.RSAES_OAEP)
            {
                if (_rsaKey.Bits % 8 != 0)
                    throw new Exception("Используйте для шифрования RSAES_OAEP ключи, длина которых в бит кратна 8");

                byte[] M = Encoding.UTF8.GetBytes(text);
                byte[] result = Array.Empty<byte>();
                int k = _rsaKey.Bits / 8, mLen = M.Length, hLen = hash.HashSize / 8, max = k - 2 * hLen - 2;

                if (max <= 0)
                    throw new Exception("Длина ключа в бит мала для шифрования RSAES-OAEP");

                for (int i = 0; i < mLen; i += max)
                {
                    byte[] m = new byte[Math.Min(max, mLen - i)];
                    Array.Copy(M, i, m, 0, m.Length);
                    result = Primitives.Concate(result, Encrypt_OAEP(Array.Empty<byte>(), m));
                }
                return Convert.ToBase64String(result);
            }

            throw new ArgumentNullException("Не задан шифр!");
        }
        public string Decrypt(string ciphertext)
        {
            if (String.IsNullOrEmpty(ciphertext))
                return "";
            if (_rsaKey.KeyD.ToString().Equals("0"))
                throw new ArgumentNullException("Не задан ключ KeyD");

            if (options == Options.SimpleRSA)
                return DecryptionSimpleRSA(ciphertext.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            if (options == Options.RSA_withAddBytes)
                return DecryptionRSAAdd(ciphertext.Split(new char[] { '|', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            if (options == Options.RSA_withCompBytes)
                return DecryptionRSAConcat(ciphertext.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            if (options == Options.RSAES_OAEP)
            {

                int k = _rsaKey.Bits / 8;
                byte[] C = Convert.FromBase64String(ciphertext);
                if (C.Length % k != 0)
                    throw new Exception("Ошибка расшифровки");
                byte[] result = Array.Empty<byte>();
                for (int i = 0; i < C.Length; i += k)
                {
                    byte[] c = C[i..(i + k)];
                    result = Primitives.Concate(result, Decrypt_OAEP(Array.Empty<byte>(), c));
                }
                return Encoding.UTF8.GetString(result);
            }

            throw new ArgumentNullException("Не задан шифр!");
        }
        #endregion

        #region Шифр RSA без преобразований текста
        private static string EncryptionSimpleRSA(byte[] text)
        {
            StringBuilder cryptotext = new(text.Length);
            mpz_t _char = new(); gmp_lib.mpz_init(_char);
            char_ptr s;

            foreach (byte b in text)
            {
                gmp_lib.mpz_set_si(_char, b);
                gmp_lib.mpz_powm(_char, _char, _rsaKey.KeyE, _rsaKey.KeyN);
                s = gmp_lib.mpz_get_str(char_ptr.Zero, 16, _char);
                cryptotext.Append($"{s.ToString()} ");
            }

            return cryptotext.ToString();
        }
        private static string DecryptionSimpleRSA(string[] ciphertext)
        {
            byte[] bytes = new byte[ciphertext.Length];
            int i = 0;
            mpz_t _char = new(); gmp_lib.mpz_init(_char);

            foreach (string cryptoNum in ciphertext)
            {
                gmp_lib.mpz_set_str(_char, new char_ptr(cryptoNum), 16);
                gmp_lib.mpz_powm(_char, _char, _rsaKey.KeyD, _rsaKey.KeyN);
                bytes[i++] = (byte.Parse(_char.ToString()));
            }

            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region Шифр RSA с начальной случайной переменной и с увеличением переменной для шифрования
        private static string EncryptionRSAAdd(byte[] text)
        {
            StringBuilder cryptotext = new();
            mpz_t _control = new(); gmp_lib.mpz_init_set_si(_control, text.Length * 8);
            gmp_lib.mpz_sub(_control, _rsaKey.KeyN, _control);
            size_t size = gmp_lib.mpz_sizeinbase(_control, 2) / 2;
            if (size < 4) return "";

            mpz_t _char = new(); gmp_lib.mpz_init(_char);
            mpz_t res = new(); gmp_lib.mpz_init(res);
            mpz_t r = new(); gmp_lib.mpz_init(r); gmp_lib.mpz_set_str(r, 
                new char_ptr(Primitives.RandomNumber(random, (int)size)), 2);
            gmp_lib.mpz_powm(res, r, _rsaKey.KeyE, _rsaKey.KeyN);
            char_ptr s = gmp_lib.mpz_get_str(char_ptr.Zero, 16, res);
            cryptotext.Append($"{s.ToString()}|");

            foreach (byte b in text)
            {
                gmp_lib.mpz_set_si(_char, b);
                gmp_lib.mpz_add(r, r, _char);
                gmp_lib.mpz_powm(res, r, _rsaKey.KeyE, _rsaKey.KeyN);
                s = gmp_lib.mpz_get_str(char_ptr.Zero, 16, res);
                cryptotext.Append($"{s.ToString()}|");
            }
            return cryptotext.ToString();
        }
        private static string DecryptionRSAAdd(string[] cryptoText)
        {
            byte[] bytes = new byte[cryptoText.Length - 1];
            mpz_t _char = new(); gmp_lib.mpz_init(_char);
            mpz_t res = new(); gmp_lib.mpz_init(res);
            mpz_t r = new(); gmp_lib.mpz_init(r);
            gmp_lib.mpz_set_str(r, new char_ptr(cryptoText[0]), 16);
            gmp_lib.mpz_powm(r, r, _rsaKey.KeyD, _rsaKey.KeyN);

            for (int i = 1; i < cryptoText.Length - 1; i++)
            {
                gmp_lib.mpz_set_str(_char, new char_ptr(cryptoText[i]), 16);
                gmp_lib.mpz_powm(_char, _char, _rsaKey.KeyD, _rsaKey.KeyN);
                gmp_lib.mpz_sub(res, _char, r);
                gmp_lib.mpz_set(r, _char);
                bytes[i - 1] = (byte.Parse(res.ToString()));
            }
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region Шифр RSA с предварительной конкатенацией байт
        private static string EncryptionRSAConcat(byte[] text)
        {
            StringBuilder cryptotext = new(text.Length);
            StringBuilder number = new();
            int maxNumLen = _rsaKey.KeyN.ToString().Length / 3, index = 0;
            char_ptr s;

            while (index < text.Length)
            {
                mpz_t buf = new(); gmp_lib.mpz_init(buf);
                int endReg = Math.Min(text.Length, index + maxNumLen);
                while (index < endReg)
                    if (text[index] < 100)
                        if (text[index] < 10) number.Append($"00{text[index++]}");
                        else number.Append($"0{text[index++]}");
                    else number.Append(text[index++]);
                gmp_lib.mpz_set_str(buf, new char_ptr(number.ToString()), 10);
                while (gmp_lib.mpz_cmp(buf, _rsaKey.KeyN) > 0)
                {
                    number.Remove(number.Length - 3, 3);
                    gmp_lib.mpz_set_str(buf, new char_ptr(number.ToString()), 10);
                    index--;
                }

                gmp_lib.mpz_powm(buf, buf, _rsaKey.KeyE, _rsaKey.KeyN);
                s = gmp_lib.mpz_get_str(char_ptr.Zero, 16, buf);
                cryptotext.Append($"{s.ToString()} ");

                number = new StringBuilder();
            }

            return cryptotext.ToString();
        }
        private static string DecryptionRSAConcat(string[] ciphertext)
        {
            List<byte> source = new();
            mpz_t _char = new(); gmp_lib.mpz_init(_char);

            foreach (var cryptoNum in ciphertext)
            {
                gmp_lib.mpz_set_str(_char, new char_ptr(cryptoNum), 16);
                gmp_lib.mpz_powm(_char, _char, _rsaKey.KeyD, _rsaKey.KeyN);
                var res = _char.ToString();
                if (res.Length % 3 == 1) res = $"00{res}";
                else if (res.Length % 3 == 2) res = $"0{res}";

                var bytes = Primitives.Split(res, 3);
                foreach (var b in bytes)
                {
                    byte ch = byte.Parse(b);
                    source.Add(ch);
                }
            }

            return Encoding.UTF8.GetString(source.ToArray());
        }
        #endregion

        #region RSAEP-OAEP
        private static byte[] Encrypt_OAEP(byte[] L, byte[] M)
        {
            if (L.Length > Math.Pow(2, 32) - 1)
                throw new ArgumentOutOfRangeException("Длина дополнения слишком большая");
            int k = _rsaKey.Bits / 8, mLen = M.Length, hLen = hash.HashSize / 8;
            if (mLen > k - 2 * hLen - 2)
                throw new ArgumentOutOfRangeException("Длина сообщения слишком большая.");

            var lHash = hash.ComputeHash(L);
            byte[] PS = new byte[k - mLen - 2 * hLen - 2];
            byte[] DB;
            if (PS.Length == 0)
                DB = Primitives.Concate(Primitives.AddByte(lHash, 0x01), M);
            else
                DB = Primitives.Concate
                    (Primitives.AddByte(Primitives
                    .Concate(lHash, PS), 0x01), M);
            byte[] seed = new byte[hLen];
            random.NextBytes(seed);
            byte[] dbMask = MGF1.GetMask(hash, seed, k - hLen - 1);
            byte[] maskedDB = Primitives.Xor(DB, dbMask);
            byte[] seedMask = MGF1.GetMask(hash, maskedDB, hLen);
            byte[] maskedSeed = Primitives.Xor(seed, seedMask);
            byte[] EM = Primitives.AddByte(0x00,
                Primitives.Concate(maskedSeed, maskedDB));

            var m = Primitives.OS2IP(EM);
            var c = Primitives.RSAEP(_rsaKey, m);
            return Primitives.I2OSP(c, k);
        }
        private static byte[] Decrypt_OAEP(byte[] L, byte[] C)
        {
            int k = _rsaKey.Bits / 8, hLen = hash.HashSize / 8;
            if (L.Length > Math.Pow(2, 32) - 1)
                throw new ArgumentOutOfRangeException("Длина дополнения слишком большая");
            if (C.Length != k)
                throw new Exception("Ошибка расшифровки");
            if (k < 2 * hLen + 2)
                throw new Exception("Ошибка расшифровки");

            var c = Primitives.OS2IP(C);
            var m = Primitives.RSADP(_rsaKey, c);
            var EM = Primitives.I2OSP(m, k);

            var lHash = hash.ComputeHash(L);
            byte[] Y = new byte[1],
                maskedSeed = new byte[hLen],
                maskedDB = new byte[k - hLen - 1];
            Array.Copy(EM, 0, Y, 0, Y.Length);
            Array.Copy(EM, 1, maskedSeed, 0, hLen);
            Array.Copy(EM, 1 + hLen, maskedDB, 0, k - hLen - 1);
            byte[] seedMask = MGF1.GetMask(hash, maskedDB, hLen);
            byte[] seed = Primitives.Xor(maskedSeed, seedMask);
            byte[] dbMask = MGF1.GetMask(hash, seed, k - hLen - 1);
            byte[] DB = Primitives.Xor(maskedDB, dbMask);
            int i = hLen;
            while (DB[i++] == 0x00) ;
            if (DB[i - 1] != 0x01)
                throw new Exception("Ошибка расшифровки");

            byte[] M = new byte[DB.Length - i];
            Array.Copy(DB, i, M, 0, M.Length);
            return M;
        }
        #endregion
    }
}