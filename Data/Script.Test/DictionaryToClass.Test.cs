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
        public int IntField;
        public string StringField;
        public TestEnum EnumField;
        public NestedClass NestedField;
    }

    public class NestedClass
    {
        public double DoubleField;
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
        public void DictionaryToClass_WithInvalidField_ThrowsInvalidOperationException()
        {
            var dict = new Dictionary<string, object>
            {
                { "InvalidField", 123 }
            };

            Assert.Throws<InvalidOperationException>(() => Converter.DictionaryToClass<TestClass>(dict));
        }
    }
}
