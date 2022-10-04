using LiteDB;
using System.Security.Cryptography;
using System.Text;

namespace Compendium.Database
{
    public static class ObjectIdExt
    {
        public static ObjectId FromHash(string input)
        {
            using MD5 md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var hex = BitConverter.ToString(bytes).Replace("-", "").Substring(0, 24);
            while (hex.Length < 24)
                hex += "0";
            return new ObjectId(hex);
        }
    }
}
