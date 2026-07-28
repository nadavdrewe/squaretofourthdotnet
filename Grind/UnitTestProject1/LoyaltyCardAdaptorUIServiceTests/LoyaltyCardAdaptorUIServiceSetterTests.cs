using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebReboot.Grind._808nd.com.LoyaltyCardUI.UICardAdaptor;
using Shouldly;
using Revel._808nd.com.Models;
using Revel._808nd.com.Classes;

namespace UnitTestProject1.LoyaltyCardAdaptorUIServiceTests
{
    [TestFixture]
    public class LoyaltyCardAdaptorUIServiceSetterTests
    {

        List<LoyaltyCardType> cardTypes;
        [SetUp]
        public void SetUp()
        {
            //some test data
            GrindContext db = new GrindContext("GrindLiveContext");
            cardTypes = db.LoyaltyCardTypes.ToList();

        }


        //Setter Tests


        //Red card tests
        [Test]
        public void Should_Set_As_Red_Card() //daily
        {
            var testRedCard = new RewardsCardNew
            {
                Active = false,
                is_vip_card = false,
                LoyaltyCardType = null
            };

            LoyaltyCardAdaptorUIService.SetUICardType(1, testRedCard, cardTypes);
            testRedCard.is_vip_card.ShouldBe(true);
            testRedCard.Active.ShouldBe(true);
            testRedCard.LoyaltyCardType.ShouldBe(null);

            LoyaltyCardAdaptorUIService.GetUICardType(testRedCard).ShouldBe(1);

        }

        [Test]
        public void Should_Set_As_Red_Card_When_Alredy_Loyalty_Card() //daily
        {
            var testRedCard = new RewardsCardNew
            {
                Active = false,
                is_vip_card = false,
                LoyaltyCardType = cardTypes.First()
            };

            LoyaltyCardAdaptorUIService.SetUICardType(1, testRedCard, cardTypes);
            testRedCard.is_vip_card.ShouldBe(true);
            testRedCard.Active.ShouldBe(true);
            testRedCard.LoyaltyCardType.ShouldBe(null);
            
            LoyaltyCardAdaptorUIService.GetUICardType(testRedCard).ShouldBe(1);
        }

        [Test]
        public void Should_Set_As_Weekly_Card()
        {
            var testRedCard = new RewardsCardNew
            {
                Active = false,
                is_vip_card = false,
                LoyaltyCardType = null
            };

            LoyaltyCardAdaptorUIService.SetUICardType(2, testRedCard, cardTypes);
            testRedCard.is_vip_card.ShouldBe(false);
            testRedCard.Active.ShouldBe(true);
            testRedCard.LoyaltyCardType.ShouldBe(cardTypes.First(x => x.id == 1));


            LoyaltyCardAdaptorUIService.GetUICardType(testRedCard).ShouldBe(2);

        }

        [Test]
        public void Should_Set_As_Monthly_Card()
        {
            var testRedCard = new RewardsCardNew
            {
                Active = false,
                is_vip_card = false,
                LoyaltyCardType = null
            };

            LoyaltyCardAdaptorUIService.SetUICardType(3, testRedCard, cardTypes);
            testRedCard.is_vip_card.ShouldBe(false);
            testRedCard.Active.ShouldBe(true);
            testRedCard.LoyaltyCardType.ShouldBe(cardTypes.First(x => x.id == 2));


            LoyaltyCardAdaptorUIService.GetUICardType(testRedCard).ShouldBe(3);

        }

        [Test]
        public void Should_Set_As_Card_Off()
        {
            var testRedCard = new RewardsCardNew
            {
                Active = false,
                is_vip_card = false,
                LoyaltyCardType = null
            };

            LoyaltyCardAdaptorUIService.SetUICardType(0, testRedCard, cardTypes);
            // testRedCard.is_vip_card.ShouldBe(false);
            testRedCard.Active.ShouldBe(false);
            //testRedCard.LoyaltyCardType.ShouldBe(cardTypes.First(x => x.id == 2));

            LoyaltyCardAdaptorUIService.GetUICardType(testRedCard).ShouldBe(0);

        }
    }
}
