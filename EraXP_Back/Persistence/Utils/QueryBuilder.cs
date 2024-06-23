using System.Text;
using EraXP_Back.Persistence.Model;
using Microsoft.Extensions.Primitives;

namespace EraXP_Back.Persistence.Utils;

public class QueryBuilder
{
    private StringBuilder _stringBuilder;
    private List<DbParam> _dbParams;
    private string? _operator = null;
    private char _paramChar;
    
    public QueryBuilder(char paramChar)
    {
        _stringBuilder = new StringBuilder();
        _dbParams = new List<DbParam>();
        _paramChar = paramChar;
    }

    public IReadOnlyList<DbParam> DbParams => _dbParams;

    public void Add(string fieldName, string paramName, object value, string oper = "=")
    {
        if (_stringBuilder.Length > 0 && _operator == null)
        {
            throw new NullReferenceException("You need to specify an operator");
        }

        if (_stringBuilder.Length > 0)
        {
            _stringBuilder
                .Append(' ')
                .Append(_operator)
                .Append(' ');
        } 
        
        _dbParams.Add((paramName, value));
        _stringBuilder.Append($"{fieldName}{oper}{_paramChar}{paramName}");
    }

    public void And()
    {
        _operator = "AND";
    }

    public override string ToString()
    {
        string result = _stringBuilder.ToString();

        if (string.IsNullOrWhiteSpace(result))
        {
            return "";
        }
        return $"WHERE {result}";
    }
}