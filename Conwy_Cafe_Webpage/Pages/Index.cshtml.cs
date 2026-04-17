using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Conwy_Cafe_Webpage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
        //}

        public void OnGet()
        {

        }

        // Trial run for calculations
        private readonly HttpClient _http;

        // Stores the answer to show on the page
        public string DisplayResult { get; set; }
        public IndexModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
        }

        // This is called when the form is submitted (POST)
        //public async Task OnPostAsync(double num1, double num2)
        //{
        //    // 1. Point to your API's specific address
        //    // Check your API's port number (e.g., 5001 or 7000)
        //    string url = $"https://localhost:7008/api/calculator/add?a={num1}&b={num2}";

        //    // 2. Make the call and wait for the response
        //    var response = await _http.GetAsync(url);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // 3. Read the JSON result
        //        var data = await response.Content.ReadFromJsonAsync<CalcResult>();
        //        DisplayResult = $"The API says: {data.Answer}";
        //    }
        //}
        //// Simple class to catch the API response
        //public class CalcResult { public double Answer { get; set; } }
    }
}
