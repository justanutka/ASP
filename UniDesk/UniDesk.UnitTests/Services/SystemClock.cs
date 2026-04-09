namespace UniDesc.Web.Services
{
    public class SystemClock : ISystemClock
    {
        public DateTime Now => DateTime.Now;
    }
}