namespace BaeknothingLib.Relation;

public interface ICompatibilityInfo
{
    public IReadOnlyCollection<Consts.PersonalityKeys> SharedKeys { get; }
    public int Tension { get; }
}

public class CompatibilityInfo : ICompatibilityInfo
{
    public IReadOnlyCollection<Consts.PersonalityKeys> SharedKeys => _sharedKeys.AsReadOnly();
    public int Tension { get; private set; }

    private readonly List<Consts.PersonalityKeys> _sharedKeys;

    public CompatibilityInfo(IPersonality target, IPersonality dest)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(dest);

        _sharedKeys = SetSharedKeys(target, dest);
        Tension = SetTension(target, dest, _sharedKeys);
    }

    static List<Consts.PersonalityKeys> SetSharedKeys(IPersonality target, IPersonality dest)
    {
        List<Consts.PersonalityKeys> result = [];

        int length = Enum.GetValues(typeof(Consts.PersonalityKeys)).Length / 2;

        var TargetHeigestThree = target.Status.OrderByDescending(x => x.Value).Take(length).Select(x => x.Key);
        var DestHeigestThree = dest.Status.OrderByDescending(x => x.Value).Take(length).Select(x => x.Key);

        result.AddRange(TargetHeigestThree.Where(key => DestHeigestThree.Contains(key)));
        return result;
    }

    static int SetTension(IPersonality target, IPersonality dest, List<Consts.PersonalityKeys> sharedKeys)
    {
        int statusSum = target.Status.Sum(x => x.Value) + dest.Status.Sum(x => x.Value);
        int sharedKeySum = sharedKeys.Sum(key => target.Status[key] + dest.Status[key]);
        return sharedKeySum - statusSum / 2;
    }
}
