using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace shared.services.grind.railgunit.com.DataShaping
{
    public static class DataShaping
    {

        public static object CreateDataShapedObject<T>(T theObjectToShape, List<string> listOfFields)
        {

            if (!listOfFields.Any() || listOfFields == null)
            {
                return theObjectToShape;
            }

            ExpandoObject objectToReturn = new ExpandoObject();

            foreach (var field in listOfFields)
            {
                var fieldsValue = theObjectToShape.GetType()
                    .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                    .GetValue(theObjectToShape, null);

                ((IDictionary<string, object>)objectToReturn).Add(field, fieldsValue);

            }

            return objectToReturn;
        }

    }

}
