using Newtonsoft.Json;
namespace ECC_Practice;
using System;

public class PointObject{
    public string x { get; set; }
    public string y { get; set; }
}

public class APITesting
{
    public async void GET()
    {
        using (var client = new HttpClient())
        {
            var response = client.GetAsync("https://localhost:7277/publicKey").Result;
            response.EnsureSuccessStatusCode();
            string body = await response.Content.ReadAsStringAsync();
            var point = JsonConvert.DeserializeObject<PointObject>(body);
            Console.WriteLine("[+] GET: {"+point.x+" , "+point.y+"}");
        }
    }
    public async void POST(string x, string y)
    {
        using (var client = new HttpClient())
        {
            try
            {
                var json = JsonConvert.SerializeObject(new PointObject {x = x, y = y});
                var content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var url = "https://localhost:7277/sessionKey";
                var response = client.PostAsync(url, content).Result;
                response.EnsureSuccessStatusCode();

                string body = await response.Content.ReadAsStringAsync();
                var responsePoint = JsonConvert.DeserializeObject<PointObject>(body);
                Console.WriteLine("[+] POSTT: {"+responsePoint.x+" , "+responsePoint.y+"}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }
    }
}