using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SqlHelper;
using System.Data.SqlClient;
using System.Data;

namespace hjwallpaper.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /Admin/User/

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Quit()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(string user,string passeord) 
        {

            Hashtable request = new Hashtable();
            var res = new JsonResult();


            string sql = "select count(*) from Account where account=@account and password=@password";

            SqlParameter[] pms = new SqlParameter[]{

                new SqlParameter("@account",SqlDbType.VarChar,50){Value = user},
                new SqlParameter("@password",SqlDbType.VarChar,50){Value = passeord}

            };

            int r = Convert.ToInt32(SqlHelper.SqlHelper.ExecuteScalar(sql, pms));

            if (r > 0)
            {

                request.Add("message", "登录成功");
                request.Add("isSucceed", true);
            }
            else
            {
                request.Add("message", "登录失败");
                request.Add("isSucceed", false);
            }

            res.Data = request;
            
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return res;

        }

    }
}
