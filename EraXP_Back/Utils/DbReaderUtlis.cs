using System.Data.Common;

namespace EraXP_Back.Utils;

public static class DbReaderUtlis
{
    public static async Task<T?> GetOrDefault<T>(this DbDataReader reader, int ordinal)
    {
        if (await reader.IsDBNullAsync(ordinal))
            return default;

        return reader.GetFieldValue<T>(ordinal);
    }
}