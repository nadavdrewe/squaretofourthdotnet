using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;

namespace Revel._808nd.com.Classes
{
    public static class RevelHelper
    {

        public static DateTime ConvertFromDateTimeOffset(DateTimeOffset dateTime)
        {
            if (dateTime.Offset.Equals(TimeSpan.Zero))
                return dateTime.UtcDateTime;
            else if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
                return DateTime.SpecifyKind(dateTime.DateTime, DateTimeKind.Local);
            else
                return dateTime.DateTime;
        }

        /// <summary>
        /// Pass in a URI as string, returns us a proper primary key as integer
        /// </summary>
        /// <param name="theKeyURI"></param>
        /// <returns></returns>
        /// 
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

      

        public static bool IsDateTimeCurrentlyWithinOpeningHours()
        {
            var startHour = 2;
            var endHour = 6;

            DateTime now = DateTime.Now;
            DateTime today = now.Date;
            DateTime start = today.AddHours(startHour);
            DateTime end = today.AddHours(endHour);

            bool invertResult = end < start;


            bool inRange = (start <= now && now <= end) ^ invertResult;
            if (!inRange)
            {

                return true;
            }

            return false;
        }

        public static
            bool IsTimeXMinsLaterThanComparisonTime(DateTime Time, DateTime ComparisonTime, int minsInterval)
        {
            try
            {
                DateTime timeNotToBeLaterThan = ComparisonTime.AddMinutes(minsInterval);

                if(Time > timeNotToBeLaterThan)
                { return false; }
                
                return true;

            }
            catch (Exception)
            {
                
                throw;
            }
          
        }


        public static int ConvertEstablishmentWithHyphenToPrimaryKey(string theKeyURI)
        {
            try
            {
                if (theKeyURI != null && theKeyURI != "")
                {

                    int theKeyAsInt = 0;

                    var keyArray = theKeyURI.Split(':');
                    var count = keyArray.Count();

                    theKeyAsInt = Convert.ToInt32(keyArray[count -1]);

                    return theKeyAsInt;

                }

                return 0;
            }
            catch (Exception ex)
            {

                //"There was a problem converting the primary key of this object from a string to an int; ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey method";
                throw ex;
            }
        }

        public static int ConvertParentOrderIDToIntegerPrimaryKey(string theKeyURI)
        {
            try
            {
                if (theKeyURI != null && theKeyURI != "")
                {

                    int theKeyAsInt = 0;

                    var keyArray = theKeyURI.Split(':');
                    var SecondArray = keyArray[0].Split(' ');


                    theKeyAsInt = Convert.ToInt32(SecondArray[1]);

                    return theKeyAsInt;

                }

                return 0;
            }
            catch (Exception ex)
            {

                //"There was a problem converting the primary key of this object from a string to an int; ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey method";
                throw ex;
            }
        }

        public static int ConvertSpecialistIDWithSpacesToID(string theKeyURI)
        {
            try
            {
                if (theKeyURI != null && theKeyURI != "")
                {

                    int theKeyAsInt = 0;

                    var keyArray = theKeyURI.Split(' ');

                    theKeyAsInt = Convert.ToInt32(keyArray[1]);

                    return theKeyAsInt;

                }

                return 0;
            }
            catch (Exception ex)
            {

                //"There was a problem converting the primary key of this object from a string to an int; ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey method";
                throw ex;
            }
        }




        public static int ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(string theKeyURI)
        {
            try
            {
                if (theKeyURI != null && theKeyURI != "")
                {

                    int theKeyAsInt = 0;

                    var keyArray = theKeyURI.Split('/');

                    int positionToAccess = keyArray.GetLength(0) - 2;

                    theKeyAsInt = Convert.ToInt32(keyArray[positionToAccess]);

                    return theKeyAsInt;

                }

                return 0;
            }
            catch (Exception ex)
            {

                //"There was a problem converting the primary key of this object from a string to an int; ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey method";
                throw ex;
            }
        }


        public static int ConvertJSONEstablishmentIDFromURIToIntegerPrimaryKey(string theKeyURI)
        {
            try
            {
                if (theKeyURI != null && theKeyURI != "")
                {

                    int theKeyAsInt = 0;

                    var keyArray = theKeyURI.Split(':');                   

                 //   int positionToAccess = keyArray.GetLength(0) - 2;

                    theKeyAsInt = Convert.ToInt32(keyArray[1]);

                    return theKeyAsInt;

                }

                return 0;
            }
            catch (Exception ex)
            {

                //"There was a problem converting the primary key of this object from a string to an int; ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey method";
                throw ex;
            }
        }


        public static string CheckIfJSONZeroAndReturnZeroDecimalString(string JSON)
        {
            if (JSON == "0")
            {
                return "0.00";
            }
            else return JSON;
        }


        //wraps ReturnYesterdayIfDateTimeNowBetween12am_3am
        public static DateTime WrapAllRevelStartingDatesInThisMethod(DateTime theDate)
        {
            return ReturnYesterdayIfDateTimeNowBetween12am_3am(theDate);
        }


        /// <summary>
        /// Should be obvious from the name
        /// </summary>
        /// <param name="theDate"></param>
        /// <returns></returns>
        public static DateTime ReturnYesterdayIfDateTimeNowBetween12am_3am(DateTime theDate)
        {
            var test = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss" ));
            //var test = new DateTime(2014, 08, 25, 02, 00, 00);


            var earliestMarker = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
            var latestMarker = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 03:00:00");


            if (test >= earliestMarker && test < latestMarker)
            {
                return theDate.AddDays(-1);
            }

            else return theDate;

        }


        public class DbHelper
        {
            public bool DeleteDuplicateOrdersFromDB()
            {
                try
                {

                    using (SqlConnection connection = new SqlConnection())
                    {

                        var connString = ConfigurationManager.ConnectionStrings["GrindContext"].ConnectionString;
                        connection.ConnectionString = connString;
                        
                        SqlCommand command = new SqlCommand("sp_RemoveDuplicateOrders", connection);                                                
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        var returnedRows = command.ExecuteReader();
                        
                        return true;

                    }

                }
                catch (Exception ex)
                {
                        
                    throw ex;
                }


            }


            public bool DeleteDuplicateOrderItemsFromDB()
            {
                try
                {

                    using (SqlConnection connection = new SqlConnection())
                    {

                        var connString = ConfigurationManager.ConnectionStrings["GrindContext"].ConnectionString;
                        connection.ConnectionString = connString;

                        SqlCommand command = new SqlCommand("sp_RemoveDuplicateOrderItems", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        var returnedRows = command.ExecuteReader();

                        return true;

                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }


            }



        }




    }
}
