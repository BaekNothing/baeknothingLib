namespace BaeknothingLib.Relation;

public interface IPersonality : ICloneable
{
    public static IPersonality Create() => new Personality();

    public long GetCode { get; }
    public IReadOnlyDictionary<Consts.PersonalityKeys, int> Status { get; }
    public IReadOnlyDictionary<long, IRelationInfo> Relation { get; }

    public void SetStatus(Consts.PersonalityKeys key, int value);

    public void Connect(IPersonality personality);
    public bool IsConnectable(IPersonality personality);
    public Dictionary<object, object> ToMap();
    public void FromMap(Dictionary<object, object> map);
}

class Personality : IPersonality
{
    readonly Dictionary<Consts.PersonalityKeys, int> _status;
    readonly long _creationTime;
    readonly Dictionary<long, IRelationInfo> _relation;

    public long GetCode => _creationTime;
    public IReadOnlyDictionary<Consts.PersonalityKeys, int> Status => _status;
    public IReadOnlyDictionary<long, IRelationInfo> Relation => _relation;

    public void SetStatus(Consts.PersonalityKeys key, int value) => _status[key] = value;
    public void Connect(IPersonality personality)
    {
        if (IsConnectable(personality))
            SetRelation(personality);
    }

    public bool IsConnectable(IPersonality personality)
    {
        if (personality == null)
            return false;
        else if (_relation.ContainsKey(personality.GetCode))
            return false;
        else if (personality == this)
            return false;
        return true;
    }

    void SetRelation(IPersonality personality)
    {
        _relation[personality.GetCode] = IRelationInfo.Creation(this, personality);
    }

    public object Clone()
    {
        var clone = new Personality();
        clone.FromMap(ToMap());
        return clone;
    }

    static readonly Mutex _mutex = new();
    public Personality()
    {
        try
        {
            _mutex.WaitOne();
            _relation = [];
            _status = [];
            _creationTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            // 기기의 LocalTime을 바꾸는 것에 취약함 = 개선 필요.

            foreach (Consts.PersonalityKeys key in Enum.GetValues(typeof(Consts.PersonalityKeys)))
            {
                _status[key] = 0;
            }
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    public void FromMap(Dictionary<object, object> map)
    {
        foreach (var key in map.Keys)
        {
            if (Enum.TryParse<Consts.PersonalityKeys>(key.ToString(), out var _statusKey))
            {
                _status[_statusKey] = (int)map[key];
            }
        }
    }

    public Dictionary<object, object> ToMap()
    {
        var map = new Dictionary<object, object>();
        foreach (var key in _status.Keys)
        {
            map[key.ToString()] = _status[key];
        }
        return map;
    }
}
