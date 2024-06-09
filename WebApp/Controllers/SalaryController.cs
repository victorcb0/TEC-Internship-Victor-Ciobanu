using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Humanizer;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using WebApp.Settings;
using Microsoft.Extensions.Options;

namespace WebApp.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ApiSettings _apiSettings;

        public SalaryController(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            List<Salary> list = new List<Salary>();
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync($"{_apiSettings.ApiUrl}/Salaries");
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<Salary>>(jstring);
                return View(list);
            }
            else
                return View(list);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.DeleteAsync($"{_apiSettings.ApiUrl}/Salaries/" + Id);
            if (message.IsSuccessStatusCode)
                return RedirectToAction("Index");
            else
                return View();
        }

        // Cod adăugat - Sarcina 4 - As a User I need to be able to add a new Salary
        public IActionResult Add()
        {
            Salary salary = new Salary();
            return View(salary);
        }

        // Cod adăugat - Sarcina 4 - As a User I need to be able to add a new Salary
        [HttpPost]
        public async Task<IActionResult> Add(Salary salary)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                var jsonSalary = JsonConvert.SerializeObject(salary);
                StringContent content = new StringContent(jsonSalary, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync($"{_apiSettings.ApiUrl}/Salaries", content);
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "There is an API Error");
                    return View(salary);
                }
            }
            else
            {
                return View(salary);
            }
        }
    }
}
