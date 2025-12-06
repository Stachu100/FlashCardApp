using System.Security.Cryptography;

namespace APIFlashCard.Utils
{
    public static class PasswordHash
    {
        public static byte[] HashAesPassword(byte[] aesCipher)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(aesCipher);
        }
    }
}