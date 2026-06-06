using Gufel.Date.Base;

namespace Gufel.Date
{
    public sealed class VDateProvider : IVDateProvider
    {
        public VDate Now => VDate.Now;
        public VDate Today => VDate.Today;
    }
}
