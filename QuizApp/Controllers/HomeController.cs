using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace QuizApp.Controllers
{
    public class HomeController : Controller
    {
        QuizAppEntities db = new QuizAppEntities();




        [HttpGet]
        public ActionResult Sregister()
        {


            return View();
        }

        [HttpPost]
        public ActionResult Sregister(student std, HttpPostedFileBase imgfile)
        {

            student s = new student();
            try
            {
                s.std_name = std.std_name;
                s.std_password = std.std_password;
                s.std_image = uploadingImage(imgfile);

                db.student.Add(s);
                db.SaveChanges();
                return RedirectToAction("slogin");

                
            }
            catch (Exception)
            {

                ViewBag.msg = "Data not inserted !!";

            }
      
              

            return View();

           
        }


        public string uploadingImage(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();


            if (file != null && file.ContentLength>0)
            {

                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("/Content/img"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('image format not accept !!'); </script> ");
                }

            }
            else
            {
                Response.Write("<script>alert('please select a file'); </script> ");
            }

            return path;           
           
        }


       
        public ActionResult Logout()
        {

            Session.Abandon();
            Session.RemoveAll();

            return RedirectToAction("Index");
        }



        [HttpGet]
   
        public ActionResult tlogin()
        {
            return View();
        }


        [HttpPost]
      
        public ActionResult tlogin(tbl_admin a)
        {

            tbl_admin ad = db.tbl_admin.Where(x => x.ad_name == a.ad_name && x.ad_pass == a.ad_pass).SingleOrDefault();

            if (ad != null )
            {
                Session["ad_id"] = ad.ad_id;
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.msg = "Invalid Username or Password"; 
            }

            return View();
        }

        [HttpGet]
      
        public ActionResult slogin()
        {
            return View();
        }


        [HttpPost]
     
        public ActionResult slogin(student s)
        {
            student sv = db.student.Where(x => x.std_name == s.std_name && x.std_password == s.std_password).SingleOrDefault();

            if(sv == null)
            {
                ViewBag.msg = "Invalid User Name or Password";
            }
            else
            {
                Session["std_id"] = s.std_id;
                return RedirectToAction("ExamDashboard");
            }

            return View();
        }

        
        public ActionResult ExamDashboard()
        {
          
            return View();
        }
 
        [HttpPost]
      
        public ActionResult ExamDashboard(string room)
        {

            List <tbl_category> list = db.tbl_category.ToList();

            foreach (var item in list)
            {

               
                if (item.cat_encrytped_string == room)
                {
                    List<tbl_questions> li = db.tbl_questions.Where(x => x.q_fk_catid == item.cat_id).ToList();
                    Queue<tbl_questions> queue = new Queue<tbl_questions>();

                    foreach(tbl_questions a in li)
                    {
                        queue.Enqueue(a);
                    }

                    TempData["questions"] = queue;
                    TempData["score"] = 0;
              
                    TempData.Keep();
                    return RedirectToAction("StartQuiz");
                }
                else
                {
                    ViewBag.error = "No Room found !!";
                }
            }
            return View();
        }

        public ActionResult StartQuiz()
        {

            if (Session["std_id"] == null)
            {
                return RedirectToAction("slogin");
            }

            tbl_questions q = null;
            if(TempData["questions"] != null)
            {
                Queue<tbl_questions> qlist = (Queue<tbl_questions>)TempData["questions"];
                if (qlist.Count > 0)
                {
                    q = qlist.Peek();
                    qlist.Dequeue();

                    TempData["questions"] = qlist;
                    TempData.Keep();
                }
                else
                {
                    return RedirectToAction("EndExam");
                }
             

           
            }
            else
            {
                return RedirectToAction("ExamDashboard");

            }

            return View(q);
            /*  if (TempData["i"] == null)
              {
                  TempData["i"] = 1;
              }
              if (Session["std_id"] == null)
              {
                  return RedirectToAction("slogin");
              }


              try
              {
                  if (TempData["examid"] == null)
                  {
                      return RedirectToAction("ExamDashboard");
                  }

                  tbl_questions q = null;
                  int examId = Convert.ToInt32(TempData["examid"].ToString());


                  if (TempData["q_id"] == null)
                  {

                      q = db.tbl_questions.First(x => x.q_fk_catid == examId);

                      var list = db.tbl_questions.Skip(Convert.ToInt32(TempData["i"].ToString()));

                      int q_id= list.First().q_id;

                      TempData["q_id"] = q.q_id;
                  }
                  else
                  {

                      int q_id = Convert.ToInt32(TempData["q_id"].ToString());
                      q = db.tbl_questions.Where(x => x.q_id == q_id && x.q_fk_catid == examId).SingleOrDefault();

                      var list = db.tbl_questions.Skip(Convert.ToInt32(TempData["i"].ToString()));

                     q_id = list.First().q_id;

                      TempData["q_id"] = q.q_id;
                      TempData["i"] = Convert.ToInt32(TempData["i"].ToString())+ 1 ;

                  }

                  TempData.Keep();
                  return View(q);
              }
              catch(Exception)
              {

                 return RedirectToAction("ExamDashboard");
              }
              */
        }

        [HttpPost]
        public ActionResult StartQuiz(tbl_questions q)
        {

            string correctans = null;
            if (q.QA != null)
            {

                correctans = "A";
              /*  if (q.QA.Equals("A")){ 
                    TempData["score"] = Convert.ToInt32(TempData["score"]) + 1;
                }*/
            }
            else if(q.QB != null)
            {
                correctans = "B";
                /*
                if (q.QB.Equals("B"))
                {
                    TempData["score"] = Convert.ToInt32(TempData["score"]) + 1;
                }*/
            }
            else if (q.QC != null)
            {
                correctans = "C";
                /*   if (q.QC.Equals("C"))
                   {
                       TempData["score"] = Convert.ToInt32(TempData["score"]) + 1;
                   }*/
            }
            else if (q.QD != null)
            {

                correctans = "D";
                /*  if (q.QD.Equals("D"))
                  {
                      TempData["score"] = Convert.ToInt32(TempData["score"]) + 1;
                  }*/
            }

            if (correctans.Equals(q.QA)) {

                TempData["score"] = Convert.ToInt32(TempData["score"]) + 1;
            }

            TempData.Keep();
            return RedirectToAction("StartQuiz");
        }

        public ActionResult EndExam(tbl_questions q)
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("tlogin");
            }
            else
            {
                return View();
            }
            
        }

        [HttpGet]

        public ActionResult Add_category()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Dashboard");
            }
            Session["ad_id"] = 1;
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

            Random r = new Random()
