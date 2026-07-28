using System;
using System.Threading.Tasks;
using Revel._808nd.com.Classes.WebserviceReader;

namespace Revel._808nd.com.Classes.FourthCreate
{
    public class CreateFourthRevelDBStackService
    {
        private string apiKeySecret { get; set; }
        private Uri rootURL { get; set; }
        private RevelDBWriter dbWriter { get; set; }
        private RevelWebserviceDataReader webReader { get; set; }
        private Establishment establishment { get; set; }
        
        public async Task<int>  CreateFourthRevelDBStack()
        {
            
            //new RevelOrganisation()
            //pull brand into DB,s
            try
            {
                var brand = new Brand();
                var webBrands = await webReader.GetRevelWebserviceData(brand,
                    brand.theAddress);

                foreach (var aBrand in webBrands)
                {
                 //assign DBparams for this org
                    aBrand.is_fourth_active = true;
                    aBrand.key_secret = apiKeySecret;
                    aBrand.revel_base_url = rootURL.ToString();

                }


                var saveOk = dbWriter.SaveRevelType(webBrands);

                Establishment establishment = this.establishment;

                var establishments = await webReader.GetRevelWebserviceData(establishment,
                    establishment.theAddress);

                foreach (var establishment1 in establishments)
                {
                    establishment1.is_fourth_active = true;
                    establishment1.RevelOrganiationName = brand.name;
                }

                var SaveEstOk = await dbWriter.SaveRevelType(establishments);

                

                var products = await webReader.GetProductsNoEstablishment();
                var saveProdsOk = await dbWriter.SaveRevelType(products);




            }
            catch (Exception ex)
            {
                
                throw new Exception("Unable to create Revel Brand", ex);
            }
            //pull establishments
            try
            {

            }
            catch (Exception ex)
            {

                throw new Exception("Unable to create Revel Establishments", ex);
            }
            //pull products
            try
            {

            }
            catch (Exception ex)
            {

                throw new Exception("Unable to create Revel Products", ex);
            }

            return 0;

        }

        public CreateFourthRevelDBStackService(Establishment est, RevelDBWriter writer)
        {
            webReader = new RevelWebserviceDataReader(est);
            dbWriter = writer;
            apiKeySecret = est.api_key;
            rootURL = est.BaseUri;
            establishment = est;

        }

    }
}
