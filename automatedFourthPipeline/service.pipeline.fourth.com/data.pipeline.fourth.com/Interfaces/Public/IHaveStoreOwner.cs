using System;
using System.Collections.Generic;
using System.Text;

namespace data.pipeline.fourth.com.Interfaces.Public
{
    public interface IHaveStoreOwner
    {
        public int StoreId { get; set; }
    }

    public interface IMightHaveStoreOwner
    {
        public int? StoreId { get; set; }
    }
}
