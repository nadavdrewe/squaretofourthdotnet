using domain.geckoboardv2.grind.com.Extensions;
using domain.geckoboardv2.grind.com.Models.BoardData;
using MongoDB.Driver;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Utility;
using Revel._808nd.com.Models;
using Revel._808nd.com.OperationsReport.Models;
using Revel._808nd.com.OperationsReport.Mongo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using xero.railgunit.com.Grind.Utility;
using domain.geckoboardv2.grind.com.Models.Discounts;
using GeckoboardLibrary.Classes.Widgets;
using Revel._808nd.com.ReportingModel;

namespace domain.geckoboardv2.grind.com.Services
{
    public class GeckoboardV2Service
    {
        HttpClient client;
        GrindContext db;
        BudgetContext budgetDb;
        MongoClient mongoClient;
        IMongoDatabase mongoDb;
        IMongoCollection<OpsReportHourlyWrapper> collection; //initial collection
        IList<PerSiteDiscount> discounts;
        //SALES REPORTS
        //Wrapper for todays data 
        //Wrapper for today / last week data

        //WRAPPER FOR ENTIRE WEEK UNTIL TODAY - DOES NOT INCLUDE TODAY
        public List<OpsReportHourlyWrapper> peristenceDataWrappersWeekToDateUntilToday = new List<OpsReportHourlyWrapper>();
        public List<OpsReportHourlyWrapper> peristenceDataWrappersLastWeekToDatePointComparisonPeriod = new List<OpsReportHourlyWrapper>();
        //WRAPPERS FOR TODAY + SAME PIT LAST WEEK
        public List<OpsReportHourlyWrapper> peristenceDataWrappersToday = new List<OpsReportHourlyWrapper>();
        public List<OpsReportHourlyWrapper> peristenceDataWrappersTodayLastWeek = new List<OpsReportHourlyWrapper>();

        public List<OpsReportHourlyWrapper> peristenceDataWrappersEntireWeekPlusToday = new List<OpsReportHourlyWrapper>();
        public List<OpsReportHourlyWrapper> peristenceDataWrappersEntireLastWeekPlusTodayLastWeek = new List<OpsReportHourlyWrapper>();

        public List<OpsReportHourlyWrapper> peristenceDataWrappersYesterday = new List<OpsReportHourlyWrapper>();

        //establishment
        IEnumerable<Establishment> Establishments;


        public List<Budget2019> BudgetsYesterday = new List<Budget2019>();
        public List<Budget2019> BudgetsTodayOnly = new List<Budget2019>();
        public List<Budget2019> BudgetsThisWeekNotIncludingToday = new List<Budget2019>();
        public List<Budget2019> BudgetsThisWTDIncludingToday = new List<Budget2019>();
        public List<Budget2019> BudgetsLastWTDIncludingLastWeekToday = new List<Budget2019>();



        public List<IndividualBoardDataset> individualBoardDatasets = new List<IndividualBoardDataset>();

        //setup
        public GeckoboardV2Service()
        {
            var RevelAPIKEY = ConfigurationManager.AppSettings["RevelAPIKEY"];
            var RevelBaseURL = ConfigurationManager.AppSettings["RevelBaseURL"];

            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelFactory helperFactory = new RevelFactory(revOrg);
            client = helperFactory.CreateShoreditchGrindHttpClient(RevelBaseURL, RevelAPIKEY);

            db = new GrindContext();
            budgetDb = new BudgetContext();

            var _connectionString = ConfigurationManager.ConnectionStrings["GrindMongoOps"].ToString();
            var _databaseName = MongoUrl.Create(_connectionString).DatabaseName;

            //use remote db
            mongoClient = new MongoClient(_connectionString);
            mongoDb = mongoClient.GetDatabase(_databaseName);
            collection = mongoDb.GetCollection<OpsReportHourlyWrapper>(OpsMongoDbStrings.OpsReportCollectionName);

            discounts = PerSiteDiscount.GetSiteDiscountPercentages().ToList();
        }

