namespace EraXP_Back.Utils;

public record Result<T> (
    T? Entity, int? Code, string? Error)
{
    public bool IsSuccess => Entity != null;
    public static implicit operator Result<T>(T item) => new(item, null, null);
    public static implicit operator Result<T>(ValueTuple<int, string> item) => new(default, item.Item1, item.Item2);
}

public record Result (
    int? Code, string? Error)
{
    private static Result? _result;
    public bool IsSuccess => Error == null;
    public static implicit operator Result(ValueTuple<int, string> item) => new(item.Item1, item.Item2);
    public static Result Ok => _result ??= new Result(null, null);
}
