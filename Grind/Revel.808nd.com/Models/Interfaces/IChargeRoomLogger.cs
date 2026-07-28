using Revel._808nd.com.Classes;
using System.Data.Entity;

namespace Revel._808nd.com.Interfaces
{
    public interface IChargeRoomLogger
    {
        DbSet<ChargeRoomOrderItemLog> ChargeRoomOrderItemLogs { get; set; }
        DbSet<ChargeRoomOrderLog> ChargeRoomOrderLogs { get; set; }


    }
}