using Revel._808nd.com.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebReboot.Grind._808nd.com.LoyaltyCardUI.UICardAdaptor
{

    /// <summary>
    /// Returns one of the UI based options - 
    /// 0 - Off / None
    /// 1 - Red Card / Daily Refresh
    /// 2 - Weekly 
    /// 3 - Monthly
    /// </summary>
    public static class LoyaltyCardAdaptorUIService
    {

        public static string MapUICardTypeToOptionText(int typeId)
        {

            switch (typeId)
            {
                case 0:
                    return "Off";
                case 1:
                    return "Daily";
                case 2:
                    return "Weekly";
                case 3:
                    return "Monthly";
            }

            throw new Exception(String.Format("Couldn't identify which card type this is - MapUiCardTypeToOptionText: TypeID: {0}", typeId));
        }

        public static int GetUICardType(RewardsCardNew card)
        {
           
            try
            {
                var active = card.Active ?? false;
                var redCard = card.is_vip_card;
                var loyaltyCardType = card.LoyaltyCardType?.id;



                //if it's inactive it's off automatically
                if (!active)
                {
                    return 0;
                }

                //this is a mistake - disable it
                if (active && redCard == null && loyaltyCardType == null)
                {
                    return 0;
                }

                //this is a mistake - disable it
                if (active && redCard == false && loyaltyCardType == null)
                {
                    return 0;
                }


                switch (redCard)
                {
                    case true:
                        return 1;
                }

                switch (loyaltyCardType)
                {
                    //weekly
                    case 1: return 2;
                    case 2: return 3;
                    case 4: return 0;
                    case 5: return 1;
                    case 7: return 2;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Couldn't identify which card type this is - GetUICardType: Card Number: {0}", card.number), ex);
            }

            throw new Exception(String.Format("Couldn't identify which card type this is - GetUICardType: Card Number: {0}", card.number));

        }

        public static void SetUICardType(int typeID, RewardsCardNew cardToSet, IEnumerable<LoyaltyCardType> loyaltyCardTypes)
        {
            switch (typeID)
            {
                //just set inactive - leave whatever settings in place
                case 0:
                    cardToSet.Active = false;
                    return;
                //daily card - set as red card, set to active - wipe other settings
                case 1:
                    cardToSet.Active = true;
                    cardToSet.is_vip_card = true;
                    cardToSet.LoyaltyCardType = null;
                    return;
                //weekly 
                case 2:
                    cardToSet.Active = true;
                    cardToSet.is_vip_card = false;
                    cardToSet.LoyaltyCardType = loyaltyCardTypes.FirstOrDefault(x => x.id == 7);
                    return;
                //monthly
                case 3:
                    cardToSet.Active = true;
                    cardToSet.is_vip_card = false;
                    cardToSet.LoyaltyCardType = loyaltyCardTypes.FirstOrDefault(x => x.id == 2);
                    return;
                default:
                    break;
            }


            throw new Exception("Couldn't identify which card type this is - SetUICardType");

        }

    }
}