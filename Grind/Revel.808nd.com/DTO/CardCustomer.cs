using Revel._808nd.com.Classes;

namespace Revel._808nd.com.DTO
{
    public class CardCustomer
    {
        public RewardsCardNew Card { get;  set; }
        public Customer Customer { get;  set; }


        public CardCustomer(RewardsCardNew card, Customer customer)
        {
            Card = card;
            Customer = customer;
            
        }

        public CardCustomer()
        {
                
        }
    }
}
