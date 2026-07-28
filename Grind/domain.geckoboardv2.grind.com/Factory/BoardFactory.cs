using domain.geckoboardv2.grind.com.Models.BoardTypeWidgetUrls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.geckoboardv2.grind.com.Factory
{
    public static class BoardFactory
    {

        public static IEnumerable<IndividualStoreBase> CreateIndividualGrindStoreBoards()
        {
            return new List<IndividualStoreBase>
            {
                //shoreditch
                new IndividualStoreBase{
                   EstablishmentId = 1,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-07ff5e30-796f-0137-4a6e-02f51f295d3c",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-08021760-796f-0137-4a70-02f51f295d3c",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-08035fe0-796f-0137-4a71-02f51f295d3c",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-0804a9e0-796f-0137-4a72-02f51f295d3c",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-080863f0-796f-0137-4a75-02f51f295d3c",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-080b0e00-796f-0137-4a77-02f51f295d3c",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-08134370-796f-0137-4a7d-02f51f295d3c",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-080c4b20-796f-0137-4a78-02f51f295d3c",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-b52a0a00-7a35-0137-9207-0e2ca7cd513a",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-b2442f70-7a36-0137-920a-0e2ca7cd513a",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-b6bb5560-7a36-0137-6276-0a2846fe5984",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-c02f1630-7a36-0137-a38c-0eb94e5f3dd6",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-5c24acd0-7a38-0137-920d-0e2ca7cd513a",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-51b82b60-7a39-0137-8873-0201ed3e2634"
                },
                //soho
                new IndividualStoreBase{
                   EstablishmentId = 3,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-84716690-7bee-0137-cff2-0e6b2ce6b99e",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-847282c0-7bee-0137-cff3-0e6b2ce6b99e",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-847381a0-7bee-0137-cff4-0e6b2ce6b99e",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-84748d40-7bee-0137-cff5-0e6b2ce6b99e",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-8475c200-7bee-0137-cff6-0e6b2ce6b99e",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-8476f4e0-7bee-0137-cff7-0e6b2ce6b99e",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-84792130-7bee-0137-cff9-0e6b2ce6b99e",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-847807e0-7bee-0137-cff8-0e6b2ce6b99e",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-847a4f30-7bee-0137-cffa-0e6b2ce6b99e",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-847b8f40-7bee-0137-cffb-0e6b2ce6b99e",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-847ceb40-7bee-0137-cffc-0e6b2ce6b99e",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-847e8120-7bee-0137-cffd-0e6b2ce6b99e",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-847fb4b0-7bee-0137-cffe-0e6b2ce6b99e",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-8480e130-7bee-0137-cfff-0e6b2ce6b99e"
                },
                //London
                new IndividualStoreBase{
                   EstablishmentId = 4,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-41db7ea0-7bf1-0137-4fc9-02f51f295d3c",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-41dce320-7bf1-0137-4fca-02f51f295d3c",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-41de9840-7bf1-0137-4fcb-02f51f295d3c",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-41e24b00-7bf1-0137-4fcc-02f51f295d3c",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-41e48d10-7bf1-0137-4fcd-02f51f295d3c",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-41e5d3b0-7bf1-0137-4fce-02f51f295d3c",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-41e89810-7bf1-0137-4fd0-02f51f295d3c",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-41e74460-7bf1-0137-4fcf-02f51f295d3c",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-41e9d820-7bf1-0137-4fd1-02f51f295d3c",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-41eb5b10-7bf1-0137-4fd2-02f51f295d3c",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-41ed01d0-7bf1-0137-4fd3-02f51f295d3c",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-41ee9ce0-7bf1-0137-4fd4-02f51f295d3c",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-41effcf0-7bf1-0137-4fd5-02f51f295d3c",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-41f19150-7bf1-0137-4fd6-02f51f295d3c"
                },
                 //Hatton Garden
                new IndividualStoreBase{
                   EstablishmentId = 5,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-cc88fff0-7bf1-0137-6547-0af6c1ee7058",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-cc8ba400-7bf1-0137-6548-0af6c1ee7058",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-cc8de980-7bf1-0137-6549-0af6c1ee7058",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-cc8fb6d0-7bf1-0137-654a-0af6c1ee7058",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-cc91a7e0-7bf1-0137-654b-0af6c1ee7058",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-cc939250-7bf1-0137-654c-0af6c1ee7058",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-cc96eb00-7bf1-0137-654e-0af6c1ee7058",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-cc954020-7bf1-0137-654d-0af6c1ee7058",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-cc98a2f0-7bf1-0137-654f-0af6c1ee7058",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-cc9ab5d0-7bf1-0137-6550-0af6c1ee7058",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-cc9cc250-7bf1-0137-6551-0af6c1ee7058",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-cc9eca20-7bf1-0137-6552-0af6c1ee7058",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-cca0c150-7bf1-0137-6553-0af6c1ee7058",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-cca2c700-7bf1-0137-6554-0af6c1ee7058"
                },
                 //Royal Exchange
                new IndividualStoreBase{
                   EstablishmentId = 6,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-6f870d60-7bf4-0137-4fda-02f51f295d3c",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-6f88c320-7bf4-0137-4fdb-02f51f295d3c",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-6f8a5cc0-7bf4-0137-4fdc-02f51f295d3c",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-6f8d7d10-7bf4-0137-4fdd-02f51f295d3c",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-6f8ee590-7bf4-0137-4fde-02f51f295d3c",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-6f902410-7bf4-0137-4fdf-02f51f295d3c",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-6f92d9c0-7bf4-0137-4fe1-02f51f295d3c",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-6f917260-7bf4-0137-4fe0-02f51f295d3c",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-6f944220-7bf4-0137-4fe2-02f51f295d3c",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-6f95ac20-7bf4-0137-4fe3-02f51f295d3c",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-6f9764a0-7bf4-0137-4fe4-02f51f295d3c",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-6f9908a0-7bf4-0137-4fe5-02f51f295d3c",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-6f9ae040-7bf4-0137-4fe6-02f51f295d3c",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-6f9c7a70-7bf4-0137-4fe7-02f51f295d3c"
                },
                 //Covent Garden
                new IndividualStoreBase{
                   EstablishmentId = 7,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-070fa8b0-7bf5-0137-6557-0af6c1ee7058",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-07116fe0-7bf5-0137-6558-0af6c1ee7058",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-07133cd0-7bf5-0137-6559-0af6c1ee7058",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-0714fc20-7bf5-0137-655a-0af6c1ee7058",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-0716a4b0-7bf5-0137-655b-0af6c1ee7058",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-07186b80-7bf5-0137-655c-0af6c1ee7058",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-071bf400-7bf5-0137-655e-0af6c1ee7058",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-071a2070-7bf5-0137-655d-0af6c1ee7058",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-071dff50-7bf5-0137-655f-0af6c1ee7058",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-07201860-7bf5-0137-6560-0af6c1ee7058",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-07224e30-7bf5-0137-6561-0af6c1ee7058",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-07244820-7bf5-0137-6562-0af6c1ee7058",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-07263dc0-7bf5-0137-6563-0af6c1ee7058",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-072833f0-7bf5-0137-6564-0af6c1ee7058"
                },
                 //Clerkenwell
                new IndividualStoreBase{
                   EstablishmentId = 8,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-17891000-7c94-0137-5072-02f51f295d3c",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-178a47a0-7c94-0137-5073-02f51f295d3c",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-178b9950-7c94-0137-5074-02f51f295d3c",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-178ce4e0-7c94-0137-5075-02f51f295d3c",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-178e23d0-7c94-0137-5076-02f51f295d3c",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-178f6c70-7c94-0137-5077-02f51f295d3c",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-1790a1c0-7c94-0137-5078-02f51f295d3c",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-17931e10-7c94-0137-507a-02f51f295d3c",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-179493d0-7c94-0137-507b-02f51f295d3c",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-17960f50-7c94-0137-507c-02f51f295d3c",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-1797b130-7c94-0137-507d-02f51f295d3c",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-179961b0-7c94-0137-507e-02f51f295d3c",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-179ae120-7c94-0137-507f-02f51f295d3c"
                },
                 //Whitechapel
                new IndividualStoreBase{
                   EstablishmentId = 9,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-a2749350-7c94-0137-5081-02f51f295d3c",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-a27623d0-7c94-0137-5082-02f51f295d3c",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-a2778440-7c94-0137-5083-02f51f295d3c",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-a278ced0-7c94-0137-5084-02f51f295d3c",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-a27a1d40-7c94-0137-5085-02f51f295d3c",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-a27b5a70-7c94-0137-5086-02f51f295d3c",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-a27de590-7c94-0137-5088-02f51f295d3c",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-a27c9560-7c94-0137-5087-02f51f295d3c",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-a27f1f10-7c94-0137-5089-02f51f295d3c",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-a2808d70-7c94-0137-508a-02f51f295d3c",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-a2820990-7c94-0137-508b-02f51f295d3c",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-a283c0e0-7c94-0137-508c-02f51f295d3c",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-a2856430-7c94-0137-508d-02f51f295d3c",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-a286e530-7c94-0137-508e-02f51f295d3c"
                },
                //Exmouth Market
                new IndividualStoreBase{
                   EstablishmentId = 10,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-960d1c30-7caa-0137-d08f-0e6b2ce6b99e",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-960e22f0-7caa-0137-d090-0e6b2ce6b99e",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-960f2840-7caa-0137-d091-0e6b2ce6b99e",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-96109bd0-7caa-0137-d092-0e6b2ce6b99e",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-9611c7a0-7caa-0137-d093-0e6b2ce6b99e",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-9612fee0-7caa-0137-d094-0e6b2ce6b99e",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-961546d0-7caa-0137-d096-0e6b2ce6b99e",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-96140d80-7caa-0137-d095-0e6b2ce6b99e",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-96164c10-7caa-0137-d097-0e6b2ce6b99e",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-96177940-7caa-0137-d098-0e6b2ce6b99e",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-9618a2b0-7caa-0137-d099-0e6b2ce6b99e",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-9619d230-7caa-0137-d09a-0e6b2ce6b99e",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-961b0510-7caa-0137-d09b-0e6b2ce6b99e",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-961c4170-7caa-0137-d09c-0e6b2ce6b99e"
                },
                  //Facebook
                new IndividualStoreBase{
                   EstablishmentId = 11,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-94da8d30-7cae-0137-50c8-02f51f295d3c",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-94dbd540-7cae-0137-50c9-02f51f295d3c",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-94dd3da0-7cae-0137-50ca-02f51f295d3c",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-94de9a50-7cae-0137-50cb-02f51f295d3c",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-94e03690-7cae-0137-50cc-02f51f295d3c",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-94e175b0-7cae-0137-50cd-02f51f295d3c",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-94e46980-7cae-0137-50cf-02f51f295d3c",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-94e64d10-7cae-0137-50d0-02f51f295d3c",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-94e830a0-7cae-0137-50d1-02f51f295d3c",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-94e830a0-7cae-0137-50d1-02f51f295d3c",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-94e9b850-7cae-0137-50d2-02f51f295d3c",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-94eb4d60-7cae-0137-50d3-02f51f295d3c",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-94ecf1d0-7cae-0137-50d4-02f51f295d3c",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-94ee74c0-7cae-0137-50d5-02f51f295d3c"
                },
                     //Greenwich
                new IndividualStoreBase{
                   EstablishmentId = 13,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-adae9520-7cae-0137-50d7-02f51f295d3c",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-adafce00-7cae-0137-50d8-02f51f295d3c",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-adb10010-7cae-0137-50d9-02f51f295d3c",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-adb22e20-7cae-0137-50da-02f51f295d3c",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-adb36160-7cae-0137-50db-02f51f295d3c",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-adb4cc90-7cae-0137-50dc-02f51f295d3c",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-adb76b60-7cae-0137-50de-02f51f295d3c",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-adb61ff0-7cae-0137-50dd-02f51f295d3c",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-adb8c730-7cae-0137-50df-02f51f295d3c",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-adba4c70-7cae-0137-50e0-02f51f295d3c",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-adbbc020-7cae-0137-50e1-02f51f295d3c",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-adbd4850-7cae-0137-50e2-02f51f295d3c",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-adbf08e0-7cae-0137-50e3-02f51f295d3c",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-adc08fd0-7cae-0137-50e4-02f51f295d3c"
                },
                //Liverpool Street
                new IndividualStoreBase{
                   EstablishmentId = 14,

                   SalesTodayVsLastWeek = "https://push.geckoboard.com/v1/send/-3cd3ad30-7cb2-0137-d0a0-0e6b2ce6b99e",
                   SalesTodayVsBudget = "https://push.geckoboard.com/v1/send/-3cd49ef0-7cb2-0137-d0a1-0e6b2ce6b99e",
                   DiscountTodayVsBudget  = "https://push.geckoboard.com/v1/send/-3cd59330-7cb2-0137-d0a2-0e6b2ce6b99e",
                   WTDVsLastWeek = "https://push.geckoboard.com/v1/send/-3cd68040-7cb2-0137-d0a3-0e6b2ce6b99e",
                   WTDVsBudget = "https://push.geckoboard.com/v1/send/-3cd79f20-7cb2-0137-d0a4-0e6b2ce6b99e",
                   WTDDiscountVsBudget = "https://push.geckoboard.com/v1/send/-3cd8e7c0-7cb2-0137-d0a5-0e6b2ce6b99e",
                   CumulativeHourlySales = "https://push.geckoboard.com/v1/send/-3cdb8520-7cb2-0137-d0a7-0e6b2ce6b99e",

                   CoffeeWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/-3cd9e910-7cb2-0137-d0a6-0e6b2ce6b99e",
                   FoodWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-3cde7340-7cb2-0137-d0a8-0e6b2ce6b99e",
                   BarWTDSalesVsLastWeek= "https://push.geckoboard.com/v1/send/51912-3cdfe910-7cb2-0137-d0a9-0e6b2ce6b99e",
                   RetailTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-3ce0f760-7cb2-0137-d0aa-0e6b2ce6b99e",
                   CoffeeVolumeTodayVsLastWeek= "https://push.geckoboard.com/v1/send/51912-3ce22e50-7cb2-0137-d0ab-0e6b2ce6b99e",

                   CoversWTDVsLastWeek= "https://push.geckoboard.com/v1/send/51912-3ce34200-7cb2-0137-d0ac-0e6b2ce6b99e",
                   AverageCoverValue= "https://push.geckoboard.com/v1/send/51912-3ce45d10-7cb2-0137-d0ad-0e6b2ce6b99e"
                },
            };
        }
    }
}
