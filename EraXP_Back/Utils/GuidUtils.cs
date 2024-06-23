namespace EraXP_Back.Utils;

public static class GuidUtils
{
    public static bool IsDefault<T>(this T? guid)
        where T : struct, IEquatable<T>
    {
        return guid == null || EqualityComparer<T>.Default.Equals(guid.Value, default);
    }
    
    public static bool IsDefault<T>(this T guid)
        where T : struct, IEquatable<T>
    {
        return EqualityComparer<T>.Default.Equals(guid, default);
    }
}