<Query Kind="Program">
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

void Main() {
    "fanfeilong".HashWithSalt("123").Dump();
}

// Define other methods and classes here
public static class HashExtention {
    public static string HashWithSalt(this string name, string ssalt) {
        var salt = System.Text.Encoding.UTF8.GetBytes(ssalt);
        var password = System.Text.Encoding.UTF8.GetBytes(name);
        var hmacMD5 = new HMACMD5(salt);
        var saltedHash = hmacMD5.ComputeHash(password);
        var sb = new StringBuilder();
        foreach (byte b in saltedHash) {
            sb.Append(b.ToString("X2"));
        }

        var hashName = sb.ToString();

        return hashName;
    }
}