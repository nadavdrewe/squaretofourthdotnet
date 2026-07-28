namespace Revel._808nd.com.Classes
{



   

    /// <summary>
    /// Holds the URL to send to, and the type of object that URL corresponds to
    /// </summary>
    public class RevelFactoryURL
    {

        public RevelFactoryURL(RevelObjectType type, string URL)
        {
            this.RevelObjectType = type;
            this.WebserviceURL = URL;
        }

        public RevelObjectType RevelObjectType
        {get;set;}
        public string WebserviceURL 
        {get; set;}
       

    }
}
