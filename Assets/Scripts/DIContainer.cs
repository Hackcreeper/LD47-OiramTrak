using System.Collections.Generic;

public class DiContainer
{
    public static DiContainer Instance => DiContainer._instance != null ? DiContainer._instance : new DiContainer();

    private static DiContainer _instance;

    private readonly Dictionary<string, object> _registeredByName = new Dictionary<string, object>();

    public DiContainer()
    {
        _instance = this;
    }
    
    public void Register<T>(string name, T obj)
    {
        if (_registeredByName.ContainsKey(name))
        {
            _registeredByName[name] = obj;
            return;
        }
        
        _registeredByName.Add(name, obj);
    }

    public T GetByName<T>(string name)
    {
        if (!_registeredByName.ContainsKey(name))
        {
            return default;
        }
        
        return (T) _registeredByName[name];
    }
}
