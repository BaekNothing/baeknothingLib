using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace BaeknothingLib.Data.Test;

public class UnitTests
{
    class TestComponent : DataComponentBase
    {
        public static string Key => "TestComponent";
        public int Value { get; private set; } = 0;
        public void SetValue(int value) => this.Value = value;

        public override string GetKey()
        {
            return Key;
        }

        public override Dictionary<string, object> ToMap()
        {
            return new Dictionary<string, object> { { nameof(Value), Value } };
        }

        protected override void FromMapInternal(Dictionary<object, object> map)
        {
            if (map.TryGetValue(nameof(Value), out var value))
            {
                Value = (int)value;
            }
        }

        public override string Print()
        {
            throw new System.NotImplementedException();
        }
    }


    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void Test_DataObject_Initialization()
    {
        var components = new List<DataComponentBase> { new TestComponent() };
        var DataObject = new DataObject(components);
        Assert.Multiple(() =>
        {
            Assert.That(DataObject.IsInitialized, Is.True);
            Assert.That(DataObject.GetComponent<TestComponent>(), Is.Not.Null);
            Assert.That(DataObject.ToMap(), Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void Test_DataObject_Component()
    {
        var components = new List<DataComponentBase> { new TestComponent() };
        var DataObject = new DataObject(components);
        var testComponent = DataObject.GetComponent<TestComponent>();

        Assert.Multiple(() =>
        {
            Assert.That(DataObject.IsInitialized, Is.True);

            Assert.That(testComponent, Is.Not.Null);
            Assert.That(testComponent?.GetKey(), Is.EqualTo(TestComponent.Key));
            Assert.That(testComponent?.ToMap(), Has.Count.EqualTo(1).And.Contains(new KeyValuePair<string, object>(nameof(TestComponent.Value), 0)));
            testComponent?.SetValue(10);
            Assert.That(testComponent?.ToMap(), Has.Count.EqualTo(1).And.Contains(new KeyValuePair<string, object>(nameof(TestComponent.Value), 10)));

            DataObject.FromMap(new Dictionary<object, object> { { TestComponent.Key, new Dictionary<object, object> { { nameof(TestComponent.Value), 20 } } } });
            Assert.That(testComponent?.Value, Is.EqualTo(20));
        });
    }
}
