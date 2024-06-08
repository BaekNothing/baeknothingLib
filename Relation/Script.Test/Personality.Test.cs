namespace BaeknothingLib.Relation.Test;

[TestFixture]
public class PersonalityTests
{
    [Test]
    public void TestPersonalityInitialization()
    {
        IPersonality personality = IPersonality.Create();
        int enumLength = Enum.GetValues(typeof(Consts.PersonalityKeys)).Length;

        Assert.IsNotNull(personality);
        Assert.AreEqual(enumLength, personality.Status.Count);
        Assert.AreEqual(0, personality.Status[Consts.PersonalityKeys.Hot]);
        Assert.AreEqual(0, personality.Status[Consts.PersonalityKeys.Cold]);
    }

    [Test]
    public void TestPersonalityClone()
    {
        IPersonality personality = IPersonality.Create();
        IPersonality clone = (IPersonality)personality.Clone();

        Assert.AreNotSame(personality, clone);
        Assert.AreNotEqual(personality.GetCode, clone.GetCode); // Clone을 하더라도 Code를 새로 생성하므로 달라야 함.
        Assert.AreEqual(personality.Status.Count, clone.Status.Count);
    }

    [Test]
    public void TestPersonalityConnection()
    {
        IPersonality personality1 = IPersonality.Create();
        IPersonality personality2 = IPersonality.Create();

        personality1.Connect(personality2);

        Assert.IsTrue(personality1.Relation.ContainsKey(personality2.GetCode));
    }

    [Test]
    public void TestCheckConnectable()
    {
        IPersonality personality1 = IPersonality.Create();
        IPersonality personality2 = IPersonality.Create();

        Assert.IsTrue(personality1.IsConnectable(personality2));
        Assert.IsFalse(personality1.IsConnectable(null));
        Assert.IsFalse(personality1.IsConnectable(personality1));

        personality1.Connect(personality2);
        Assert.IsFalse(personality1.IsConnectable(personality2));
    }

    [Test]
    public void TestToMapAndFromMap()
    {
        IPersonality personality = IPersonality.Create();
        var map = personality.ToMap();
        IPersonality newPersonality = IPersonality.Create();
        newPersonality.FromMap(map);

        Assert.AreEqual(personality.Status.Count, newPersonality.Status.Count);
        foreach (var key in personality.Status.Keys)
        {
            Assert.AreEqual(personality.Status[key], newPersonality.Status[key]);
        }
    }

    [Test]
    public void TestSetStatus()
    {
        IPersonality personality = IPersonality.Create();
        personality.SetStatus(Consts.PersonalityKeys.Hot, 10);
        personality.SetStatus(Consts.PersonalityKeys.Cold, 20);

        Assert.AreEqual(10, personality.Status[Consts.PersonalityKeys.Hot]);
        Assert.AreEqual(20, personality.Status[Consts.PersonalityKeys.Cold]);
    }
}
