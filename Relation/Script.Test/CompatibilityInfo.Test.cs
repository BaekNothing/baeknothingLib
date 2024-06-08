namespace BaeknothingLib.Relation.Test;

[TestFixture]
public class CompatibilityInfoTests
{
    [Test]
    public void TestCompatibilityInfoInitialization()
    {
        var personality1 = IPersonality.Create();
        var personality2 = IPersonality.Create();
        var compatibilityInfo = new CompatibilityInfo(personality1, personality2);

        int enumLength = Enum.GetValues(typeof(Consts.PersonalityKeys)).Length;

        Assert.IsNotNull(compatibilityInfo.SharedKeys);
        Assert.AreEqual(enumLength / 2, compatibilityInfo.SharedKeys.Count);
    }


    [Test]
    public void TestSetSharedKeys()
    {
        var personality1 = IPersonality.Create();
        var personality2 = IPersonality.Create();

        personality1.SetStatus(Consts.PersonalityKeys.Cold, 10);
        personality2.SetStatus(Consts.PersonalityKeys.Cold, 10);

        MethodInfo method = typeof(CompatibilityInfo).GetMethod("SetSharedKeys", BindingFlags.NonPublic | BindingFlags.Static);

        var sharedKeys = (List<Consts.PersonalityKeys>)method.Invoke(null, new object[] { personality1, personality2 });


        int enumLength = Enum.GetValues(typeof(Consts.PersonalityKeys)).Length;

        Assert.AreEqual(enumLength / 2, sharedKeys.Count);
        Assert.Contains(Consts.PersonalityKeys.Hot, sharedKeys);
    }

    [Test]
    public void TestSetTension()
    {
        var personality1 = IPersonality.Create();
        var personality2 = IPersonality.Create();

        personality1.SetStatus(Consts.PersonalityKeys.Hot, 10);
        personality2.SetStatus(Consts.PersonalityKeys.Hot, 10);
        var sharedKeys = new List<Consts.PersonalityKeys> { Consts.PersonalityKeys.Hot };

        MethodInfo method = typeof(CompatibilityInfo).GetMethod("SetTension", BindingFlags.NonPublic | BindingFlags.Static);

        int tension = (int)method.Invoke(null, new object[] { personality1, personality2, sharedKeys });

        Assert.AreEqual(10, tension);
    }
}
