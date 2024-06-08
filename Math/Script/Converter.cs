namespace BaeknothingLib.MathUtility;
public static class Converter
{
    private const uint FirstDigit = 1000000000;
    private const uint SecondDigit = 100000;

    /// <summary>
    /// uint는 10자리까지 표현할 수 있다. <br/>
    /// double은 1.7E+308 ~ 1.7E-308 사이의 값을 가진다. <br/>
    /// 10자리를 다음과 같이 쪼개어 표현한다. # #### ###### <br/>
    /// 첫번째 #은 이후 9자리 숫자의 정보를 온전히 보존하기 위해 언제나 1이다 <br/>
    /// 2 ~ 5번째 #은 double의 자릿수를 표현한다. <br/>
    /// 6 ~ 10번째 #은 double의 가장 앞자리 숫자 5개를 표현한다. <br/>
    /// </summary>
    /// <param name="value">양의 정수인 double</param>
    /// <returns>1 미만 = Error, 1 이상 = 10자리 정수</returns>
    public static uint DoubleToInt(double value)
    {
        if (value < 1)
            throw new ArgumentException("value is not a positive number");

        // 자릿수 표현
        int exponent = (int)Math.Log10(value);
        uint exponentPart = (uint)exponent;

        // 가장 앞자리 숫자 5개 추출
        double normalizedValue = value / Math.Pow(10, exponent);
        uint mantissaPart = (uint)(normalizedValue * (SecondDigit / 10)); // 5자리로 표현

        // 각 부분 결합
        return FirstDigit + (exponentPart + 1) * SecondDigit + mantissaPart;
    }

    /// <summary>
    /// DoubleToInt형식의 int를 double로 변환한다. <br/>
    /// </summary>
    /// <param name="value">DoubleToInt형식의 int</param>
    /// <returns>복원된 double 값</returns>
    public static double IntToDouble(uint value)
    {
        if (value < FirstDigit)
            throw new ArgumentException("value is not DoubleToInt type");

        // 지수 및 가수 추출
        uint exponentPart = (value % FirstDigit) / SecondDigit;
        uint mantissaPart = value % SecondDigit;

        // 지수와 가수를 사용하여 원래의 double 값 복원
        double mantissa = mantissaPart / (double)(SecondDigit / 10);
        return mantissa * Math.Pow(10, exponentPart > 0 ? exponentPart - 1 : 0);
    }
}
