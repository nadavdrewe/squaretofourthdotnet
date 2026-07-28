using System;
using System.Collections.Generic;
using System.Text;

namespace data.pipeline.fourth.com.Interfaces.Public
{
    public interface IHaveBrandOwner
    {
        public int BrandId { get; set; }
    }

    public interface IMightHaveBrandOwner
    {
        public int? BrandId { get; set; }
    }
}
