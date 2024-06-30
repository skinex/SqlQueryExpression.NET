using Newtonsoft.Json;

namespace Ap.Tools.TSqlService.Abstractions.Models;

public class TSqlTable : Dictionary<string, object>
{
    public static TSqlTable From(Dictionary<string, object> dictionary)
    {
        var sqlTable = new TSqlTable();
        
        foreach (var keyValuePair in dictionary)
        {
            sqlTable[keyValuePair.Key] = keyValuePair.Value;
        }

        return sqlTable;
    }
    
    public T GetColumnValue<T>(string columnName)
    {
        if (!ContainsKey(columnName))
        {
            return default;
        }

        var value = this[columnName];

        if (value is T requestedValue)
        {
            return requestedValue;
        }
            
        return ParseRequestedType<T>(value);
    }

    private static T ParseRequestedType<T>(object value)
    {
        var requestedType = typeof(T);
        if (!requestedType.IsValueType)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));
        }

        var underlyingType = Nullable.GetUnderlyingType(requestedType) ?? requestedType;

        if (value.GetType() == underlyingType)
        {
            return (T)value;
        }

        if (underlyingType == typeof(Guid) && 
            TryResolveGuid(value, out T columnValue))
        {
            return columnValue;
        }

        if (underlyingType == typeof(DateTime) && 
            TryResolveDateTime(value, out T parsedValue))
        {
            return parsedValue;
        }

        return (T)Convert.ChangeType(value, underlyingType);
    }

    private static bool TryResolveDateTime<T>(object value, out T columnValue)
    {
        columnValue = default;

        if (value is not string sValue || string.IsNullOrWhiteSpace(sValue))
        {
            return false;
        }

        if (!DateTime.TryParse(sValue, out var parsedDateTime))
        {
            return false;
        }

        object parsed = parsedDateTime;
        columnValue = (T)parsed;

        return false;
}

    private static bool TryResolveGuid<T>(object value, out T columnValue)
    {
        columnValue = default;

        if (value is not string sValue || string.IsNullOrWhiteSpace(sValue))
        {
            return false;
        }

        if (!Guid.TryParse(sValue, out var parsedGuid))
        {
            return false;
        }
        
        object parsed = parsedGuid;
        columnValue = (T)parsed;
        return true;
    }
}