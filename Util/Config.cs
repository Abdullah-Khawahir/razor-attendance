public class Config
{

    public static T? Env<T>(string key, T? fallback = default) where T : IParsable<T>, new()
    {
        string? value = Environment.GetEnvironmentVariable(key);

        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine($"[WARN] Environment variable '{key}' not found. Using fallback: {fallback}");
            return fallback;
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)value;
        }

        if (T.TryParse(value, null, out var parsed))
        {
            return parsed;
        }
        Console.WriteLine($"[WARN] Failed to parse environment variable '{key}' with value '{value}'. Using fallback: {fallback}");
        return fallback;
    }
}

