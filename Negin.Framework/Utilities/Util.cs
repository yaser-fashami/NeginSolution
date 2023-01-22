
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Negin.Framework.Utilities;

public static class Util
{
    public static byte[] GetHash(string inputString)
    {
        return RandomNumberGenerator.GetBytes(128 / 8);
    }

    public static string GetHashString(string inputString)
    {
       var res = KeyDerivation.Pbkdf2(
                    password: inputString!,
                    salt: GetHash(inputString),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);

        return Convert.ToBase64String(res);
    }

    public static Object TrimAllStringFields(Object obj)
    {
        var properties = obj.GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.PropertyType.Name == "String")
            {
                var temp = property.GetValue(obj);
                temp = temp?.ToString()?.Trim();
                if (temp != null)
                {
					property.SetValue(obj, temp, null);
				}
			}
        }

		return obj;
    }

}
