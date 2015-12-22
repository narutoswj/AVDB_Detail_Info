using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AVDB_Detail_Info
{
    class Program
    {
        static Regex RegexbigImage = new Regex(@"a class=""bigImage"" href=""(.*?)""");
        static Regex Regexfanhao = new Regex(@"<p><span class=""header"">番號:</span> (.*?)</span></p>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
        static Regex Regexgroup = new Regex(@"/group/(.*?)""");
        static Regex RegexreleaseDate = new Regex(@"發行時間:</strong> (.*?)<");
        static Regex Regexlength = new Regex(@"影片長度:</strong> (.*?)<");

        static Regex Regexdirector = new Regex(@"/director/(.*?)"">(.*?)<");
        static Regex Regexstudio = new Regex(@"/studio/(.*?)"">(.*?)<");
        static Regex Regexlabel = new Regex(@"/label/(.*?)"">(.*?)<");
        static Regex Regexseries = new Regex(@"/series/(.*?)"">(.*?)<");
        static Regex Regexgenre = new Regex(@"/genre/(.*?)"">(.*?)<");
        static Regex Regexactress = new Regex(@"aria-expanded=""true"">(.*?)<");
        static Regex RegexSnapshot = new Regex(@"layer-img=""(.*?)"" alt=""(.*?)""");

        static string connectString = "Data Source=.;Initial Catalog=Media;Integrated Security=True";

        static void Main(string[] args)
        {
            try { 
            DataSet ds = GetUrlList();
            string test = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                string url = r.ItemArray[0].ToString();
                string html = GetUrltoHtml(url, "utf-8");
                string fanhao = Regexfanhao.Matches(html)[0].Groups[1].Value.ToString();
                string group = Regexgroup.Matches(html)[0].Groups[1].Value.ToString();

                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine(group + " - " + fanhao);
                Console.WriteLine();

                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + group;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //if (!File.Exists(path + "\\" + fanhao + ".jpeg"))
                //{
                //    WebClient webClient = new WebClient();
                //    webClient.DownloadFile(RegexbigImage.Matches(html)[0].Groups[1].ToString(), path + "\\" + fanhao + ".jpeg");
                //    webClient.Dispose();
                //}

                GetDetailBasic(url, fanhao, html);
                GetDirector(url, fanhao, html);
                GetStudio(url, fanhao, html);
                GetLabel(url, fanhao, html);
                GetSeries(url, fanhao, html);
                GetGenre(url, fanhao, html);
                GetActress(url, fanhao, html);
                GetSnapshot(url, fanhao, html);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("---------------------------------------------------");
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void GetActress(string url, string fanhao, string html)
        {
            MatchCollection ActressMatchCollection = Regexactress.Matches(html);

            foreach (Match match in ActressMatchCollection)
            {
                SqlConnection sqlCnt = new SqlConnection(connectString);
                sqlCnt.Open();
                SqlCommand cmd = sqlCnt.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO [dbo].[VideoActress] ([SerialNumber],[ActressCode],[Actress]) VALUES (@SerialNumber,@ActressCode,@Actress)";
                cmd.Parameters.Add("@SerialNumber", SqlDbType.NVarChar);
                cmd.Parameters.Add("@ActressCode", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Actress", SqlDbType.NVarChar);

                cmd.Parameters["@SerialNumber"].Value = fanhao;
                cmd.Parameters["@ActressCode"].Value = match.Groups[1].Value.ToString();
                cmd.Parameters["@Actress"].Value = match.Groups[2].Value.ToString();

                Console.WriteLine(DateTime.Now + " Actress: " + match.Groups[1].Value.ToString());

                try
                {
                    cmd.ExecuteScalar();
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                //string test = match.Groups[1].Value.ToString();
            }
        }

        private static void GetGenre(string url, string fanhao, string html)
        {
            MatchCollection GenreMatchCollection = Regexgenre.Matches(html);

            foreach (Match match in GenreMatchCollection)
            {
                SqlConnection sqlCnt = new SqlConnection(connectString);
                sqlCnt.Open();
                SqlCommand cmd = sqlCnt.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO [dbo].[VideoGenre] ([SerialNumber],[GenreCode],[Genre]) VALUES (@SerialNumber,@GenreCode,@Genre)";
                cmd.Parameters.Add("@SerialNumber", SqlDbType.NVarChar);
                cmd.Parameters.Add("@GenreCode", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Genre", SqlDbType.NVarChar);

                cmd.Parameters["@SerialNumber"].Value = fanhao;
                cmd.Parameters["@GenreCode"].Value = match.Groups[1].Value.ToString();
                cmd.Parameters["@Genre"].Value = match.Groups[2].Value.ToString();

                Console.WriteLine(DateTime.Now + " Genre: " + match.Groups[2].Value.ToString());
                try
                {
                    cmd.ExecuteScalar();
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                //string test = match.Groups[1].Value.ToString();
            }
        }

        private static void GetSeries(string url, string fanhao, string html)
        {
            MatchCollection seriesMatchCollection = Regexseries.Matches(html);

            foreach (Match match in seriesMatchCollection)
            {
                SqlConnection sqlCnt = new SqlConnection(connectString);
                sqlCnt.Open();
                SqlCommand cmd = sqlCnt.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO [dbo].[VideoSeries] ([SerialNumber],[SeriesCode],[Series]) VALUES (@SerialNumber,@SeriesCode,@Series)";
                cmd.Parameters.Add("@SerialNumber", SqlDbType.NVarChar);
                cmd.Parameters.Add("@SeriesCode", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Series", SqlDbType.NVarChar);

                cmd.Parameters["@SerialNumber"].Value = fanhao;
                cmd.Parameters["@SeriesCode"].Value = match.Groups[1].Value.ToString();
                cmd.Parameters["@Series"].Value = match.Groups[2].Value.ToString();

                Console.WriteLine(DateTime.Now + " Series: " + match.Groups[2].Value.ToString());

                try
                {
                    cmd.ExecuteScalar();
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                //string test = match.Groups[1].Value.ToString();
            }
        }

        private static void GetLabel(string url, string fanhao, string html)
        {
            MatchCollection labelMatchCollection = Regexlabel.Matches(html);

            foreach (Match match in labelMatchCollection)
            {
                SqlConnection sqlCnt = new SqlConnection(connectString);
                sqlCnt.Open();
                SqlCommand cmd = sqlCnt.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO [dbo].[VideoLabel] ([SerialNumber],[LabelCode],[Label]) VALUES (@SerialNumber,@LabelCode,@Label)";
                cmd.Parameters.Add("@SerialNumber", SqlDbType.NVarChar);
                cmd.Parameters.Add("@LabelCode", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Label", SqlDbType.NVarChar);

                cmd.Parameters["@SerialNumber"].Value = fanhao;
                cmd.Parameters["@LabelCode"].Value = match.Groups[1].Value.ToString();
                cmd.Parameters["@Label"].Value = match.Groups[2].Value.ToString();

                Console.WriteLine(DateTime.Now + " Label: " + match.Groups[2].Value.ToString());

                try
                {
                    cmd.ExecuteScalar();
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                //string test = match.Groups[1].Value.ToString();
            }
        }

        private static void GetStudio(string url, string fanhao, string html)
        {
            MatchCollection studioMatchCollection = Regexstudio.Matches(html);

            foreach (Match match in studioMatchCollection)
            {
                SqlConnection sqlCnt = new SqlConnection(connectString);
                sqlCnt.Open();
                SqlCommand cmd = sqlCnt.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO [dbo].[VideoStudio] ([SerialNumber],[StudioCode],[Studio]) VALUES (@SerialNumber,@StudioCode,@Studio)";
                cmd.Parameters.Add("@SerialNumber", SqlDbType.NVarChar);
                cmd.Parameters.Add("@StudioCode", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Studio", SqlDbType.NVarChar);

                cmd.Parameters["@SerialNumber"].Value = fanhao;
                cmd.Parameters["@StudioCode"].Value = match.Groups[1].Value.ToString();
                cmd.Parameters["@Studio"].Value = match.Groups[2].Value.ToString();

                Console.WriteLine(DateTime.Now + " Studio: " + match.Groups[2].Value.ToString());

                try
                {
                    cmd.ExecuteScalar();
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                //string test = match.Groups[1].Value.ToString();
            }
        }

        private static void GetDirector(string url, string fanhao, string html)
        {
            MatchCollection directorMatchCollection = Regexdirector.Matches(html);

            foreach (Match match in directorMatchCollection)
            {
                SqlConnection sqlCnt = new SqlConnection(connectString);
                sqlCnt.Open();
                SqlCommand cmd = sqlCnt.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO [dbo].[VideoDirector] ([SerialNumber],[DirectorCode],[Director]) VALUES (@SerialNumber,@DirectorCode,@Director)";
                cmd.Parameters.Add("@SerialNumber", SqlDbType.NVarChar);
                cmd.Parameters.Add("@DirectorCode", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Director", SqlDbType.NVarChar);

                cmd.Parameters["@SerialNumber"].Value = fanhao;
                cmd.Parameters["@DirectorCode"].Value = match.Groups[1].Value.ToString();
                cmd.Parameters["@Director"].Value = match.Groups[2].Value.ToString();

                Console.WriteLine(DateTime.Now + " Director: " + match.Groups[2].Value.ToString());

                try
                {
                    cmd.ExecuteScalar();
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                //string test = match.Groups[1].Value.ToString();
            }
        }

        private static void GetSnapshot(string url, string fanhao, string html)
        {
            MatchCollection directorMatchCollection = RegexSnapshot.Matches(html);

            foreach (Match match in directorMatchCollection)
            {
                SqlConnection sqlCnt = new SqlConnection(connectString);
                sqlCnt.Open();
                SqlCommand cmd = sqlCnt.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO [dbo].[VideoSnapshot] ([SerialNumber],[URL],[PicName]) VALUES (@SerialNumber,@URL,@PicName)";
                cmd.Parameters.Add("@SerialNumber", SqlDbType.NVarChar);
                cmd.Parameters.Add("@URL", SqlDbType.NVarChar);
                cmd.Parameters.Add("@PicName", SqlDbType.NVarChar);

                cmd.Parameters["@SerialNumber"].Value = fanhao;
                cmd.Parameters["@URL"].Value = match.Groups[1].Value.ToString();
                cmd.Parameters["@PicName"].Value = match.Groups[2].Value.ToString();

                Console.WriteLine(DateTime.Now + " Snapshot: " + match.Groups[2].Value.ToString());

                try
                {
                    cmd.ExecuteScalar();
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    sqlCnt.Close();
                    sqlCnt.Dispose();
                }
                //string test = match.Groups[1].Value.ToString();
            }
        }

        private static void GetDetailBasic(string url, string fanhao, string html)
        {
            SqlConnection sqlCnt = new SqlConnection(connectString);
            string bigImage = RegexbigImage.Matches(html)[0].Groups[1].Value;
            string releaseDate, length;
            try
            {
                releaseDate = RegexreleaseDate.Matches(html)[0].Groups[1].Value;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                releaseDate = "";
            }
            try
            {
                length = Regexlength.Matches(html)[0].Groups[1].Value;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                length = "";
            }
            sqlCnt.Open();
            SqlCommand cmd = sqlCnt.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO [dbo].[VideoDetail] ([SerialNumber],[ReleaseDate],[Fanhao],[Length],[BigPicUrl]) VALUES (@SerialNumber,@ReleaseDate,@Fanhao,@Length,@BigPicUrl)";
            cmd.Parameters.Add("@SerialNumber", SqlDbType.NVarChar);
            cmd.Parameters.Add("@ReleaseDate", SqlDbType.NVarChar);
            cmd.Parameters.Add("@Fanhao", SqlDbType.NVarChar);
            cmd.Parameters.Add("@Length", SqlDbType.NVarChar);
            cmd.Parameters.Add("@BigPicUrl", SqlDbType.NVarChar);
            cmd.Parameters["@SerialNumber"].Value = Regexfanhao.Matches(html)[0].Groups[1].Value.ToString();
            cmd.Parameters["@ReleaseDate"].Value = releaseDate;
            cmd.Parameters["@Fanhao"].Value = fanhao;
            cmd.Parameters["@Length"].Value = length;
            cmd.Parameters["@BigPicUrl"].Value = RegexbigImage.Matches(html)[0].Groups[1].Value.ToString();

            try
            {
                cmd.ExecuteScalar();
                sqlCnt.Close();
                sqlCnt.Dispose();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                sqlCnt.Close();
                sqlCnt.Dispose();
            }
        }

        public static string GetUrltoHtml(string Url, string type)
        {
            try
            {
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }

        }

        public static DataSet GetUrlList()
        {
            SqlConnection sqlCnt = new SqlConnection(connectString);
            SqlCommand cmd = sqlCnt.CreateCommand();
            cmd.CommandTimeout = 60;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT A.[SerialHyperLink]  FROM [Media].[dbo].[VideoSummary] A  LEFT JOIN [Media].[dbo].[VideoDetail] B  ON A.SerialNumber =B.Fanhao  where B.SerialNumber is null";
            DataSet ds = new DataSet();
            sqlCnt.Open();
            SqlDataAdapter command = new SqlDataAdapter(cmd.CommandText, connectString);
            command.Fill(ds, "ds");

            return ds;
        }
    }
}
