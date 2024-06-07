using System;
using NUnit.Framework;
using BaeknothingLib.MathUtility;

namespace BaeknothingLib.MathUtility.Test;

public class Tests
{
    [Test]
    public void TestDoubleToIntAndBack()
    {
        // 테스트할 double 값 설정
        double[] testValues = { 123456789.123, 1.23456, 98765.4321, 1000000000.0 };

        foreach (var originalValue in testValues)
        {
            uint encodedValue = Converter.DoubleToInt(originalValue);
            double decodedValue = Converter.IntToDouble(encodedValue);

            Console.WriteLine($"Original: {originalValue}, Encoded: {encodedValue}, Decoded: {decodedValue}, Diff: {(originalValue - decodedValue) / originalValue}");
            // 원본 값과 복원된 값이 거의 같은지 확인 (5자리 정밀도 내에서)
            Assert.That((originalValue - decodedValue) / originalValue, Is.LessThan(0.001), $"Failed for value {originalValue}");
        }
    }

    [Test]
    public void TestDoubleToIntAndBack_BetweenZeroAndOne()
    {
        // 0과 1 사이의 double 값으로 테스트
        double[] testValues = { 0.123456789, 0.000000001, 0.999999999 };

        foreach (var originalValue in testValues)
        {
            uint encodedValue = Converter.DoubleToInt(originalValue);
            double decodedValue = Converter.IntToDouble(encodedValue);

            Console.WriteLine($"Original: {originalValue}, Encoded: {encodedValue}, Decoded: {decodedValue}");

            Assert.That(encodedValue, Is.EqualTo(1000000000), "Encoded value should be 1000000000");
            Assert.That(decodedValue, Is.EqualTo(0), "Decoded value should be 0");
        }
    }


    [Test]
    public void TestDoubleToIntAndBack_Random()
    {
        // 랜덤한 double 값으로 테스트
        Random random = new Random();
        for (int i = 0; i < 100; i++)
        {
            double originalValue = random.NextDouble() * Math.Pow(10, random.Next(1, 308));
            uint encodedValue = Converter.DoubleToInt(originalValue);
            double decodedValue = Converter.IntToDouble(encodedValue);

            Console.WriteLine($"Original: {originalValue}, Encoded: {encodedValue}, Decoded: {decodedValue}");
            // 원본 값과 복원된 값이 거의 같은지 확인 (5자리 정밀도 내에서)
            Assert.That((originalValue - decodedValue) / originalValue, Is.LessThan(0.001), $"Failed for value {originalValue}");
        }
    }

    [Test]
    public void TestInvalidDoubleToInt()
    {
        // 음수 값 테스트
        Assert.Throws<ArgumentException>(() => Converter.DoubleToInt(-123.456));
    }

    [Test]
    public void TestInvalidIntToDouble()
    {
        // 유효하지 않은 uint 값 테스트
        Assert.Throws<ArgumentException>(() => Converter.IntToDouble(999999999));
    }
}
