namespace BaeknothingLib.Relation;

public interface IRelationInfo
{
    public static IRelationInfo Creation(IPersonality target, IPersonality dest)
        => new RelationInfo(target, dest);
    public IPersonality Dest { get; }
    public ICompatibilityInfo Compatibility { get; }
}

class RelationInfo(IPersonality target, IPersonality dest) : IRelationInfo
{
    public IPersonality Dest => dest;
    public ICompatibilityInfo Compatibility { get; private set; } = new CompatibilityInfo(target, dest);
}
