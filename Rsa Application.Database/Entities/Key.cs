using Rsa_Application.Database.Entities.Base;

namespace Rsa_Application.Database.Entities
{
    public class Key : Entity
    {
        #region Поля
        public override int Id { get; }
        public override string Name { get; set; }
        public int Bits { get; set; }
        public string State { get; set; }
        public string CreateDate { get; set; }
        public string KeyE { get; set; }
        public string KeyN { get; set; }
        public string KeyD { get; set; }
        #endregion

        public Key(string name, string createDate, string state, int bits, string keyE, string keyN, string keyD)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(state))
                throw new ArgumentNullException();
            if (bits < 128)
                throw new ArgumentOutOfRangeException();
            if (string.IsNullOrEmpty(keyE) && string.IsNullOrEmpty(keyN) && string.IsNullOrEmpty(keyD))
            {
                Cryptography.Key key = new(bits);
                key.CreateKeys();
                KeyE = key.KeyE.ToString();
                KeyN = key.KeyN.ToString();
                KeyD = key.KeyD.ToString();
            }
            else
            {
                KeyE = keyE;
                KeyN = keyN;
                KeyD = keyD;
            }
            Name = name;
            Bits = bits;
            State = state;
            CreateDate = string.IsNullOrEmpty(createDate) ? DateTime.Now.ToString() : createDate;
        }
    }
}
