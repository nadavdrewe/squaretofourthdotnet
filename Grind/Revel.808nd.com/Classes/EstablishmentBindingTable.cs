using System.Collections.Generic;

namespace Revel._808nd.com.Classes
{
    public class EstablishmentBindingTable
    {

        public Dictionary<int, string> widgetBindMappings { get; set; }


        public EstablishmentBindingTable(int ID)
        {
            widgetBindMappings = new Dictionary<int, string>();
            switch (ID)
            {
                //shoreditch
                case 1:
                    //today vs same day last  week -1                
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/54410-fc682d7c-6c0e-472e-97d5-619329a387e6");
                    //avg spend -2 
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/79220-fe700c4f-92e9-47e7-9701-da3d57f50cfe");
                    //no of orders today -3
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/54410-f4af5c85-1829-48a3-b5aa-90c31a73324a");
                    //Booze sales - 4
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/54410-f8865585-67f8-4eeb-b00e-d80d326b6bec");
                    //hot drink NUMBER -5
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/54410-5e607a48-d1b3-498e-b03a-bf437a51eedf");
                    //FOOD - 6
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/54410-4059a9b0-a6c8-4547-8af2-99583214d90d");
                    //LAST UPDATED - 7
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/54410-9477cd53-52ce-4f1c-95ca-fa534da6d40b");
                    //NO OF SOFT DRINKS - 8
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/54410-0e2a483a-7097-4263-98cd-512ef0ce018b");
                    //HOUR AND SPEND
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/54410-819b588f-87b8-471c-9967-63d4efbce153");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/79220-180b2590-e3b0-40ee-9227-495d095692e8");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/79220-4b93445b-c282-4b50-8f85-ba43c0273497");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/79220-0367bf7e-b5c2-40a7-81aa-8b703ce0bb4d");
                    //WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/79220-3df6af56-4cea-4d0b-8738-37aa8dbfee34");
                    //breakfast
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/79220-41a308f0-b90c-4d6c-9c0a-9766db59a2d2");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/79220-c3eb7146-69b2-4ff5-8183-5d89a659cc92");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/79220-1cc1c815-a265-4d4c-89e9-ff4fd6f7a712");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/79220-78b5b304-4b8c-4ddb-8de7-0d411e63558b");

                    //LastWeekNetVSweekBeforeNet
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/54410-50711c58-6f95-44f2-9c03-4bf7f971891d");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/54410-fe62415b-48bb-4755-9130-b93b6ffe11cc");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/54410-91b9e90d-24eb-42c8-b9e4-ef5a999fa3a5");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/79220-4dd3b595-3ea8-4efd-9459-7d6cefc69471");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/54410-d0e5bbf4-7176-456c-886b-197b8ebce1b5");

                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/54410-e3761368-a0f7-4051-9a51-ace158885c63");

                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/79220-5546a81d-7c78-4f43-96c1-12f8d4df66f7");

                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/79220-c0f336e1-5678-4488-be0d-f9873e20ecb4");

                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/79220-c231ad8f-ce20-4f46-9e77-0f77483e12c3");

                    break;

                //parent
                case 2:
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/54410-6a341a20-c99b-0131-2f92-22000a1fda8a");
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/95518-c499883d-a59a-4eed-a7b1-ca2b7cbd64ca");
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/54410-6a37d700-c99b-0131-2f93-22000a1fda8a");
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/54410-6a3d46f0-c99b-0131-2f97-22000a1fda8a");
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/54410-6a393510-c99b-0131-2f94-22000a1fda8a");
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/54410-6a3e7ff0-c99b-0131-2f98-22000a1fda8a");
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/54410-6a2d9990-c99b-0131-2f8e-22000a1fda8a");
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/54410-6a3b8cd0-c99b-0131-2f96-22000a1fda8a");
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/54410-6a40d430-c99b-0131-2f99-22000a1fda8a");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/95518-96963721-3610-41a7-a82a-5574acce9eff");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/95518-8aabbba2-a32d-4155-b3e1-a7e842eb972a");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/95518-9047a8c1-0820-4c63-a9c6-4990c4e59fa3");
                    //WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/95518-7d17b7b1-03d2-4baf-bf89-fbb8ddaac7a1");
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/95518-60631d18-486e-4ab4-ab07-6cf20d964501");

                    //LastWeekNetVSweekBeforeNet
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/54410-6a29d0e0-c99b-0131-2f8c-22000a1fda8a");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/54410-6a27bad0-c99b-0131-2f8b-22000a1fda8a");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/54410-6a2bd990-c99b-0131-2f8d-22000a1fda8a");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/95518-7d17b7b1-03d2-4baf-bf89-fbb8ddaac7a1");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/54410-6a32f130-c99b-0131-2f91-22000a1fda8a");
                    //Last 30 days gross                
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/54410-6a448750-c99b-0131-2f9b-22000a1fda8a");

                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/95518-6c7211e7-c644-411c-863d-7fce8ced74da");
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/95518-b6774dfe-d7f3-470d-bd16-df22ebcf0fe8");

                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/95518-0de438bb-2282-4e0d-98f6-a8cc9bf3513b");

                    break;

                //soho
                case 3:
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/54410-eb839460-b4da-0131-6648-22000a1fabf9");
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/92011-f8763fba-7c5d-414f-82e0-9f5dccf3a45a");
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/54410-7d2e238b-4ff1-4b5e-a19b-9136c0d01cd9");
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/54410-bda512ac-549b-4109-8b7e-3e3bb65b5284");
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/54410-0b8503bb-6209-4565-9789-204dad45f2ef");
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/54410-fca4c3a6-d19c-4cf7-a9aa-3543deb76607");
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/54410-4d5be68f-046b-478c-87c7-61a4ba635a71");
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/54410-1a234e47-3d67-4bf6-b6f6-851f87cfba6e");
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/54410-ba257efc-5d17-470e-a30f-65e556d2996c");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/92011-198cb85e-77fb-4d7e-b1d7-48cbeb31c9e6");
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/92011-a47e22c3-ce54-4f3b-b29b-372c12f37962");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/92011-f7348e14-f6cc-4db3-ab14-3705c88a7a71");
                    //WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/92011-ccd8a2a3-d71c-4b4a-8498-d8cd3f994e93");
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/92011-2292254f-188d-4dae-9e64-f0fadc66c92c");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/92011-f332572f-7268-496a-99fa-cfa07f14bd96");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/92011-68e99e8b-b4b7-4e9a-ae57-a9f82bb35539");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/92011-506660ad-d39e-4ce8-9405-910ac5b5c686");

                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/54410-eb8d18c0-b4da-0131-6650-22000a1fabf9");
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/54410-eb8bfc20-b4da-0131-664f-22000a1fabf9");
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/54410-eb8e5300-b4da-0131-6651-22000a1fabf9");
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/92011-25b3bb27-2808-4116-990a-7160bd4cb497");
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/54410-90e402fe-ea78-45cb-9608-fc442d413492");
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/54410-79609e24-2783-4402-bc05-1c827ee3ed10");
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/92011-7a63de25-55a0-4f19-b346-7fd3ced51084");
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/92011-c8abb6b1-daf1-4093-a4f0-41b4eb8686e7");
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/92011-62b15cce-c248-426d-a011-9bc5e6737a6e");
                    //AVG COFFEE SERVICE TIME

                    break;

                //london
                case 4:
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/54410-7f1c36a0-c590-0131-a663-22000a1e86ad");
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/94910-bccfc8d3-97ea-45a9-bad8-0cd99c5312f3");
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/54410-7f1e7220-c590-0131-a664-22000a1e86ad");
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/94910-99192115-59e0-49bb-8c23-8479bc02a30a");
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/54410-7f20c320-c590-0131-a665-22000a1e86ad");
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/54410-7f280600-c590-0131-a669-22000a1e86ad");
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/54410-7f150ae0-c590-0131-a65f-22000a1e86ad");
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/54410-7f244170-c590-0131-a667-22000a1e86ad");
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/54410-7f29a260-c590-0131-a66a-22000a1e86ad");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/94910-40db97e3-43ea-44c5-82d6-a8e3bf49074d");
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/94910-076350b8-c549-49fd-9600-937098c67f7f");
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/94910-7d8a2c35-9eaf-4bb4-87f8-bda34b68ff77");
                    //WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/94910-56267203-ced2-4145-8173-54507806f371");
                    //breakfast
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/94910-ff5b123a-a09d-4068-ad41-8daa91968bf3");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/94910-5feebdd9-7cde-4911-bb90-a4e8e1b1633b");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/94910-dc358b81-45b3-4fe7-bb83-1bdaba27076f");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/94910-b7dbafaf-37c2-4bc7-ad26-4a2bef6cf6da");

                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/54410-7f11b010-c590-0131-a65d-22000a1e86ad");
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/54410-7f1085a0-c590-0131-a65c-22000a1e86ad");
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/54410-7f13d5a0-c590-0131-a65e-22000a1e86ad");
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/94910-4f80f70a-b9c0-4841-a61e-5daacc6147f1");
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/54410-7f1aec20-c590-0131-a662-22000a1e86ad");
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/54410-7f3006f0-c590-0131-a66d-22000a1e86ad");
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/94910-19f1435e-c976-4fbd-8f3d-67eadfa4ea69");
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/94910-3cf293fc-6760-4c2e-9238-b44a377e8576");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/94910-402f8d71-4d92-4f61-938d-e2d041db645d");
                    break;

                //holborn
                case 5:
                    //today vs same day last  week -1                
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/54410-d818dac0-2c8f-0132-4c4a-22000b5e86d6");
                    //avg spend -2 
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/111794-886634cc-3c1b-46f2-954a-932f85503c62");
                    //no of orders today -3
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/54410-d819b890-2c8f-0132-4c4b-22000b5e86d6");
                    //Booze sales - 4
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/54410-d81f31f0-2c8f-0132-4c4f-22000b5e86d6");
                    //hot drink NUMBER -5
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/54410-d81a6f60-2c8f-0132-4c4c-22000b5e86d6");
                    //FOOD - 6
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/54410-d8200960-2c8f-0132-4c50-22000b5e86d6");
                    //LAST UPDATED - 7
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/54410-d814e3f0-2c8f-0132-4c46-22000b5e86d6");
                    //NO OF SOFT DRINKS - 8
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/54410-d81e48e0-2c8f-0132-4c4e-22000b5e86d6");
                    //HOUR AND SPEND
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/54410-d820f850-2c8f-0132-4c51-22000b5e86d6");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/111794-d70fdde1-5838-4f4d-b54d-e7d25dfb8d52");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/111794-9925086e-4f17-436d-b955-0aca82311b63");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/111794-c7a95a0b-c477-49df-8987-36b0a4f9a08a");
                    //WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/111794-7eeecb1b-ece0-4731-991b-6f0cbbb4a7f5");
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/111794-9d09728e-5635-4592-b658-385d387b3f21");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/111794-bb8c00dd-dd1f-40c2-a248-fdd5cf172060");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/111794-f4e4f1d7-d229-4646-a8fe-bc5fe6f4527b");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/111794-785f9b3f-3d90-4921-9e80-585db07e32c9");

                    //LastWeekNetVSweekBeforeGross
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/54410-d8131210-2c8f-0132-4c44-22000b5e86d6");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/54410-d8121a00-2c8f-0132-4c43-22000b5e86d6");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/54410-d81413e0-2c8f-0132-4c45-22000b5e86d6");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/111794-0aaf8b20-cba0-41f2-b78b-b1bc3715a687");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/54410-d817ae00-2c8f-0132-4c49-22000b5e86d6");
                    //last 30 days 
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/54410-d8228870-2c8f-0132-4c53-22000b5e86d6");
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/111794-21f1e24f-91a1-45cc-9f66-70a54ae10afe");
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/111794-ddf76986-0d1b-48f1-8613-78f875a86a31");
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/111794-9f77da06-4cc3-48d3-bfc7-1038547344db");
                    break;



                //royal exchange
                case 6:
                    //today vs same day last  week -1                
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/-9709ce30-f9ee-0133-13da-22000bf8a2ac");
                    //avg spend -2 
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/-9722f470-f9ee-0133-13eb-22000bf8a2ac");
                    //no of orders today -3
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/-9711ce00-f9ee-0133-13e0-22000bf8a2ac");
                    //Booze sales - 4
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/-9715a8d0-f9ee-0133-13e3-22000bf8a2ac");
                    //hot drink NUMBER -5
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/-97133470-f9ee-0133-13e1-22000bf8a2ac");
                    //FOOD - 6
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/-9716f430-f9ee-0133-13e4-22000bf8a2ac");
                    //LAST UPDATED - 7
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/-971055f0-f9ee-0133-13df-22000bf8a2ac");
                    //NO OF SOFT DRINKS - 8
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/-9714a180-f9ee-0133-13e2-22000bf8a2ac");
                    //HOUR AND SPEND
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/-971ab400-f9ee-0133-13e5-22000bf8a2ac");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/-971e8ee0-f9ee-0133-13e8-22000bf8a2ac");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/-9720f970-f9ee-0133-13ea-22000bf8a2ac");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/-9723dfe0-f9ee-0133-13ec-22000bf8a2ac");
                    //WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/51912-972c8680-f9ee-0133-13f2-22000bf8a2ac");
                    //breakfast
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/-97254830-f9ee-0133-13ed-22000bf8a2ac");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/-9726ac00-f9ee-0133-13ee-22000bf8a2ac");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/-9727c3c0-f9ee-0133-13ef-22000bf8a2ac");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/-972b0f50-f9ee-0133-13f1-22000bf8a2ac");

                    //LastWeekNetVSweekBeforeGross
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/-970dc790-f9ee-0133-13dd-22000bf8a2ac");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/-970c5400-f9ee-0133-13dc-22000bf8a2ac");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/-970f27d0-f9ee-0133-13de-22000bf8a2ac");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/51912-972c8680-f9ee-0133-13f2-22000bf8a2ac");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/-971bff30-f9ee-0133-13e6-22000bf8a2ac");
                    //last 30 days 
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/-971d4bb0-f9ee-0133-13e7-22000bf8a2ac");
                    //Rolling 7 days PIT vs Previous Period
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/-971fa1a0-f9ee-0133-13e9-22000bf8a2ac");
                    //weekly sales by category
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/-9729fb40-f9ee-0133-13f0-22000bf8a2ac");
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/189362-89fa5b16-d75f-4c83-b21f-5abb0a46d72f");
                    break;


                //covent
                case 7:
                    //today vs same day last  week -1                
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/54410-020900d0-57f8-0133-2038-22000b4a0396");
                    //avg spend -2 
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/54410-021a0890-57f8-0133-2049-22000b4a0396");
                    //no of orders today -3
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/54410-020e8d50-57f8-0133-203e-22000b4a0396");
                    //Booze sales - 4
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/54410-0210e010-57f8-0133-2041-22000b4a0396");
                    //hot drink NUMBER -5
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/54410-020f4450-57f8-0133-203f-22000b4a0396");
                    //FOOD - 6
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/54410-0211afe0-57f8-0133-2042-22000b4a0396");
                    //LAST UPDATED - 7
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/54410-020db370-57f8-0133-203d-22000b4a0396");
                    //NO OF SOFT DRINKS - 8
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/54410-02101a70-57f8-0133-2040-22000b4a0396");
                    //HOUR AND SPEND
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/54410-02128650-57f8-0133-2043-22000b4a0396");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/54410-0216adf0-57f8-0133-2046-22000b4a0396");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/54410-0218f1c0-57f8-0133-2048-22000b4a0396");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/54410-021af450-57f8-0133-204a-22000b4a0396");
                    //WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/54410-021bdbb0-57f8-0133-204b-22000b4a0396");
                    //breakfast
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/54410-021cd110-57f8-0133-204c-22000b4a0396");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/54410-021ea600-57f8-0133-204d-22000b4a0396");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/54410-021fa740-57f8-0133-204e-22000b4a0396");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/54410-02216310-57f8-0133-2050-22000b4a0396");

                    //LastWeekNetVSweekBeforeGross
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/54410-020b36a0-57f8-0133-203b-22000b4a0396");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/54410-020a8330-57f8-0133-203a-22000b4a0396");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/54410-020be610-57f8-0133-203c-22000b4a0396");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/163867-c8975889-ed22-43ee-8c12-953a71a939a9");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/54410-0214a340-57f8-0133-2044-22000b4a0396");
                    //last 30 days 
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/54410-021593e0-57f8-0133-2045-22000b4a0396");
                    //Rolling 7 days PIT vs Previous Period
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/54410-0217e910-57f8-0133-2047-22000b4a0396");
                    //weekly sales by category
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/54410-022087d0-57f8-0133-204f-22000b4a0396");
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/163867-f24adfcc-ca0a-4179-a46f-b92d579e257c");
                    break;


                //radio??
                case 8:
                    //today vs same day last  week -1                
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/54410-87439ad0-57f9-0133-efdc-22000b7b85e6");
                    //avg spend -2 
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/54410-87534120-57f9-0133-efed-22000b7b85e6");
                    //no of orders today -3
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/54410-8749d6b0-57f9-0133-efe2-22000b7b85e6");
                    //Booze sales - 4
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/54410-874c4ab0-57f9-0133-efe5-22000b7b85e6");
                    //hot drink NUMBER -5
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/54410-874ab090-57f9-0133-efe3-22000b7b85e6");
                    //FOOD - 6
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/54410-874d2360-57f9-0133-efe6-22000b7b85e6");
                    //LAST UPDATED - 7
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/54410-87492190-57f9-0133-efe1-22000b7b85e6");
                    //NO OF SOFT DRINKS - 8
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/54410-874b8af0-57f9-0133-efe4-22000b7b85e6");
                    //HOUR AND SPEND
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/54410-874dde80-57f9-0133-efe7-22000b7b85e6");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/54410-87507d50-57f9-0133-efea-22000b7b85e6");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/54410-87528870-57f9-0133-efec-22000b7b85e6");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/54410-87541bb0-57f9-0133-efee-22000b7b85e6");
                    //?
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/54410-8754d3f0-57f9-0133-efef-22000b7b85e6");
                    //breaks
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/54410-87558760-57f9-0133-eff0-22000b7b85e6");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/54410-8756a6c0-57f9-0133-eff1-22000b7b85e6");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/54410-87578d50-57f9-0133-eff2-22000b7b85e6");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/54410-875923d0-57f9-0133-eff4-22000b7b85e6");

                    //LastWeekNetVSweekBeforeGross
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/54410-87470ea0-57f9-0133-efdf-22000b7b85e6");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/54410-8745cd40-57f9-0133-efde-22000b7b85e6");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/54410-87481040-57f9-0133-efe0-22000b7b85e6");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/163870-0400f762-7b3a-4af1-bc9f-af6a504b004b");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/54410-874eb5b0-57f9-0133-efe8-22000b7b85e6");
                    //last 30 days 
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/54410-874f7d90-57f9-0133-efe9-22000b7b85e6");
                    //Rolling 7 days PIT vs Previous Period
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/54410-8751a8a0-57f9-0133-efeb-22000b7b85e6");
                    //weekly sales by category
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/54410-87586a90-57f9-0133-eff3-22000b7b85e6");
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/163870-ee294261-241e-461e-9b6f-37e77d993779");

                    break;


                case 9:
                    //today vs same day last  week -1                
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/-8b289190-dda6-0134-e527-22000b5980c2");
                    //avg spend -2 
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/-8b367c90-dda6-0134-e538-22000b5980c2");
                    //no of orders today -3
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/-8b2d8890-dda6-0134-e52d-22000b5980c2");
                    //Booze sales - 4
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/-8b2ffaa0-dda6-0134-e530-22000b5980c2");
                    //hot drink NUMBER -5
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/-8b2e62e0-dda6-0134-e52e-22000b5980c2");
                    //FOOD - 6
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/-8b30ba40-dda6-0134-e531-22000b5980c2");
                    //LAST UPDATED - 7
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/-8b2c7570-dda6-0134-e52c-22000b5980c2");
                    //NO OF SOFT DRINKS - 8
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/-8b2f35f0-dda6-0134-e52f-22000b5980c2");
                    //HOUR AND SPEND
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/-8b317850-dda6-0134-e532-22000b5980c2");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/-8b342780-dda6-0134-e535-22000b5980c2");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/-8b35b810-dda6-0134-e537-22000b5980c2");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/-8b373b90-dda6-0134-e539-22000b5980c2");
                    ///WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/51912-8b3c49d0-dda6-0134-e53f-22000b5980c2");
                    //breaks
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/-8b380e60-dda6-0134-e53a-22000b5980c2");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/-8b38ddf0-dda6-0134-e53b-22000b5980c2");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/-8b39c660-dda6-0134-e53c-22000b5980c2");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/-8b3b7c40-dda6-0134-e53e-22000b5980c2");

                    //LastWeekNetVSweekBeforeGross
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/-8b2ae7a0-dda6-0134-e52a-22000b5980c2");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/-8b2a1f70-dda6-0134-e529-22000b5980c2");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/-8b2baa40-dda6-0134-e52b-22000b5980c2");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/51912-8b3c49d0-dda6-0134-e53f-22000b5980c2");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/-8b324d10-dda6-0134-e533-22000b5980c2");
                    //last 30 days 
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/-8b332520-dda6-0134-e534-22000b5980c2");
                    //Rolling 7 days PIT vs Previous Period
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/-8b34ebb0-dda6-0134-e536-22000b5980c2");
                    //weekly sales by category
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/-8b3aac30-dda6-0134-e53d-22000b5980c2");
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/");

                    break;





                case 10:
                    //today vs same day last  week -1                
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/-f5e41220-c2fd-0134-bd2e-22000b5980c2");
                    //avg spend -2 
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/-f5f25ba0-c2fd-0134-bd3f-22000b5980c2");
                    //no of orders today -3
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/-f5e8bff0-c2fd-0134-bd34-22000b5980c2");
                    //Booze sales - 4
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/-f5eb1790-c2fd-0134-bd37-22000b5980c2");
                    //hot drink NUMBER -5
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/-f5e98380-c2fd-0134-bd35-22000b5980c2");
                    //FOOD - 6
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/-f5ebeac0-c2fd-0134-bd38-22000b5980c2");
                    //LAST UPDATED - 7
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/-f5e7f5c0-c2fd-0134-bd33-22000b5980c2");
                    //NO OF SOFT DRINKS - 8
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/-f5ea45a0-c2fd-0134-bd36-22000b5980c2");
                    //HOUR AND SPEND
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/-f5ecf550-c2fd-0134-bd39-22000b5980c2");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/-f5effca0-c2fd-0134-bd3c-22000b5980c2");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/-f5f18ef0-c2fd-0134-bd3e-22000b5980c2");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/-f5f35f50-c2fd-0134-bd40-22000b5980c2");
                    ///WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/51912-f5f88240-c2fd-0134-bd46-22000b5980c2");
                    //breaks
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/-f5f46740-c2fd-0134-bd41-22000b5980c2");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/-f5f55340-c2fd-0134-bd42-22000b5980c2");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/-f5f621b0-c2fd-0134-bd43-22000b5980c2");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/-f5f7be20-c2fd-0134-bd45-22000b5980c2");

                    //LastWeekNetVSweekBeforeGross
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/-f5e65aa0-c2fd-0134-bd31-22000b5980c2");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/-f5e598b0-c2fd-0134-bd30-22000b5980c2");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/-f5e71f70-c2fd-0134-bd32-22000b5980c2");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/-f5ee0d20-c2fd-0134-bd3a-22000b5980c2");
                    //last 30 days 
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/-f5eed7c0-c2fd-0134-bd3b-22000b5980c2");
                    //Rolling 7 days PIT vs Previous Period
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/-f5f0c850-c2fd-0134-bd3d-22000b5980c2");
                    //weekly sales by category
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/-f5f6f420-c2fd-0134-bd44-22000b5980c2");
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/");

                    break;

                case 11:
                    //today vs same day last  week -1                
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/-ccaa4a50-3c1e-0136-4c58-22000b9d0561");
                    //avg spend -2 
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/-ccb7c560-3c1e-0136-4c62-22000b9d0561");
                    //no of orders today -3
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/-ccabc480-3c1e-0136-4c59-22000b9d0561");
                    //Booze sales - 4
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/-ccafb260-3c1e-0136-4c5c-22000b9d0561");
                    //hot drink NUMBER -5
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/-ccad2690-3c1e-0136-4c5a-22000b9d0561");
                    //FOOD - 6
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/-ccb10500-3c1e-0136-4c5d-22000b9d0561");
                    //LAST UPDATED - 7
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/-cca0dd90-3c1e-0136-4c54-22000b9d0561");
                    //NO OF SOFT DRINKS - 8
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/-ccae6ca0-3c1e-0136-4c5b-22000b9d0561");
                    //HOUR AND SPEND
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/-ccb24f00-3c1e-0136-4c5e-22000b9d0561");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/-ccb3a240-3c1e-0136-4c5f-22000b9d0561");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/-ccb65100-3c1e-0136-4c61-22000b9d0561");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/-ccb91ef0-3c1e-0136-4c63-22000b9d0561");
                    ///WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/51912-ccc14f30-3c1e-0136-4c69-22000b9d0561");
                    //breaks
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/-ccba9900-3c1e-0136-4c64-22000b9d0561");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/-ccbbf2d0-3c1e-0136-4c65-22000b9d0561");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/51912-ccc2b280-3c1e-0136-4c6a-22000b9d0561");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/51912-ccc2b280-3c1e-0136-4c6a-22000b9d0561");



                    //LastWeekNetVSweekBeforeGross
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/-cc9e3ae0-3c1e-0136-4c52-22000b9d0561");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/-cc9c82f0-3c1e-0136-4c51-22000b9d0561");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/-cc9f9220-3c1e-0136-4c53-22000b9d0561");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/-cca8e6e0-3c1e-0136-4c57-22000b9d0561");
                    //last 30 days 
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/");
                    //Rolling 7 days PIT vs Previous Period
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/-ccb50080-3c1e-0136-4c60-22000b9d0561");
                    //weekly sales by category
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/-ccbe9610-3c1e-0136-4c67-22000b9d0561");
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/");

                    break;
                    
                case 13:
                    //today vs same day last  week -1                
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/-22ad7de0-c983-0136-7564-02ad66dbe3cc");
                    //avg spend -2 
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/-22b6f460-c983-0136-756e-02ad66dbe3cc");
                    //no of orders today -3
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/-22ae71a0-c983-0136-7565-02ad66dbe3cc");
                    //Booze sales - 4
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/-22b18920-c983-0136-7568-02ad66dbe3cc");
                    //hot drink NUMBER -5
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/-22af7c00-c983-0136-7566-02ad66dbe3cc");
                    //FOOD - 6
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/-22b26750-c983-0136-7569-02ad66dbe3cc");
                    //LAST UPDATED - 7
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/-22a968c0-c983-0136-7560-02ad66dbe3cc");
                    //NO OF SOFT DRINKS - 8
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/-22b0a650-c983-0136-7567-02ad66dbe3cc");
                    //HOUR AND SPEND
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/-22b35900-c983-0136-756a-02ad66dbe3cc");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/-22b44250-c983-0136-756b-02ad66dbe3cc");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/-22b60370-c983-0136-756d-02ad66dbe3cc");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/-22b81e80-c983-0136-756f-02ad66dbe3cc");
                    ///WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/51912-22beb9c0-c983-0136-7575-02ad66dbe3cc");
                    //breaks
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/-22b989c0-c983-0136-7570-02ad66dbe3cc");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/-22baf670-c983-0136-7571-02ad66dbe3cc");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/-22bc0350-c983-0136-7572-02ad66dbe3cc");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/-22bdd470-c983-0136-7574-02ad66dbe3cc");
                    

                    //LastWeekNetVSweekBeforeGross
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/-22a7abe0-c983-0136-755e-02ad66dbe3cc");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/-22a6baa0-c983-0136-755d-02ad66dbe3cc");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/-22a88ae0-c983-0136-755f-02ad66dbe3cc");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/-22ac70b0-c983-0136-7563-02ad66dbe3cc");
                    //last 30 days 
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/");
                    //Rolling 7 days PIT vs Previous Period
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/-22b520b0-c983-0136-756c-02ad66dbe3cc");
                    //weekly sales by category
                    widgetBindMappings.Add(108, "");
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/");

                    break;

                case 14:
                    //today vs same day last  week -1                
                    widgetBindMappings.Add(1, "https://push.geckoboard.com/v1/send/-59958a70-3a87-0137-6bfe-0ee70790bb7e");
                    //avg spend -2 
                    widgetBindMappings.Add(2, "https://push.geckoboard.com/v1/send/-59a06320-3a87-0137-6c08-0ee70790bb7e");
                    //no of orders today -3
                    widgetBindMappings.Add(3, "https://push.geckoboard.com/v1/send/-59967b40-3a87-0137-6bff-0ee70790bb7e");
                    //Booze sales - 4
                    widgetBindMappings.Add(4, "https://push.geckoboard.com/v1/send/-5999dd00-3a87-0137-6c02-0ee70790bb7e");
                    //hot drink NUMBER -5
                    widgetBindMappings.Add(5, "https://push.geckoboard.com/v1/send/-59979b10-3a87-0137-6c00-0ee70790bb7e");
                    //FOOD - 6
                    widgetBindMappings.Add(6, "https://push.geckoboard.com/v1/send/-599aea00-3a87-0137-6c03-0ee70790bb7e");
                    //LAST UPDATED - 7
                    widgetBindMappings.Add(7, "https://push.geckoboard.com/v1/send/-59919a90-3a87-0137-6bfa-0ee70790bb7e");
                    //NO OF SOFT DRINKS - 8
                    widgetBindMappings.Add(8, "https://push.geckoboard.com/v1/send/-5998c320-3a87-0137-6c01-0ee70790bb7e");
                    //HOUR AND SPEND
                    widgetBindMappings.Add(9, "https://push.geckoboard.com/v1/send/-599bf570-3a87-0137-6c04-0ee70790bb7e");
                    //DISCOUNT
                    widgetBindMappings.Add(10, "https://push.geckoboard.com/v1/send/-599d1770-3a87-0137-6c05-0ee70790bb7e");
                    //AVG COFFEE SERVICE TIME
                    widgetBindMappings.Add(11, "https://push.geckoboard.com/v1/send/51912-59a8e090-3a87-0137-6c10-0ee70790bb7e");
                    //OPEN UNPAID ORDER (TAB)
                    widgetBindMappings.Add(12, "https://push.geckoboard.com/v1/send/-59a16600-3a87-0137-6c09-0ee70790bb7e");
                    ///WEEK VS BUDGET BULLET
                    widgetBindMappings.Add(13, "https://push.geckoboard.com/v1/send/51912-59a7f150-3a87-0137-6c0f-0ee70790bb7e");
                    //breaks
                    widgetBindMappings.Add(14, "https://push.geckoboard.com/v1/send/-59a257d0-3a87-0137-6c0a-0ee70790bb7e");
                    //lunch 
                    widgetBindMappings.Add(15, "https://push.geckoboard.com/v1/send/-59a35a00-3a87-0137-6c0b-0ee70790bb7e");
                    //dinner
                    widgetBindMappings.Add(16, "https://push.geckoboard.com/v1/send/-59a44d80-3a87-0137-6c0c-0ee70790bb7e");
                    //service charge
                    widgetBindMappings.Add(17, "https://push.geckoboard.com/v1/send/-59a6fca0-3a87-0137-6c0e-0ee70790bb7e");


                    //LastWeekNetVSweekBeforeGross
                    widgetBindMappings.Add(101, "https://push.geckoboard.com/v1/send/-598f6a00-3a87-0137-6bf8-0ee70790bb7e");
                    //yesterdayVSYesterdayLastWeek
                    widgetBindMappings.Add(102, "https://push.geckoboard.com/v1/send/-598e73d0-3a87-0137-6bf7-0ee70790bb7e");
                    //lastMonthVSBudget
                    widgetBindMappings.Add(103, "https://push.geckoboard.com/v1/send/-59908250-3a87-0137-6bf9-0ee70790bb7e");
                    //LAST MONTH VS LAST YEAR BUDGET BULLET CHART
                    widgetBindMappings.Add(104, "https://push.geckoboard.com/v1/send/");
                    //ThisYearStartToToday vs last year
                    widgetBindMappings.Add(105, "https://push.geckoboard.com/v1/send/-5994a2d0-3a87-0137-6bfd-0ee70790bb7e");
                    //last 30 days 
                    widgetBindMappings.Add(106, "https://push.geckoboard.com/v1/send/");
                    //Rolling 7 days PIT vs Previous Period
                    widgetBindMappings.Add(107, "https://push.geckoboard.com/v1/send/-599e1980-3a87-0137-6c06-0ee70790bb7e");
                    //weekly sales by category
                    widgetBindMappings.Add(108, "https://push.geckoboard.com/v1/send/-59a57d90-3a87-0137-6c0d-0ee70790bb7e");
                    widgetBindMappings.Add(109, "https://push.geckoboard.com/v1/send/");

                    break;

            }











        }

    }
}
