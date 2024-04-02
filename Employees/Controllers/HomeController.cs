using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Employees.Models;
using System.Linq;

namespace Employees.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
{
   List<Employee> employees = await GetEmployeesFromApi();
Dictionary<string, int> map = new Dictionary<string, int>();

foreach (Employee e in employees)
{
    int totalHoursWorked = (int)(e.EndTime - e.StartTime).TotalHours;

   if (!string.IsNullOrEmpty(e.Name))
    {
        
        if (map.ContainsKey(e.Name))
        {
            map[e.Name] += totalHoursWorked;
        }
        else
        {
            map.Add(e.Name, totalHoursWorked);
        }
    }
}
var sortedEmployeeHoursList = map.ToList();

   
    sortedEmployeeHoursList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

   
    var sortedEmployeeHoursMap = new Dictionary<string, int>(sortedEmployeeHoursList);

    return View(sortedEmployeeHoursMap);

  //  return View(map);
}


        private async Task<List<Employee>> GetEmployeesFromApi()
        {
            List<Employee> employees = new List<Employee>();
            string uri = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage responseMessage = await httpClient.GetAsync(uri);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    employees = JsonConvert.DeserializeObject<List<Employee>>(data);
                }
            }

            return employees;
        }

      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}