using Revel._808nd.com.Classes;

namespace Revel._808nd.com.DTO.Factory
{
    public class CardCustomerFactory
    {

        public CardCustomer Create(RewardsCardNew card, Customer customer)
        {
            var DTO = new CardCustomer();

            if (card == null)
            {
                DTO.Card = new RewardsCardNew
                {
                    number = "0",
                    created_by = "Sorry your card did not exist",
                    theAddress = "Sorry your card did not exist",                    
                    current_points = 0,
                    total_points = 0,
                    customer_revel = "Sorry your card did not exist",
                };
            }
            else
            {
                DTO.Card = card;
            }

            if (customer == null)
            {
                DTO.Customer = null;
            }
            else
            {
                DTO.Customer = customer;
            }


            return DTO;


        }

    }
}
