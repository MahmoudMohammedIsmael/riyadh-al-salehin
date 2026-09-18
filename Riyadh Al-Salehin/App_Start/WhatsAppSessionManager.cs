
using System.Collections.Concurrent;

namespace Riyadh_Al_Salehin.App_Start
{
    public static class WhatsAppSessionManager
    {
        private static readonly ConcurrentDictionary<
            string,
            WhatsAppBookingSession> Sessions =
            new ConcurrentDictionary<
                string,
                WhatsAppBookingSession>();


        public static WhatsAppBookingSession Get(string phone)
        {
            return Sessions.GetOrAdd(
                phone,
                p => new WhatsAppBookingSession
                {
                    Phone = p,
                    Step = 0
                });
        }


        public static void Remove(string phone)
        {
            WhatsAppBookingSession session;

            Sessions.TryRemove(
                phone,
                out session);
        }
    }
}

