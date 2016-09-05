using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hjwallpaper.Models
{
    public class WallpaperModel
    {


        public int wpID { set; get; }
        public int caID { set; get; }
        public string imageUrl { set; get; }
        public int clickNum { set; get; }
        public string keyWord { set; get; }
        public int suID { set; get; }
        public string title { set; get; }

        //主题名字
        public string sbTitle { set; get; }
        //分类名字
        public string caTitle { set; get; }

    }
}