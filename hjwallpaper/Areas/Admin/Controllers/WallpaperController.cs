using hjwallpaper.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hjwallpaper.Areas.Admin.Controllers
{
    public class WallpaperController : Controller
    {
        //
        // GET: /Admin/Wallpaper/

        //可上传的图片类型
        private readonly static string[] imageType = new string[] { ".jpg", ".png", ".gif" };


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult List()
        {

            return View();
        }

        //获取图片列表
        [HttpGet]
        public ActionResult ListJson()
        {

            Hashtable request = new Hashtable();
            var res = new JsonResult();

            List<WallpaperModel> cateList = new List<WallpaperModel>();

            string sql = "select a.*,b.title as caTitle,c.title as sbTitle from Wallpaper as a left join Category as b on a.caID = b.caID left join Subject as c on a.sbID = c.sbID";

            SqlDataReader reader = SqlHelper.SqlHelper.ExecuteReader(sql, null);

            if (reader.HasRows)
            {

                while (reader.Read())
                {
                    WallpaperModel wallpaperModel = new WallpaperModel();

                    wallpaperModel.wpID = reader.GetInt32(0);
                    wallpaperModel.caID = reader.GetInt32(1);

                    object imgUrl = reader.IsDBNull(2) ? null : reader.GetString(2);
                    wallpaperModel.imageUrl = imgUrl == null ? string.Empty : Convert.ToString(imgUrl);

                    wallpaperModel.clickNum = reader.GetInt32(3);

                    object keyWord = reader.IsDBNull(4) ? null : reader.GetString(4);
                    wallpaperModel.keyWord = keyWord == null ? string.Empty : Convert.ToString(keyWord);

                    wallpaperModel.suID = reader.GetInt32(5);

                    object title = reader.IsDBNull(6) ? null : reader.GetString(6);
                    wallpaperModel.title = title == null ? string.Empty : Convert.ToString(title);

                    object caTitle = reader.IsDBNull(7) ? null : reader.GetString(7);
                    wallpaperModel.caTitle = caTitle == null ? string.Empty : Convert.ToString(caTitle);

                    object sbTitle = reader.IsDBNull(8) ? null : reader.GetString(8);
                    wallpaperModel.sbTitle = sbTitle == null ? string.Empty : Convert.ToString(sbTitle);


                    cateList.Add(wallpaperModel);
                }


                request.Add("message", "查询成功");
                request.Add("isSucceed", true);
                request.Add("data", cateList);
            }
            else
            {
                request.Add("message", "暂时没有壁纸");
                request.Add("isSucceed", false);
            }


            res.Data = request;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return res;
        }


        //发布图片
        [HttpPost]
        public ActionResult AddPhoto(HttpPostedFileBase fileUp, int caID, int sbID, string keyWord,string title) 
        {

            Hashtable request = new Hashtable();
            var res = new JsonResult();


            if (fileUp == null)
            {
                request.Add("message", "请选择图片");
                request.Add("isSucceed", false);

                res.Data = request;

                res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                return res;

            }

            //获取文件的扩展名
            string extenStr = Path.GetExtension(fileUp.FileName);

            bool isHaveType = false;

            foreach (var type in imageType)
            {
                if (extenStr.ToUpper() == type.ToUpper())
                {
                    isHaveType = true;
                }

            }

            if (!isHaveType)
            {
                request.Add("message", "文件类型不支持");
                request.Add("isSucceed", false);

                res.Data = request;

                res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                return res;
            }

            //根据时间生成文件夹路径
            string dir = "/Public/UpLoad/Images/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";

            //判断目录是否存在，不存在就创建目录
            Directory.CreateDirectory(Path.GetDirectoryName(Request.MapPath(dir)));

            //对文件重命名
            string newFileName = Guid.NewGuid().ToString() + extenStr;

            //构建完整的相对文件路径
            string fullDir = dir + newFileName;

            //上传的本地路径
            var fileName = Path.Combine(Request.MapPath(dir), newFileName);


            try
            {
                //保存文件
                fileUp.SaveAs(fileName);

                //return Content("ok" + "<html><head></head><body><img src='" + fullDir + "'></body></html>");


                string sql = "INSERT INTO Wallpaper (caID,imageUrl,clickNum,keyWord,sbID,title) VALUES (@caID,@imageUrl,@clickNum,@keyWord,@sbID,@title)";

                SqlParameter[] pms = new SqlParameter[]{

                    new SqlParameter("@caID",SqlDbType.Int){Value = caID},
                    new SqlParameter("@imageUrl",SqlDbType.VarChar,150){Value = (fullDir == null ? "" : fullDir)},
                    new SqlParameter("@clickNum",SqlDbType.Int){Value = (Request.Form["clickNum"]==null? 10 : Convert.ToInt32(Request.Form["clickNum"]) )},
                    new SqlParameter("@keyWord",SqlDbType.VarChar,150){Value = (keyWord == null ? "" : keyWord)},
                    new SqlParameter("@sbID",SqlDbType.Int){Value = sbID},
                    new SqlParameter("@title",SqlDbType.VarChar,50){Value = (title == null ? "" : title)},
                }; 

                int r = Convert.ToInt32(SqlHelper.SqlHelper.ExecuteNonQuery(sql, pms));

                if (r > 0)
                {

                    request.Add("message", "上传成功");
                    request.Add("isSucceed", true);
                    request.Add("data", fullDir);
                }
                else
                {
                    request.Add("message", "上传失败");
                    request.Add("isSucceed", false);
                }

            }
            catch
            {

                
                request.Add("message", "上传失败");
                request.Add("isSucceed", false);
                
            }

            res.Data = request;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return res;
        }


        [HttpGet]
        public ActionResult Delete() 
        {
            
            Hashtable request = new Hashtable();
            var res = new JsonResult();

            //过去get的参数
            object wpID = Request.QueryString["wpID"];

            ////获取post参数
            //object wpID = Request.Form["wpID"];


            if (wpID == null || wpID.Equals(""))
            {

                request.Add("message", "参数错误");
                request.Add("isSucceed", false);

                res.Data = request;

                res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                return res;

            }


            string sql = "DELETE FROM  Wallpaper WHERE wpID=@wpID";

            SqlParameter[] pms = new SqlParameter[]{

                new SqlParameter("@wpID",SqlDbType.Int){Value = wpID}

            };

            int r = Convert.ToInt32(SqlHelper.SqlHelper.ExecuteNonQuery(sql, pms));

            if (r > 0)
            {

                request.Add("message", "删除成功");
                request.Add("isSucceed", true);
            }
            else
            {
                request.Add("message", "删除失败");
                request.Add("isSucceed", false);
            }

            res.Data = request;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return res;
        
        }


        [HttpGet]
        public ActionResult WpCount()
        {

            Hashtable request = new Hashtable();
            var res = new JsonResult();

            string sql = "SELECT COUNT(*) FROM  Wallpaper";

            int r = Convert.ToInt32(SqlHelper.SqlHelper.ExecuteScalar(sql, null));

            
            request.Add("message", "查询成功");
            request.Add("isSucceed", true);
            request.Add("data", r);
            
            res.Data = request;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return res;

        }



    }
}
