namespace BaeknothingLib.MathUtility;
public static class Converter
{
    const uint _firstDigit = 1000000000;
    const uint _secondDigit = 100000;

    /// <summary>
    /// uint는 10자리까지 표현할 수 있다. <br/>
    /// double은 1.7E+308 ~ 1.7E-308 사이의 값을 가진다. <br/>
    /// 10자리를 다음과 같이 쪼개어 표현한다. # #### ###### <br/>
    /// 첫번째 #은 이후 9자리 숫자의 정보를 온전히 보존하기 위해 언제나 1이다 <br/>
    /// 2 ~ 5번째 #은 double의 자릿수를 표현한다. <br/>
    /// 6 ~ 10번째 #은 double의 가장 앞자리 숫자 5개를 표현한다. <br/>
    /// </summary>
    /// <param name="value"> 양의 정수인 double </param>
    /// <returns> 0 미만 = Error, 0 이상 1 미만 = 1000000000, 1이상 = 10자리 정수 </returns>
    public static uint DoubleToInt(double value)
    {
        if (value < 0)
            throw new System.ArgumentException("value is not positive number");
        else if (value < 1)
            return _firstDigit;

        // 자릿수 표현
        int exponent = (int)Math.Log10(Math.Abs(value));
        uint exponentPart = (uint)(exponent < 0 ? 0 : exponent);

        // 가장 앞자리 숫자 5개 추출
        double normalizedValue = value / Math.Pow(10, exponentPart);
        uint mantissaPart = (uint)(Math.Abs(normalizedValue) * (_secondDigit / 10)); // 5자리로 표현

        // 각 부분 결합
        uint result = _firstDigit + (exponentPart + 1) * _secondDigit + mantissaPart;
        return result;
    }

    /// <summary>
    /// DoubleToInt형식의 int를 double로 변환한다. <br/>
    /// </summary>
    /// <param name="value"> DoubleToInt형식의 int </param>
    /// <returns></returns>
    public static double IntToDouble(uint value)
    {
        if (value < _firstDigit)
            throw new ArgumentException("value is not DoubleToInt type");
        else if (value == _firstDigit)
            return 0;

        // 첫 번째 자리 (항상 1)은 무시
        // 2 ~ 5번째 자리: 지수
        // 6 ~ 10번째 자리: 가수

        uint exponentPart = value % _firstDigit / _secondDigit;
        uint mantissaPart = value % _secondDigit;
        double mantissa = mantissaPart / (double)_secondDigit;
        double result = mantissa * Math.Pow(10, exponentPart);
        return result;
    }
}
