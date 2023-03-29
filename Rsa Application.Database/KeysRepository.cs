using Rsa_Application.Database.Context;
using Rsa_Application.Database.Entities;
using System.Collections.Generic;

namespace Rsa_Application.Database
{
    public class KeysRepository : Repository<Key>
    {
        private readonly KeysDbContext _db;

        public KeysRepository(KeysDbContext db) : base(db) { _db = db; }

        public Key? GetActiveKey() => Items.SingleOrDefault(item => item.State == "активен");

        public void SetActiveKey(Key value)
        {
            var x = Items.SingleOrDefault(item => item.Id == value.Id);
            if (x != null)
            {
                Items.SingleOrDefault(item => item.State == "активен").State = "неактивен";
                Items.SingleOrDefault(item => item.Id == value.Id).State = "активен";
                if (AutoSaveChanges)
                    _db.SaveChanges();
            }
        }
        
        public Key? Get(string _keyE, string _keyN) => Items
            .Where(k => k.KeyE == _keyE && k.KeyN == _keyN)
            .FirstOrDefault();

        public new void Delete(int id)
        {
            var item = Items.Where(item => item.Id == id).FirstOrDefault();
            if (item.State == "активен")
                throw new Exception("Нельзя удалять активный ключ");

            if (item != null)
                _db.Remove(item);

            if (AutoSaveChanges)
                _db.SaveChanges();
        }

    }
}
