using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebReboot.Grind._808nd.com.LoyaltyCardUI.UICardAdaptor;
using Shouldly;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;

namespace UnitTestProject1.LoyaltyCardAdaptorUIServiceTests
{
    [TestFixture]
    public class LoyaltyCardAdaptorUIServiceGetterTests
    {

        List<LoyaltyCardType> cardTypes;

        [SetUp]
        public void SetUp()
        {
            //some test data
            GrindContext db = new GrindContext("GrindLiveContext");
            cardTypes = db.LoyaltyCardTypes.ToList();
        }

        //Getter Tests
        [Test]
        public void Should_Give_Red_Card_Result()
        {
            var testRedCard = new RewardsCardNew
            {
                Active = true,
                is_vip_card = true,
                LoyaltyCardType = null
            };

            var result = LoyaltyCardAdaptorUIService.GetUICardType(testRedCard);
            result.ShouldBe(1);
        }

        [Test]
        public void Should_Give_Zero_Card_Result()
        {
            var testZeroCard = new RewardsCardNew
            {
                Active = false,
                is_vip_card = true,
                LoyaltyCardType = cardTypes.First(x=>x.id == 1)
            };

            var result = LoyaltyCardAdaptorUIService.GetUICardType(testZeroCard);
            result.ShouldBe(0);
        }


        //LOTALTY CARDS
        [Test]
        public void Should_Give_Correct_Loyalty_Card_Result_Given_Type_7()
        {
            var testZeroCard = new RewardsCardNew
            {
                Active = true,
                is_vip_card = false,
                LoyaltyCardType = cardTypes.First(x => x.id == 7)
            };

            var result = LoyaltyCardAdaptorUIService.GetUICardType(testZeroCard);
            result.ShouldBe(2);
        }

        [Test]
        public void Should_Give_Correct_Loyalty_Card_Result_Given_Type_5()
        {
            var testZeroCard = new RewardsCardNew
            {
                Active = true,
                is_vip_card = false,
                LoyaltyCardType = cardTypes.First(x => x.id == 5)
            };

            var result = LoyaltyCardAdaptorUIService.GetUICardType(testZeroCard);
            result.ShouldBe(1);
        }
        [Test]
        public void Should_Give_Correct_Loyalty_Card_Result_Given_Type_4()
        {
            var testZeroCard = new RewardsCardNew
            {
                Active = true,
                is_vip_card = false,
                LoyaltyCardType = cardTypes.First(x => x.id == 4)
            };

            var result = LoyaltyCardAdaptorUIService.GetUICardType(testZeroCard);
            result.ShouldBe(0);
        }
        [Test]
        public void Should_Give_Correct_Loyalty_Card_Result_Given_Type_2()
        {
            var testZeroCard = new RewardsCardNew
            {
                Active = true,
                is_vip_card = false,
                LoyaltyCardType = cardTypes.First(x => x.id == 2)
            };

            var result = LoyaltyCardAdaptorUIService.GetUICardType(testZeroCard);
            result.ShouldBe(3);
        }
        [Test]
        public void Should_Give_Correct_Loyalty_Card_Result_Given_Type_1()
        {
            var testZeroCard = new RewardsCardNew
            {
                Active = true,
                is_vip_card = false,
                LoyaltyCardType = cardTypes.First(x => x.id == 1)
            };

            var result = LoyaltyCardAdaptorUIService.GetUICardType(testZeroCard);
            result.ShouldBe(2);
        }

    }
}
