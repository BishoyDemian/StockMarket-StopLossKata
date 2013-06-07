using System;
using System.Threading.Tasks;

namespace StockMarket.Trader.Time
{
    public class TimeProvider : ITimeProvider
    {
        public void After(int seconds, Action<short> action, short param)
        {
            if (action == null)
                throw new ArgumentNullException();

            Task.Run(() =>
                     {
                         Task.Delay(TimeSpan.FromSeconds(seconds))
                             .Wait();
                         
                         action(param);
                     });
        }
    }
}