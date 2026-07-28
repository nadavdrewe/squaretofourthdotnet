using System;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class CashupNotifier : INotifier
    {

        [Key]
        public int Id { get; set; }
        public string NotificationAddress { get; set; }
        public int DBKEY_establishment_id { get; set; }
        public virtual Establishment Establishment { get; set; }
        public bool Enabled { get; set; }
        public bool UniversalContact { get; set; }


        public NotificationResultContext Notify()
        {
            try
            {
                //do some notifying


                return new NotificationResultContext
                {
                    Result = NotificationResult.OK,
                    Message = "Notified!!!!"

                };

            }
            catch (Exception ex)
            {

                return new NotificationResultContext
                {
                    Message = ex.InnerException.ToString(),
                    Result = NotificationResult.EXCEPTION
                };

            }

        }
    }

    public interface INotifier
    {
        NotificationResultContext Notify();
    }

    public class NotificationResultContext
    {
        public NotificationResult Result { get; set; }
        public string Message { get; set; }
    }

    public enum NotificationResult
    {
        OK = 0, FAIL = 1, EXCEPTION = 3
    }

}