;
            c.cat_name = cat.cat_name;
            c.cat_fk_ad_id = Convert.ToInt32(Session["ad_id"].ToString());
            c.cat_encrytped_string = crypt.Encrypt(cat.cat_name.Trim() + r.Next().ToString(),true) ;

            db.tbl_category.Add(c);
            db.SaveChanges();

            return RedirectToAction("Add_category");
           

        }



        [HttpGet]
        public ActionResult Add_Questions()
        {


    
            int sid = Convert.ToInt32(Session["ad_id"]);
            List<tbl_category> li = db.tbl_category.Where(x => x.cat_fk_ad_id == sid).ToList();
            ViewBag.list = new SelectList(li, "cat_id", "cat_name");

          
            return View();

        }

        [HttpPost]
        public ActionResult Add_Questions(tbl_questions q)
        {


            int sid = Convert.ToInt32(Session["ad_id"]);
            List<tbl_category> li = db.tbl_category.Where(x => x.cat_fk_ad_id == sid).ToList();
            ViewBag.list = new SelectList(li, "cat_id", "cat_name");

            tbl_questions qa = new tbl_questions();

            qa.q_text = q.q_text;
            qa.QA = q.QA;
            qa.QB = q.QB;
            qa.QC = q.QC;
            qa.QD = q.QD;
            qa.QCorrectAns = q.QCorrectAns;

            qa.q_fk_catid = q.q_fk_catid;

            db.tbl_questions.Add(qa);

            db.SaveChanges();

            TempData["ms"] = "Question successfully added";
            TempData.Keep();

            return RedirectToAction("Add_Questions");
        }

        public ActionResult Index()
        {

            if (Session["ad_id"]!=null)
            {
                return RedirectToAction("Dashboard");
            }

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