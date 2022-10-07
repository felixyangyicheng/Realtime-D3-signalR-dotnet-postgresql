using Newtonsoft.Json;
using System.Text;

class Program
{
    static tbllog logModel = null;

    static string baseUrl = "http://realtime_d3_api:8080/";


    static void Main(string[] args)
        
    {
        Console.WriteLine("running on " + baseUrl);
        while (true)
        {
            try
            {
                logModel = Applog.getlogData();
                if (logModel != null)
                {
                    using (var client = new HttpClient())
                    {
                        string contentType = "application/json";
                        string PostUrl = baseUrl + "api/tbllog";
                        var content = new StringContent(JsonConvert.SerializeObject(logModel), Encoding.UTF8, contentType);
                        using (HttpResponseMessage response = client.PostAsync(PostUrl, content).Result)
                        {
                            if (response.IsSuccessStatusCode)
                                Console.WriteLine("{0}", logModel.Value);
                            else
                            {
                                Console.WriteLine("{0}", response);

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.Message );
            }

            Thread.Sleep(8000);
        }
    }
}

public class tbllog
{
    public int Id { get; set; }
    public int Value { get; set; }
    public string Detail { get; set; }
    public DateTime LogDate { get; set; }
}

static class Applog
{
    public static tbllog getlogData()
    {
        tbllog objdata = new tbllog()
        {
            Detail = "Operation-Code~" + Utilities.RandomNumber(1, 1000),
            Value = Utilities.RandomNumber(1,100),
            LogDate = DateTime.UtcNow
        };

        return objdata;
    }
}

static class Utilities
{
    public static int RandomNumber(int min, int max)
    {
        Random random = new Random();
        return random.Next(min, max);
    }
}