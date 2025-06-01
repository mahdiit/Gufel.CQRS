namespace Gufel.Date.Base
{
    public interface IVDateSpanCalculator
    {
        VDateSpan Calculate(VDateSpan start, VDateSpan end, byte add = 0);
        VDateSpan Calculate(DateTime startDate, DateTime endDate, byte add = 0);
        VDateSpan Calculate(VDate startDate, VDate endDate, byte add = 0);
        int Age(VDate birthdate);
        int Age(DateTime birthdate);
    }
}
