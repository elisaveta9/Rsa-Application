namespace Rsa_Application.Infrastructure.Interfaces
{
    internal interface IFileService
    {
        void Open(string path, ref string fileText, out bool isCrypto, out string opt, out string keyE, out string keyN);
        void Save(string path, string text);
        void Save(string path, string keyE, string keyN, string opt, string ciphertext);
    }
}