        public async Task Bootstrap()
        {
            GetAllEstablishments();
        }

        void GetAllEstablishments()
        {
            Establishments = db.Establishments.Where(x => x.establishment_id != 2).ToList();
        }

        public decimal GetBudgetForTodayOnly(int establishmentId)
        {
            return this.BudgetsTodayOnly.Where(x => x.EstablishmentId == establishmentId).Sum(x => x.Amount);
        }

        public decimal GetBudgetForEntireWTDNotIncludingToday(int establishmentId)
        {
            return this.BudgetsThisWeekNotIncludingToday.Where(x => x.EstablishmentId == establishmentId).Sum(x => x.Amount);
        }

        public decimal GetBudgetForEntireWTD(int establishmentId)
        {
            return this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == establishmentId).Sum(x => x.Amount);
        }

        public decimal GetBudgetForEntireWTD()
        {
            return this.BudgetsThisWTDIncludingToday.Sum(x => x.Amount);
        }

        public decimal GetBudgetForEntireLastWTD()
        {
            return this.BudgetsLastWTDIncludingLastWeekToday.Sum(x => x.Amount);
        }

        public decimal GetBudgetForEntireLastWTD(int establishmentId)
        {
            return this.BudgetsLastWTDIncludingLastWeekToday.Where(x => x.EstablishmentId == establishmentId).Sum(x => x.Amount);
        }


