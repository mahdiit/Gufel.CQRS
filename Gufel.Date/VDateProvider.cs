using Gufel.Date.Base;

namespace Gufel.Date
{
    public class VDateProvider : IVDateProvider
    {
        public VDate Now => VDate.Now;
        public VDate Today => VDate.Today;
    }
}
