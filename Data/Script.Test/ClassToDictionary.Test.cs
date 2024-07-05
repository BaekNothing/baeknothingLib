using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BaeknothingLib.Data.Test;

public class ClassToDictionaryTest
{
    [TestFixture]
    public class ConverterTest
    {
        public class TestClass
        {
            [Converter.ConvertTarget]
            public int IntField = 42;

            [Converter.ConvertTarget]
            public string StringField = "Hello";

            [Converter.ConvertTarget]
            public TestEnum EnumField = TestEnum.ValueOne;

            [Converter.ConvertTarget]
            public NestedTestClass NestedField = new();

            public int NonConvertedField = 100; // This field should not be converted
        }

        public class NestedTestClass
        {
            [Converter.ConvertTarget]
            public double DoubleField = 3.14;
        }

        public enum TestEnum
        {
            ValueOne,
            ValueTwo
        }

        [Test]
        public void NullTest()
        {
            Assert.IsTrue(true);
        }


        [Test]
        public void ClassToDictionary_WithSimpleFields_ReturnsCorrectDictionary()
        {
            var testClass = new TestClass();
            var result = Converter.ClassToDictionary(testClass);

            Assert.NotNull(result);
            Assert.AreEqual(42, result["IntField"]);
            Assert.AreEqual("Hello", result["StringField"]);
            Assert.AreEqual("ValueOne", result["EnumField"]);
        }

        [Test]
        public void ClassToDictionary_WithNestedFields_ReturnsCorrectDictionary()
        {
            var testClass = new TestClass();
            var result = Converter.ClassToDictionary(testClass);

            Assert.NotNull(result);
            Assert.IsInstanceOf<Dictionary<string, object>>(result["NestedField"]);

            var nestedResult = (Dictionary<string, object>)result["NestedField"];
            Assert.AreEqual(3.14, nestedResult["DoubleField"]);
        }

        [Test]
        public void ClassToDictionary_WithoutConvertTargetAttribute_ExcludesField()
        {
            var testClass = new TestClass();
            var result = Converter.ClassToDictionary(testClass);

            Assert.NotNull(result);
            Assert.IsFalse(result.ContainsKey("NonConvertedField"));
        }

        [Test]
        public void ClassToDictionary_WithNullObject_ReturnsNull()
        {
            Assert.Throws<System.ArgumentNullException>(() =>
            {
                Converter.ClassToDictionary(null);
            });
        }
    }
}
