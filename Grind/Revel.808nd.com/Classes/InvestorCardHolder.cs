namespace Revel._808nd.com.Classes
{
    public class InvestorCardHolder
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Amount { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public decimal? InitialLoad { get; set; }
        public decimal? MonthlyLoad { get; set; }
        public decimal? WeeklyLoad { get; set; }
        public int CurrentInitalBucket { get; set; }
        public int CurrentPeriodBucket { get; set; }
        public bool HasBeenAdded { get; set; }
    }
}
