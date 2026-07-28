using System;
using System.Collections.Generic;
using System.Text;
using Square;

namespace domain.pipeline.fourth.com.Square.SalesFactories.Helper
{
    public static class NullObjectHelper
    {
        public static TeamMember CreateNullEmployee()
        {
            return new TeamMember
            {
                EmailAddress = "",
                GivenName = "",
                FamilyName = "",
                Id = ""
            };
        }
    }
}
