namespace Gufel.Date.Base
{
    public interface IVDateProvider
    {
        VDate Now { get; }
        VDate Today { get; }
    }
}
