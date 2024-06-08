namespace BaeknothingLib.Relation.Test;

[TestFixture]
public class RelationTests
{
    [Test]
    public void TestRelationInitialization()
    {
        var personality1 = IPersonality.Create();
        var personality2 = IPersonality.Create();
        var relation = IRelationInfo.Creation(personality1, personality2);

        Assert.AreEqual(personality2, relation.Dest);
        Assert.IsNotNull(relation.Compatibility);
    }
}
