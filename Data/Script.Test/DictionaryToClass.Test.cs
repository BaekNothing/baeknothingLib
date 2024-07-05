using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using BaeknothingLib.Data;

namespace BaeknothingLib.Data.Test;

public class DictionaryToClassTest
{
    public class TestClass
    {
        public int IntField = 0;
        public string StringField = "";
        public TestEnum EnumField = TestEnum.ValueOne;
        public NestedClass NestedField = new NestedClass();
    }

    public class NestedClass
    {
        public double DoubleField = 0;
    }

    public enum TestEnum
    {
        ValueOne,
        ValueTwo
    }

    [TestFixture]
    public class DictionaryToClassTests
    {
        [Test]
        public void DictionaryToClass_WithNullDictionary_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Converter.DictionaryToClass<TestClass>(null));
        }

        [Test]
        public void DictionaryToClass_WithValidData_ReturnsCorrectInstance()
        {
            var dict = new Dictionary<string, object>
            {
                { "IntField", 42 },
                { "StringField", "Hello" },
                { "EnumField", "ValueOne" },
                { "NestedField", new Dictionary<string, object> { { "DoubleField", 3.14 } } }
            };

            var result = Converter.DictionaryToClass<TestClass>(dict);

            Assert.NotNull(result);
            Assert.AreEqual(42, result.IntField);
            Assert.AreEqual("Hello", result.StringField);
            Assert.AreEqual(TestEnum.ValueOne, result.EnumField);
            Assert.NotNull(result.NestedField);
            Assert.AreEqual(3.14, result.NestedField.DoubleField);
        }

        [Test]
        public void DictionaryToClass_WithInvalidField()
        {
            var dict = new Dictionary<string, object>
            {
                { "InvalidField", 123 }
            };

            var result = Converter.DictionaryToClass<TestClass>(dict);

            Assert.NotNull(result);
            Assert.AreEqual(0, result.IntField);
            Assert.AreEqual("", result.StringField);
            Assert.AreEqual(TestEnum.ValueOne, result.EnumField);
            Assert.NotNull(result.NestedField);
            Assert.AreEqual(0, result.NestedField.DoubleField);
        }
    }
}
