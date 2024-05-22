namespace EraXP_Back.Persistence.Model;

public record DbParam(
    string Name,
    object Value
)
{
    public static implicit operator DbParam(ValueTuple<string, object> data) => new DbParam(data.Item1, data.Item2);
}