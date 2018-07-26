using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            RepositoryService.CreateDefaultObject("Account");
            RepositoryService.AddField("Account", "FirstName", "varchar(255)");
            RepositoryService.Insert("INSERT INTO Account (FirstName) VALUES ('Test " + DateTime.Now.ToString() + "')");
            var accounts = RepositoryService.Select<dynamic>("SELECT * FROM Account");
            var list = new List<Dictionary<string, string>>();
            foreach(var account in accounts)
            {
                var dictionary = new Dictionary<string, string>();
                foreach(KeyValuePair<string,object> kv in account)
                {
                    dictionary[kv.Key] = kv.Value?.ToString();
                }
                list.Add(dictionary);
            }
            return View(list);
        }

        public ActionResult About(string id)
        {
            ViewBag.Message = "Your application description page.";
              
            var dictionary = new Dictionary<string, string>();
            if(id != null)
            {
                var record = RepositoryService.GetByID<dynamic>(id);
                foreach (KeyValuePair<string, object> kv in record)
                {
                    dictionary[kv.Key] = kv.Value?.ToString();
                }
            }    
            return View(dictionary);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}