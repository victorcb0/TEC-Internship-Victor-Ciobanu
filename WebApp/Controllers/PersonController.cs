using WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebApp.Settings;
using Microsoft.Extensions.Options;

namespace WebApp.Controllers
{
    public class PersonController : Controller
    {
        //HINT task 8 start

        /*        private readonly IConfiguration _config;
                private readonly string _api;
                public PersonController(IConfiguration config)
                {
                    _config = config;
                    _api = _config.GetValue<string>("");
                }*/

        //HINT task 8 end

        private readonly ApiSettings _apiSettings;

        public PersonController(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync($"{_apiSettings.ApiUrl}/persons");
            if(message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                List<PersonInformation> list = JsonConvert.DeserializeObject<List<PersonInformation>>(jstring);
                return View(list);
            }
            else
            return View(new List<PersonInformation>());
        }
        public IActionResult Add()
        {
            Person person = new Person();
            return View(person);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Person person)
        {
            if(ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                var jsonPerson = JsonConvert.SerializeObject(person);
                StringContent content = new StringContent(jsonPerson,Encoding.UTF8,"application/json");
                HttpResponseMessage message = await client.PostAsync($"{_apiSettings.ApiUrl}/persons", content);
                if(message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "There is an API Error");
                    return View(person);
                }

            }
            else
            {
                return View(person);
            }
        }

        public async Task<IActionResult> Update(int Id)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync($"{_apiSettings.ApiUrl}/persons/" + Id);
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                Person person = JsonConvert.DeserializeObject<Person>(jstring);
                return View(person);
            }
            else
                return RedirectToAction("Add");
        }
        [HttpPost]
        public async Task<IActionResult> Update(Person person)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                var jsonperson = JsonConvert.SerializeObject(person);
                StringContent content = new StringContent(jsonperson, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PutAsync($"{_apiSettings.ApiUrl}/persons", content);
                if(message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(person);
                }
            }
            else
                return View(person);
        }

        // Cod adăugat - Sarcina 1 - As a User I want to be able to delete a Person
        public async Task<IActionResult> Delete(int Id)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.DeleteAsync($"{_apiSettings.ApiUrl}/persons/" + Id);
            if (message.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return BadRequest("Error deleting person");
            }
        }
    }
}
