<Query Kind="Program" />

void Main() {
    "__div".IsLower().Dump();
    "Div".IsLower().Dump();
}

// Define other methods and classes here
public static class StringExtension {
    public static bool IsUpper(this string value) {
        // Consider string to be uppercase if it has no lowercase letters.
        for (int i = 0; i < value.Length; i++) {
            if (char.IsLower(value[i])) {
                return false;
            }
        }
        return true;
    }

    public static bool IsLower(this string value) {
        // Consider string to be lowercase if it has no uppercase letters.
        for (int i = 0; i < value.Length; i++) {
            if (char.IsUpper(value[i])) {
                return false;
            }
        }
        return true;
    }
}