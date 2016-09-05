using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;



namespace SqlHelper
{
    public static class SqlHelper
    {
        //链接字符串
        private static readonly string conStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        //执行（insert/delete/update）
        //
        public static int ExecuteNonQuery(string sql,params SqlParameter[] pms) 
        {
            using(SqlConnection con = new SqlConnection(conStr))
            {

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    con.Open();

                    return cmd.ExecuteNonQuery();

                }
            }
        
        }

        //执行查询语句
        public static object ExecuteScalar(string sql, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    con.Open();

                    return cmd.ExecuteScalar();

                }
            }

        }


        //执行查询多行多列
        public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] pms)
        {
            SqlConnection con = new SqlConnection(conStr);
            
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {

                if (pms != null)
                {
                    cmd.Parameters.AddRange(pms);
                }

                try
                {

                    con.Open();

                    //System.Data.CommandBehavior.CloseConnection 这个枚举参数，表示将来使用完毕sqltareader后在关闭reader的同时关闭connection对象也关闭
                    return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                }
                catch 
                {
                    con.Close();
                    con.Dispose();
                    throw;
                }
                
            }
        
        }



    }
}
