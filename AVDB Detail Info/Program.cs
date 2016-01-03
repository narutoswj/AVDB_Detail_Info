using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
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
        static string connectStringSQLite = "Data Source  =D:\\Media.db";

        static void Main(string[] args)
        {
            CreateTable(connectStringSQLite);

            while (GetUrlList(connectStringSQLite).Count >= 1)
            {
                foreach (string url in GetUrlList(connectStringSQLite))
                {
                    string pageHtml = GetUrltoHtml(url, "utf-8");
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(pageHtml);

                    var htmlNode = htmlDocument.DocumentNode;

                    string fanhao = "";
                    string length = "";
                    string releasedate = "";
                    string director = "";
                    string directorhyperlink = "";
                    string studio = "";
                    string studiohyperlink = "";
                    string series = "";
                    string serieshyperlink = "";
                    string label = "";
                    string labelhyperlink = "";
                    string genre = "";
                    string genrehyperlink = "";
                    string actress = "";
                    string snapshotname = "";
                    string snapshotimage = "";
                    string insertSQL = "";

                    try
                    {
                        var allinfo = htmlNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[1]/div[2]");
                        var infolist = allinfo.SelectNodes("//p");
                        var actresslist = htmlNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[4]/div[1]/nav[1]/ul[1]//li");
                        var snapshotlist = htmlNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[5]/div[1]/img");

                        foreach (var info in infolist)
                        {
                            var attrspan = info.SelectSingleNode("./span");
                            if (attrspan != null)
                            {
                                if (attrspan.InnerHtml.Contains("番號"))
                                {
                                    fanhao = info.LastChild.InnerText.Trim();
                                    Console.WriteLine(fanhao);
                                }
                            }

                            var attrstrong = info.SelectSingleNode("./strong");
                            if (attrstrong != null)
                            {
                                if (attrstrong.InnerHtml.Contains("長度"))
                                {
                                    length = info.LastChild.InnerText.Trim();
                                    Console.WriteLine(length);
                                }
                                else if (attrstrong.InnerHtml.Contains("發行時間"))
                                {
                                    releasedate = info.LastChild.InnerText.Trim();
                                    Console.WriteLine(releasedate);
                                }
                                else if (attrstrong.InnerHtml.Contains("導演"))
                                {
                                    var directors = info.SelectNodes("./a");
                                    foreach (var d in directors)
                                    {
                                        director = d.InnerText;
                                        directorhyperlink = d.GetAttributeValue("href", "No").Split('/')[4];
                                        insertSQL = insertSQL + "INSERT INTO [VideoDirector] SELECT '" + fanhao + "','" + directorhyperlink + "','" + director + "';";
                                        Console.WriteLine(director);
                                        Console.WriteLine(directorhyperlink);
                                    }
                                }
                                else if (attrstrong.InnerHtml.Contains("制作商"))
                                {
                                    var studios = info.SelectNodes("./a");
                                    foreach (var s in studios)
                                    {
                                        studio = s.InnerText;
                                        studiohyperlink = s.GetAttributeValue("href", "No").Split('/')[4];
                                        insertSQL = insertSQL + "INSERT INTO [VideoStudio] SELECT '" + fanhao + "','" + studiohyperlink + "','" + studio + "';";
                                        Console.WriteLine(studio);
                                        Console.WriteLine(studiohyperlink);
                                    }
                                }
                                else if (attrstrong.InnerHtml.Contains("發行商"))
                                {
                                    var labels = info.SelectNodes("./a");
                                    foreach (var l in labels)
                                    {
                                        label = l.InnerText;
                                        labelhyperlink = l.GetAttributeValue("href", "No").Split('/')[4];
                                        insertSQL = insertSQL + "INSERT INTO [VideoLabel] SELECT '" + fanhao + "','" + labelhyperlink + "','" + label + "';";
                                        Console.WriteLine(label);
                                        Console.WriteLine(labelhyperlink);
                                    }
                                }
                                else if (attrstrong.InnerHtml.Contains("系列"))
                                {
                                    var serieses = info.SelectNodes("./a");
                                    foreach (var s in serieses)
                                    {
                                        series = s.InnerText;
                                        serieshyperlink = s.GetAttributeValue("href", "No").Split('/')[4];
                                        insertSQL = insertSQL + "INSERT INTO [VideoSeries] SELECT '" + fanhao + "','" + serieshyperlink + "','" + series + "';";
                                        Console.WriteLine(series);
                                        Console.WriteLine(serieshyperlink);
                                    }
                                }
                                else if (attrstrong.InnerHtml.Contains("類別"))
                                {
                                    var genres = info.SelectNodes("./a");
                                    foreach (var g in genres)
                                    {
                                        genre = g.InnerText;
                                        genrehyperlink = g.GetAttributeValue("href", "No").Split('/')[4];
                                        insertSQL = insertSQL + "INSERT INTO [VideoGenre] SELECT '" + fanhao + "','" + genrehyperlink + "','" + genre + "';";
                                        Console.WriteLine(genre);
                                        Console.WriteLine(genrehyperlink);
                                    }
                                }
                            }
                        }

                        if (actresslist != null)
                        {
                            foreach (var ac in actresslist)
                            {
                                actress = ac.InnerText;
                                insertSQL = insertSQL + "INSERT INTO [VideoActress] SELECT '" + fanhao + "','','" + actress + "';";
                                Console.WriteLine(actress);
                            }
                        }

                        if (snapshotlist != null)
                        {
                            foreach (var s in snapshotlist)
                            {
                                snapshotname = s.GetAttributeValue("alt", "No");
                                snapshotimage = s.GetAttributeValue("data-original", "No");
                                insertSQL = insertSQL + "INSERT INTO [VideoSnapshot] SELECT '" + fanhao + "','" + snapshotimage + "','" + snapshotname + "';";
                                Console.WriteLine(snapshotimage);
                            }
                        }

                        string coverImage = htmlNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/a[1]").GetAttributeValue("href", "No");
                        Console.WriteLine(coverImage);
                        insertSQL = insertSQL + "INSERT INTO [VideoDetail] SELECT '" + fanhao + "','" + releasedate + "','" + fanhao + "','" + length + "','" + coverImage + "';";
                        ExectueInsertQuery(connectStringSQLite,insertSQL);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
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



        private static List<string> GetUrlList(string connectStringSQLite)
        {
            List<string> pagelist = new List<string>();
            SQLiteConnection conn = null;
            conn = new SQLiteConnection(connectStringSQLite);
            conn.Open();
            string sql = @"SELECT A.[SerialHyperLink]  FROM [VideoSummary] A  LEFT JOIN [VideoDetail] B  ON A.SerialNumber =B.Fanhao  WHERE B.SerialNumber IS NULL ORDER BY A.[SerialHyperLink] DESC LIMIT 0,100";
            //string sql = @"SELECT A.[SerialHyperLink]  FROM [VideoSummary] A  LEFT JOIN [VideoDetail] B  ON A.SerialNumber =B.Fanhao  WHERE A.[SerialHyperLink] = 'https://avdb.lol/fanhao/ZZZD-004.html'";
            SQLiteCommand cmdFailedPage = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = cmdFailedPage.ExecuteReader();
            while (reader.Read())
            {
                pagelist.Add(reader.GetString(0));

            }
            conn.Close();

            return pagelist;
        }

        private static void CreateTable(string connectStringSQLite)
        {
            SQLiteConnection conn = null;
            conn = new SQLiteConnection(connectStringSQLite);
            conn.Open();
            string sql = @"CREATE TABLE IF NOT EXISTS [VideoActress](
                            [SerialNumber] [nvarchar](50) NULL,
                            [ActressCode] [nvarchar](50) NULL,
                            [Actress] [nvarchar](50) NULL);
                            CREATE TABLE IF NOT EXISTS [VideoDetail](
                            [SerialNumber] [nvarchar](500) NULL,
                            [ReleaseDate] [nvarchar](500) NULL,
                            [Fanhao] [nvarchar](500) NULL,
                            [Length] [nvarchar](500) NULL,
                            [BigPicUrl] [nvarchar](500) NULL);
                            CREATE TABLE IF NOT EXISTS [VideoDirector](
                            [SerialNumber] [nvarchar](50) NULL,
                            [DirectorCode] [nvarchar](50) NULL,
                            [Director] [nvarchar](50) NULL);
                            CREATE TABLE IF NOT EXISTS [VideoGenre](
                            [SerialNumber] [nvarchar](50) NULL,
                            [GenreCode] [nvarchar](50) NULL,
                            [Genre] [nvarchar](50) NULL);
                            CREATE TABLE IF NOT EXISTS [VideoLabel](
                            [SerialNumber] [nvarchar](50) NULL,
                            [LabelCode] [nvarchar](50) NULL,
                            [Label] [nvarchar](50) NULL);
                            CREATE TABLE IF NOT EXISTS [VideoSeries](
                            [SerialNumber] [nvarchar](50) NULL,
                            [SeriesCode] [nvarchar](50) NULL,
                            [Series] [nvarchar](50) NULL);
                            CREATE TABLE IF NOT EXISTS [VideoSnapshot](
                            [SerialNumber] [nvarchar](50) NULL,
                            [URL] [nvarchar](500) NULL,
                            [PicName] [nvarchar](500) NULL);
                            CREATE TABLE IF NOT EXISTS [VideoStudio](
                            [SerialNumber] [nvarchar](50) NULL,
                            [StudioCode] [nvarchar](50) NULL,
                            [Studio] [nvarchar](50) NULL);";

            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
            cmdCreateTable.ExecuteNonQuery();
            conn.Close();
        }

        private static void ExectueInsertQuery(string connectStringSQLite, string query)
        {
            SQLiteConnection conn = null;
            conn = new SQLiteConnection(connectStringSQLite);
            conn.Open();
            string sql = query;

            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
            cmdCreateTable.ExecuteNonQuery();
            conn.Close();
        }
    }
}
