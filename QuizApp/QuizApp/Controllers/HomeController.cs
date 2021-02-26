using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizApp.Controllers
{
    public class HomeController : Controller
    {
        QuizAppEntities db = new QuizAppEntities();

        public ActionResult tlogin()
        {
            return View();
        }


        public ActionResult slogin()
        {
            return View();
        }


        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]

        public ActionResult Add_category()
        {
            Session["ad_id"] = 2;
            int ad_id = Convert.ToInt32(Session["ad_id"].ToString());

            List<tbl_category> catlist = db.tbl_category.Where(x=>x.cat_fk_ad_id==ad_id).OrderByDescending(x => x.cat_id).ToList();
            ViewData["list"] = catlist;
            return View();
        }


        [HttpPost]

        public ActionResult Add_category(tbl_category cat)
        {
        
            List<tbl_category> catlist = db.tbl_category.OrderByDescending(x => x.cat_id).ToList();
            ViewData["list"] = catlist;


            tbl_category c = new tbl_category();

            c.cat_name = cat.cat_name;
            c.cat_fk_ad_id = Convert.ToInt32(Session["ad_id"].ToString());
            db.tbl_category.Add(c);
            db.SaveChanges();

            return RedirectToAction("Add_category");
           

        }



        [HttpGet]
        public ActionResult AddQuestions()
        {

            int sid = Convert.ToInt32(Session["ad_id"]);
            List<tbl_category> li = db.tbl_category.Where(x => x.cat_id == sid).ToList();
            ViewBag.list = new SelectList(li, "cat-id", "cat_name");

            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}