        public decimal GetBudgetForYesterday(int establishmentId)
        {
            return this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == establishmentId).Sum(x => x.Amount);
        }

        public GrindYesterdayBoardDataset GenerateYesterdayBoardDataset()
        {
            return new GrindYesterdayBoardDataset
            {
                YesterdayVsBudget_Yesterday = this.peristenceDataWrappersYesterday.Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                YesterdayVsBudget_Budget = this.BudgetsYesterday.Sum(x => x.Amount),

                Clerkenwell_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 8).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Clerkenwell_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 8).Sum(x => x.Amount),

                Covent_Garden_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 7).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Covent_Garden_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 7).Sum(x => x.Amount),

                Exmouth_Market_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 10).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Exmouth_Market_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 10).Sum(x => x.Amount),

                Facebook_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 11).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Facebook_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 11).Sum(x => x.Amount),

                Greenwich_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 13).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Greenwich_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 13).Sum(x => x.Amount),

                Hatton_Garden_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 5).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Hatton_Garden_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 5).Sum(x => x.Amount),

                Liverpool_Street_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 14).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Liverpool_Street_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 14).Sum(x => x.Amount),

                London_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 4).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                London_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 4).Sum(x => x.Amount),

                Royal_Exchange_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 6).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Royal_Exchange_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 6).Sum(x => x.Amount),

                Shoreditch_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 1).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Shoreditch_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 1).Sum(x => x.Amount),

                Soho_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 3).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Soho_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 3).Sum(x => x.Amount),

                Whitechapel_Yesterday = this.peristenceDataWrappersYesterday.Where(x => x.establishmentId == 9).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Whitechapel_Budget = this.BudgetsYesterday.Where(x => x.EstablishmentId == 9).Sum(x => x.Amount),

            };
        }

        public GrindTodayBoardDataset GenerateTodayBoardDataset()
        {
            return new GrindTodayBoardDataset
            {
                TodayVsSameDayLastWeek_Today = this.peristenceDataWrappersToday.Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                TodayVsSameDayLastWeek_LastWeek = this.peristenceDataWrappersTodayLastWeek.Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),

                TodayVsBudgets_Today = this.peristenceDataWrappersToday.Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                TodayVsBudgets_Budget = this.BudgetsTodayOnly.Sum(x => x.Amount),

                Clerkenwell_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 8).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Clerkenwell_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 8).Sum(x => x.Amount),

                Covent_Garden_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 7).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Covent_Garden_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 7).Sum(x => x.Amount),

                Exmouth_Market_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 10).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Exmouth_Market_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 10).Sum(x => x.Amount),

                Facebook_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 11).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Facebook_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 11).Sum(x => x.Amount),

                Greenwich_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 13).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Greenwich_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 13).Sum(x => x.Amount),

                Liverpool_Street_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 14).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Liverpool_Street_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 14).Sum(x => x.Amount),

                Hatton_Garden_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 5).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Hatton_Garden_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 5).Sum(x => x.Amount),

                London_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 4).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                London_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 4).Sum(x => x.Amount),

                Royal_Exchange_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 6).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Royal_Exchange_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 6).Sum(x => x.Amount),

                Soho_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 3).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Soho_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 3).Sum(x => x.Amount),

                Whitechapel_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 9).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Whitechapel_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 9).Sum(x => x.Amount),

                Shoreditch_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == 1).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Shoreditch_Budget = this.BudgetsTodayOnly.Where(x => x.EstablishmentId == 1).Sum(x => x.Amount)
            };
        }

        /// <summary>
        /// This uses same 'today board dataset' it's same widgets just different calcs - all widgets are 'week to date'
        /// </summary>
        /// <returns></returns>
        public GrindTodayBoardDataset GenerateWTDBoardDataset()
        {
            return new GrindTodayBoardDataset
            {
                TodayVsSameDayLastWeek_Today = this.peristenceDataWrappersEntireWeekPlusToday.Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                TodayVsSameDayLastWeek_LastWeek = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),

                TodayVsBudgets_Today = this.peristenceDataWrappersEntireWeekPlusToday.Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                TodayVsBudgets_Budget = this.GetBudgetForEntireWTD(),

                Clerkenwell_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 8).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Clerkenwell_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 8).Sum(x => x.Amount),

                Covent_Garden_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 7).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Covent_Garden_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 7).Sum(x => x.Amount),

                Exmouth_Market_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 10).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Exmouth_Market_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 10).Sum(x => x.Amount),

                Facebook_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 11).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Facebook_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 11).Sum(x => x.Amount),

                Greenwich_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 13).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Greenwich_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 13).Sum(x => x.Amount),

                Liverpool_Street_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 14).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Liverpool_Street_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 14).Sum(x => x.Amount),

                Hatton_Garden_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 5).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Hatton_Garden_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 5).Sum(x => x.Amount),

                London_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 4).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                London_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 4).Sum(x => x.Amount),

                Royal_Exchange_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 6).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Royal_Exchange_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 6).Sum(x => x.Amount),

                Soho_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 3).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Soho_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 3).Sum(x => x.Amount),

                Whitechapel_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 9).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Whitechapel_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 9).Sum(x => x.Amount),

                Shoreditch_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == 1).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),
                Shoreditch_Budget = this.BudgetsThisWTDIncludingToday.Where(x => x.EstablishmentId == 1).Sum(x => x.Amount)
            };
        }

        public IndividualBoardDataset GenerateIndiviudalBoardDataset(int establishmentId)
        {
            if (establishmentId == 11)
            {
                var stopPlz = "";
            }
            try
            {
                var SalesTodayVsBudget_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price));
                var SalesTodayVsBudget_Budget = GetBudgetForTodayOnly(establishmentId);

                var SalesTodayVsLastWeek_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price));
                var SalesTodayVsLastWeek_LastWeek = this.peristenceDataWrappersTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price));

                var WTDVsBudget_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price));
                var WTDVsBudget_Budget = this.GetBudgetForEntireWTD(establishmentId);

                var WTDVsLastWeek_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price));
                var WTDVsLastWeek_LastWeek = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price));

                //cats
                var coffeeWTD = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Coffee/Hot Drinks").FirstOrDefault()?.price ?? 0.00M));
                var coffeeWTDLast = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Coffee/Hot Drinks").FirstOrDefault()?.price ?? 0.00M));

                var coffeeVolToday = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToInt32(report.opsReport.product_mix_data.Where(x => x.product_name == "Coffee/Hot Drinks").FirstOrDefault()?.n_items));
                var coffeeVolLast = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToInt32(report.opsReport.product_mix_data.Where(x => x.product_name == "Coffee/Hot Drinks").FirstOrDefault()?.n_items));

                var barWTD = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Bar").FirstOrDefault()?.price ?? 0.00M));
                var barWTDLast = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Bar").FirstOrDefault()?.price ?? 0.00M));

                var foodWTD = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Food").FirstOrDefault()?.price ?? 0.00M));
                var foodWTDLast = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Food").FirstOrDefault()?.price ?? 0.00M));

                var juiceWTD = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Juice").FirstOrDefault()?.price ?? 0.00M));
                var juiceWTDLast = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Juice").FirstOrDefault()?.price ?? 0.00M));

                var reatilWTD = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Retail").FirstOrDefault()?.price ?? 0.00M));
                var reatilWTDLast = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Retail").FirstOrDefault()?.price ?? 0.00M));

                var softDrinksWTD = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Soft Drinks").FirstOrDefault()?.price ?? 0.00M));
                var softDrinksWTDLast = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(report => Convert.ToDecimal(report.opsReport.product_mix_data.Where(x => x.product_name == "Soft Drinks").FirstOrDefault()?.price ?? 0.00M));

                ////SUMS FOR COVERS / AVG
                var todayTotalSales = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price));
                var todayCovers = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(x => Convert.ToDecimal(x.opsReport.sales_data.total_number_of_people));


                var lastWeekTotalSales = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price));
                var lastWeekCovers = this.peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(x => Convert.ToDecimal(x.opsReport.sales_data.total_number_of_people));

                var CoversWTDVsLastWeekToday = todayCovers;
                var CoversWTDVsLastWeekLastWeek = lastWeekCovers;


                var AverageCoverValueToday = 0.00M;
                var AverageCoverValueLastWeek = 0.00M;

                if (CoversWTDVsLastWeekToday > 0 && CoversWTDVsLastWeekLastWeek > 0)
                {
                    AverageCoverValueToday = this.peristenceDataWrappersToday.Where(x => x.establishmentId == establishmentId).Sum(x => Convert.ToDecimal(x.opsReport.sales_data.avg_sale_per_person)); //todayTotalSales / todayCovers;
                    AverageCoverValueLastWeek = this.peristenceDataWrappersTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(x => Convert.ToDecimal(x.opsReport.sales_data.avg_sale_per_person));
                    //this.peristenceDataWrappersToday.Where(x => x.establishmentId == establishmentId).Sum(x => Convert.ToDecimal(x.opsReport.sales_data.avg_sale_per_person));
                    //this.peristenceDataWrappersTodayLastWeek.Where(x => x.establishmentId == establishmentId).Sum(x => Convert.ToDecimal(x.opsReport.sales_data.avg_sale_per_person));
                }


                PerSiteDiscount discountPercenttage = discounts.First(x => x.EstablishmentId.ToString() == establishmentId.ToString());

                var cumulativehourAndSpendToday = new List<HourAndSpend>();
                var cumulativehourAndSpendBudget = new List<HourAndSpend>();
                var cumulativehourAndSpendSameDayLastWeek = new List<HourAndSpend>();
                List<DateTime> hours = this.peristenceDataWrappersToday.Select(x => x.containerStart).Distinct().ToList();

                var todaySum = 0.00M;
                var budgetSum = 0.00M;
                var lastWeekSum = 0.00M;

                if (SalesTodayVsBudget_Today > 0)
                {
                    foreach (var item in hours)
                    {
                        var today = peristenceDataWrappersToday.Where(x => x.establishmentId == establishmentId).FirstOrDefault(x => x.containerStart == item);
                        var sameDayLastWeek = peristenceDataWrappersTodayLastWeek.Where(x => x.establishmentId == establishmentId).FirstOrDefault(x => x.containerStart == item.AddDays(-7));
                        var budget = BudgetsTodayOnly
                            .Where(x => x.EstablishmentId == establishmentId)
                           .Where(x => x.BudgetDate >= item && x.BudgetDate < item.AddHours(1)).Sum(x => x.Amount); //there's 4 budgets per hour


                        todaySum += Convert.ToDecimal(today.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price);
                        budgetSum += budget;
                        lastWeekSum += Convert.ToDecimal(sameDayLastWeek.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price);

                        cumulativehourAndSpendToday.Add(new HourAndSpend { Date = today.containerStart, Hour = today.containerStart.ToString(), Value = todaySum + Convert.ToDecimal(today.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price) });
                        cumulativehourAndSpendBudget.Add(new HourAndSpend { Date = today.containerStart, Hour = today.containerStart.ToString(), Value = budgetSum + Convert.ToDecimal(budget) });
                        cumulativehourAndSpendSameDayLastWeek.Add(new HourAndSpend { Date = sameDayLastWeek.containerStart, Hour = sameDayLastWeek.containerStart.ToString(), Value = lastWeekSum + Convert.ToDecimal(sameDayLastWeek.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price) });
                    }
                }

                //discount needs to be 'just today'
                var discountBudget = discountPercenttage.DiscountSalesPercentage * this.peristenceDataWrappersToday.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price));
                //ACTUAL POCO
                var widgetDataset = new IndividualBoardDataset
                {
                    EstablishmentId = establishmentId,
                    //just today
                    SalesTodayVsBudget_Today = SalesTodayVsBudget_Today,
                    SalesTodayVsBudget_Budget = SalesTodayVsBudget_Budget,

                    SalesTodayVsLastWeek_Today = SalesTodayVsLastWeek_Today,
                    SalesTodayVsLastWeek_LastWeek = SalesTodayVsLastWeek_LastWeek,

                    WTDVsBudget_Today = WTDVsBudget_Today,
                    WTDVsBudget_Budget = WTDVsBudget_Budget,

                    WTDVsLastWeek_Today = WTDVsLastWeek_Today,
                    WTDVsLastWeek_LastWeek = WTDVsLastWeek_LastWeek,

                    //CATS
                    CoffeeWTDSalesVsLastWeek_Today = coffeeWTD,
                    CoffeeWTDSalesVsLastWeek_LastWeek = coffeeWTDLast,

                    BarWTDSalesVsLastWeek_Today = barWTD,
                    BarWTDSalesVsLastWeek_LastWeek = barWTDLast,

                    FoodWTDSalesVsLastWeek_Today = foodWTD,
                    FoodWTDSalesVsLastWeek_LastWeek = foodWTDLast,

                    RetailTodayVsLastWeek_Today = reatilWTD,
                    RetailTodayVsLastWeek_LastWeek = reatilWTDLast,

                    CoffeeVolumeTodayVsLastWeek_Today = coffeeVolToday,
                    CoffeeVolumeTodayVsLastWeek_LastWeek = coffeeVolLast,

                    CoversWTDVsLastWeek_Today = CoversWTDVsLastWeekToday,
                    CoversWTDVsLastWeek_LastWeek = CoversWTDVsLastWeekLastWeek,

                    AverageCoverValueWTD_Today = AverageCoverValueToday,
                    AverageCoverValueWTD_LastWeek = AverageCoverValueLastWeek,

                    DiscountTodayVsBudget_Today = this.peristenceDataWrappersToday.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.sales_data.total_discounts)),
                    DiscountTodayVsBudget_Budget = discountPercenttage.DiscountSalesPercentage * this.peristenceDataWrappersToday.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.product_mix_data.First(y => y.row_type == "totals_row").price)),

                    WTDDiscountVsBudget_Today = this.peristenceDataWrappersEntireWeekPlusToday.Where(x => x.establishmentId == establishmentId).Sum(X => Convert.ToDecimal(X.opsReport.sales_data.total_discounts)),
                    WTDDiscountVsBudget_Budget = discountBudget,

                    CumulativeHourAndSpendsBudget = cumulativehourAndSpendBudget,
                    CumulativeHourAndSpendsSameDayLastWeeek = cumulativehourAndSpendSameDayLastWeek,
                    CumulativeHourAndSpendsToday = cumulativehourAndSpendToday
                };

                individualBoardDatasets.Add(widgetDataset);

                return widgetDataset;
            }
            catch (Exception ex)
            {
                var whichGrin = establishmentId;
                //throw;
            }

            return new IndividualBoardDataset();
        }

        public async Task GatherAllDailyAndComparisonRawData(DateTime currentTime)
        {
            var todayStart = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 03, 00, 00);
            var yesterdayStart = todayStart.AddDays(-1);
            var startOfWeekMonday = todayStart.StartOfWeek(DayOfWeek.Monday);
            startOfWeekMonday = new DateTime(startOfWeekMonday.Year, startOfWeekMonday.Month, startOfWeekMonday.Day, 03, 00, 00);

            var startOfWeekMondayLastWeek = startOfWeekMonday.AddDays(-7);
            var todayLastWeek = todayStart.AddDays(-7);

            foreach (var est in Establishments.Where(x => x.establishment_id != 2))
            {
                try
                {
                    ///populate today
                    await GatherDailyHourlyIncrementalDataUpUntilNowFromRevel(todayStart, currentTime, est.establishment_id, peristenceDataWrappersToday);
                    //populate today last week
                    await GatherDailyHourlyIncrementalDataUpUntilNowFromRevel(todayStart.AddDays(-7), currentTime.AddDays(-7), est.establishment_id, peristenceDataWrappersTodayLastWeek);

                    var yeesterdayWrapper = collection.AsQueryable().Where(x => x.establishmentId == est.establishment_id).Where(x => x.containerStart >= yesterdayStart && x.containerStart < todayStart).ToList();
                    peristenceDataWrappersYesterday.AddRange(yeesterdayWrapper);
                    peristenceDataWrappersEntireWeekPlusToday.AddRange(this.peristenceDataWrappersToday.Where(x => x.establishmentId == est.establishment_id).ToList());
                    peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.AddRange(this.peristenceDataWrappersTodayLastWeek.Where(x => x.establishmentId == est.establishment_id).ToList());

                    //budgets
                    var wholeWhatButNotTodayForThisEstablishment = budgetDb.Budget2019s.Where(X => X.EstablishmentId == est.establishment_id).Where(x => x.BudgetDate >= startOfWeekMonday).Where(x => x.BudgetDate < todayStart).ToList();
                    var todayOnly = budgetDb.Budget2019s.Where(x => x.BudgetDate >= todayStart).Where(X => X.EstablishmentId == est.establishment_id).Where(x => x.BudgetDate < currentTime).ToList();

                    var entireLastWeekWTD = budgetDb.Budget2019s.Where(x => x.BudgetDate >= startOfWeekMondayLastWeek).Where(x => x.BudgetDate < todayLastWeek).ToList();

                    this.BudgetsTodayOnly.AddRange(todayOnly);
                    this.BudgetsThisWeekNotIncludingToday.AddRange(wholeWhatButNotTodayForThisEstablishment);

                    this.BudgetsThisWTDIncludingToday.AddRange(todayOnly.Concat(wholeWhatButNotTodayForThisEstablishment));
                    this.BudgetsLastWTDIncludingLastWeekToday.AddRange(entireLastWeekWTD);

                    var yesterdayBudgets = budgetDb.Budget2019s.Where(X => X.EstablishmentId == est.establishment_id).Where(x => x.BudgetDate >= yesterdayStart && x.BudgetDate < todayStart).ToList();
                    this.BudgetsYesterday.AddRange(yesterdayBudgets);

                    //TEST CODE                    
                    var test = BudgetsThisWTDIncludingToday.Where(x => x.BudgetDate > currentTime);
                    var sumBudgetThisWEek = BudgetsThisWTDIncludingToday.Sum(x => x.Amount);

                    if (todayStart.DayOfWeek != DayOfWeek.Monday)
                    {
                        //populate Monday until today (WTD)
                        var everytthingUpUntilToday = await GatherDailyHourlyIncrementalDataUpUntilNowFromMongoDb(startOfWeekMonday, todayStart, est.establishment_id, this.peristenceDataWrappersWeekToDateUntilToday);
                        //populate WTD comparison period
                        var sameLaskWeek = await GatherDailyHourlyIncrementalDataUpUntilNowFromMongoDb(startOfWeekMondayLastWeek, todayLastWeek, est.establishment_id, this.peristenceDataWrappersLastWeekToDatePointComparisonPeriod);

                        //amalgam today                     
                        peristenceDataWrappersEntireWeekPlusToday.AddRange(everytthingUpUntilToday);
                        peristenceDataWrappersEntireLastWeekPlusTodayLastWeek.AddRange(sameLaskWeek);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception in establishment " + est.name + " - " + ex.Message);
                    throw;
                }
            }
        }


        /// <summary>
        /// Gets hourly wrappers from Revel - for all Grinds
        /// </summary>
        /// <param name="dayStart"></param>
        /// <param name="dayEnd"></param>
        /// <param name="establishmentIdOfGrind"></param>
        /// <param name="wrapperToPopulate"></param>
        /// <returns></returns>
        async Task<List<OpsReportHourlyWrapper>> GatherDailyHourlyIncrementalDataUpUntilNowFromMongoDb(DateTime dayStart,
            DateTime dayEnd,
            int establishmentId,
            List<OpsReportHourlyWrapper> wrapperToPopulate)
        {
            List<OpsReportHourlyWrapper> result = this.collection.AsQueryable()
                .Where(x => x.containerStart >= dayStart)
                .Where(x => x.containerEnd < dayEnd)
                .Where(x => x.establishmentId == establishmentId)
                .ToList();

            return result;

        }


        /// <summary>
        /// Needs to get hourly of data from API - every 15 mins
        /// </summary>
        /// <param name="dayStart"></param>
        /// <param name="currentPoint"></param>
        /// <param name="establishmentIdOfGrind"></param>
        /// <returns></returns>
        async Task GatherDailyHourlyIncrementalDataUpUntilNowFromRevel(DateTime dayStart, DateTime currentEndpoint, int establishmentIdOfGrind, List<OpsReportHourlyWrapper> wrapperToPopulate)
        {
            //generate hourly minute blocks
            var _hourlyBlocks = new List<DateTimeStartEndRange>();
            var currentLoopDate = dayStart;
            while (currentLoopDate < currentEndpoint)
            {
                var endOfLoop = currentLoopDate.AddMinutes(60);
                _hourlyBlocks.Add(new DateTimeStartEndRange { Start = currentLoopDate, End = endOfLoop });
                //increment
                currentLoopDate = endOfLoop;
            }

            foreach (var hourlyBlock in _hourlyBlocks)
            {
                var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}&range_to={1}&establishment={2}", hourlyBlock.Start.ToRevelDate(), hourlyBlock.End.ToRevelDate(), establishmentIdOfGrind);

                var response = await client.GetAsync(query);
                var content = await response.Content.ReadAsStringAsync();
                var poco = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(content);

                wrapperToPopulate.Add(new OpsReportHourlyWrapper
                {
                    containerStart = hourlyBlock.Start,
                    containerEnd = hourlyBlock.End,
                    establishmentId = establishmentIdOfGrind,
                    opsReport = poco
                });
            }

            //done
        }

    }
}
