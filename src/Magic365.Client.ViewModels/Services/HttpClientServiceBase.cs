namespace Magic365.Client.ViewModels.Services;

public class HttpClientServiceBase
{
    /* COMMENT THE FOLLWOING LINE AND UNCOMMENT THE AFTER TO USE THE LOCAL API IN DEBUG MODE INSTEAD OF THE ONLINE VERSION */
    //protected const string BaseUrl = "https://Magic365.azurewebsites.net";

    /* UNCOMMENT THIS FOR LOCAL TESTING */
    protected const string BaseUrl = "https://localhost:7210";
}
