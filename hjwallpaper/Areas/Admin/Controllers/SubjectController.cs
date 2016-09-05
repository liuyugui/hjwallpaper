using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hjwallpaper.Models;
using System.Data.SqlClient;
using System.IO;
using System.Data;

namespace hjwallpaper.Areas.Admin.Controllers
{
    public class SubjectController : Controller
    {
        //
        // GET: /Admin/Subject/

        public ActionResult Index()
        {
            return View();
        }

        //可上传的图片类型
        private readonly static string[] imageType = new string[] { ".jpg", ".png", ".gif" };

        [HttpPost]
        public ActionResult AddJson(HttpPostedFileBase fileUp, string title)
        {
            
            Hashtable request = new Hashtable();
            var res = new JsonResult();

            if (fileUp == null)
            {
                request.Add("message", "请选择主题图片");
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
                request.Add("message", "图片类型不支持");
                request.Add("isSucceed", false);

                res.Data = request;

                res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                return res;
            }

            //根据时间生成文件夹路径
            string dir = "/Public/UpLoad/Images/Subject/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";

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

                string sql = "INSERT INTO Subject (title,imageUrl) VALUES (@title,@imageUrl);";

                SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@imageUrl",SqlDbType.VarChar,150){Value = (fullDir == null ? "" : fullDir)},
                    new SqlParameter("@title",SqlDbType.VarChar,50){Value = (title == null ? "" : title)},
                };

                int r = Convert.ToInt32(SqlHelper.SqlHelper.ExecuteNonQuery(sql, pms));

                if (r > 0)
                {

                    request.Add("message", "添加成功");
                    request.Add("isSucceed", true);
                    request.Add("data", fullDir);
                }
                else
                {
                    request.Add("message", "添加失败");
                    request.Add("isSucceed", false);
                }

            }
            catch
            {


                request.Add("message", "主题图片上传失败");
                request.Add("isSucceed", false);

            }


            res.Data = request;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return res;

        
        }


        [HttpGet]
        public ActionResult List()
        {

            Hashtable request = new Hashtable();
            var res = new JsonResult();

            List<SubjectModel> cateList = new List<SubjectModel>();

            string sql = "select * from Subject";

            SqlDataReader reader = SqlHelper.SqlHelper.ExecuteReader(sql, null);

            if (reader.HasRows)
            {

                while (reader.Read())
                {
                    SubjectModel cateModel = new SubjectModel();

                    cateModel.sbID = reader.GetInt32(0);

                    object title = reader.IsDBNull(1) ? null : reader.GetString(1);
                    cateModel.title = title == null ? string.Empty : Convert.ToString(title);

                    object imageUrl = reader.IsDBNull(2) ? null : reader.GetString(2);
                    cateModel.imageUrl = imageUrl == null ? string.Empty : Convert.ToString(imageUrl);

                    cateList.Add(cateModel);
                }


                request.Add("message", "查询成功");
                request.Add("isSucceed", true);
                request.Add("data", cateList);
            }
            else
            {
                request.Add("message", "暂时没有主题");
                request.Add("isSucceed", false);
            }


            res.Data = request;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return res;


        }
        

    }


}
