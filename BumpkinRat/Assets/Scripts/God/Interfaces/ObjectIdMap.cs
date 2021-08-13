using System.Collections.Generic;

public class IdToObjectMap<T>
{
    private Dictionary<int, T> Mapping { get; set; }

    public IdToObjectMap()
    {
        Mapping = new Dictionary<int, T>();
    }

    public IdToObjectMap(Dictionary<int, T> dict)
    {
        Mapping = new Dictionary<int, T>(dict);
    }

    public bool ExistsInMap(int i)
    {
        return Mapping.ContainsKey(i);
    }

    public T GetObjectFromMap(int key)
    {
        return Mapping[key];
    }

}