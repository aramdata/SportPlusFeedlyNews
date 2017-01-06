using MongoDB.Bson;
using Rdio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.Text;
using System.ServiceModel.Syndication;
using SimpleFeedReader;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace Rdio.Util
{
    public static class Common
    {
        public enum ServiceResultStatus
        {
            OK=0,
            Error=1
        }
        public enum ServiceResultMessage
        {
            [Description("با موفقیت انجام شد")]
            OKMessage = 0,
            [Description("مشکلی رخ داده است")]
            FaildMessage = 1
        }
        public enum RssLanguege {
            [Description("فارسی")]
            Persian = 0,
            [Description("انگلیسی")]
            English = 1
        }

        public static string PersianNumberToEnglishNumber(string input)
        {
            if (input.Trim() == "") return "";

            //۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹
            input = input.Replace("۰", "0");
            input = input.Replace("۱", "1");
            input = input.Replace("۲", "2");
            input = input.Replace("۳", "3");
            input = input.Replace("۴", "4");
            input = input.Replace("۵", "5");
            input = input.Replace("۶", "6");
            input = input.Replace("۷", "7");
            input = input.Replace("۸", "8");
            input = input.Replace("۹", "9");
            return input;
        }

        public static string CleanHtmlContent(string html)
        {
            string noHTML = Regex.Replace(html, @"<[^>]+>|&nbsp;", "").Trim();
            string noHTMLNormalised = Regex.Replace(noHTML, @"\s{2,}", " ");
            return noHTMLNormalised;
        }

        public static DateTime ParsDateFromHtml(string html)
        {
            //in Time Remove All Space for example (2016 - 12 - 29T14: 55:13Z) => (2016-12-29T14:55:13Z)
            var DateSimple1 = @"(\d+)(-|\/)(\d+)(?:-|\/)(?:(\d+)|s+(\d+):(\d+)(?::(\d+))?(?:\.(\d+))?)?";
            var DateSimple2 = @"\d{2,4} + (دی)\s + \d{1,2}";
            var DateSimple3 = @"(Jan(uary)?|Feb(ruary)?|Mar(ch)?|Apr(il)?|May|Jun(e)?|Jul(y)?|Aug(ust)?|Sep(tember)?|Oct(ober)?|Nov(ember)?|Dec(ember)?)\s+\d{1,2},\s+\d{4}";

            var TimeSimple1 = @"([01]?[0-9]|2[0-3]):[0-5][0-9](:[0-5][0-9])?";

            var Clean = PersianNumberToEnglishNumber(html);
            var _Date=Regex.Matches(Clean, DateSimple1);
            var _Time= Regex.Matches(Clean.Replace(" ",""), TimeSimple1);

            var _d= Regex.Matches(Clean, DateSimple2);
            var Date = "";
            var Time = "";

            foreach (Match item in _Date)
                if (item.Length > Date.Length)
                    Date = item.Value;

            foreach (Match item in _Time)
                if (item.Length > Time.Length)
                    Time = item.Value;

            return new DateTime();
        }
        public static string GetMD5(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));
            return sb.ToString();
        }
        public class rssitem {
            public string title { get; set; }
            public string link { get; set; }
            public string description { get; set; }
            public DateTime date { get; set; }
        }
        public async static Task<List<rssitem>> ParseRssFile(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return new List<rssitem>();
                string content = "";
                using (var client = new HttpClient())
                {
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0");
                    client.DefaultRequestHeaders.Add("Host", "fut5al.ir");


                    using (var r = await client.GetAsync(new Uri(url)))
                    {
                        content = await r.Content.ReadAsStringAsync();
                        
                    }
                }

                XmlDocument rssXmlDoc = new XmlDocument();
                //rssXmlDoc.Load(url);
                rssXmlDoc.LoadXml(content);
                XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");
                StringBuilder rssContent = new StringBuilder();
                var res = new List<rssitem>();
                foreach (XmlNode rssNode in rssNodes)
                {
                    XmlNode rssSubNode = rssNode.SelectSingleNode("title");
                    string title = rssSubNode != null ? rssSubNode.InnerText : "";

                    rssSubNode = rssNode.SelectSingleNode("link");
                    string link = rssSubNode != null ? rssSubNode.InnerText : "";

                    rssSubNode = rssNode.SelectSingleNode("description");
                    string description = rssSubNode != null ? rssSubNode.InnerText : "";

                    rssSubNode = rssNode.SelectSingleNode("pubDate");
                    string date = rssSubNode != null ? rssSubNode.InnerText : "";
                    var datetime = DateTime.Now;
                    DateTime.TryParse(date, out datetime);

                    rssContent.Append("<a href='" + link + "'>" + title + "</a><br>" + description);
                    res.Add(new rssitem() { title = title, link = link, description = description, date = datetime });
                }
                return res;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
            
        }

        public static List<FeedItem> ParseRssFile2(string url)
        {
            using (XmlReader reader = XmlReader.Create(url))
            {
                var reader1 = new FeedReader();
                var items = reader1.RetrieveFeed(reader);
                return items.ToList();
                //SyndicationFeed feed = SyndicationFeed.Load(reader);
                //return feed.Items;
            }
        }

        public enum SearchCondition {
            Popular=0,
            MostVisited=1,
            MostSelling=2,
            Newest=3
        }
        public static string ToBase62(this long id)
        {
            var BASE62_ALPHABET = new string[]{
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
            var hash = "";
            var hashDigits = new List<long>();
            double dividend = id;
            double remainder = 0;
            while (dividend > 0)
            {
                remainder = Math.Floor((double)(dividend % 62));
                dividend = Math.Floor((double)(dividend / 62));
                hashDigits.Insert(0, (int)remainder);
            }
            foreach (var v in hashDigits)
            {
                hash += BASE62_ALPHABET[v];
            }
            return hash;
        }
        public static async Task<bool> InitializeAsync(System.Web.Mvc.Controller c)
        {
            c.ViewBag.myPerms = await userPerms(My.id);
            c.ViewBag.UserNewsNegareshFlows = await UserNegareshFlowsAsync("خبر");
            c.ViewBag.UserPageNegareshFlows = await UserNegareshFlowsAsync("صفحه");
            var ninboxCount = await NoSql.Instance.RunCommandAsync<BsonDocument>("{count:'pages',query:{touser:'" + My.name + "'}}");
            c.ViewBag.pinboxCount = ninboxCount["n"].AsInt32;
            var pinboxCount = await NoSql.Instance.RunCommandAsync<BsonDocument>("{count:'news',query:{touser:'" + My.name + "'}}");
            c.ViewBag.ninboxCount = pinboxCount["n"].AsInt32;
            return true;
        }
        public static string ToStringFarsi(this string input)
        {
            if (!string.IsNullOrEmpty(input))
                return input.Replace("ي", "ی").Replace("ك", "ک");
            else
                return "";
        }
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string ToStringPersianDateTime(this long input)
        {
            DateTime dt = new DateTime(input);
            return string.Format("{0} {1}:{2}", ConvertG2JDateText(dt), dt.Hour.ToString("00"), dt.Minute.ToString("00"));
        }

        public static string toJSON(this object input)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(input);
        }
        public static T fromJSON<T>(this string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(input);
        }
        public static async Task<BsonValue> AllRolesAsync()
        {
            string q = "{aggregate:'roles',pipeline:[{$sort : { 'name' : 1 }}]}";
            var roles = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            return roles.GetValue("result");
        }
        public static async Task<BsonValue> RoleUsersAsync(string[] roles)
        {
            string q = "{aggregate:'users',pipeline:[{$match:{'roles':{$in:" + roles.toJSON() + "}}},{$sort : { 'name' : 1 }},{$project:{name:1,_id:0}}]}";
            var users = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            return users.GetValue("result");
        }

        public static async Task<IEnumerable<string>> AllCategoriesAsync()
        {
            string q = "{aggregate:'categories',pipeline:[{$sort : {'name' : 1 }}]}";
            var categories = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            return categories["result"].AsBsonArray.Select(c=>c["name"].AsString);
        }
        public static async Task<BsonArray> AllNewsPapersAsync()
        {
            string q = "{distinct:'pages',key:'newspaper'}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            return model["values"].AsBsonArray;
        }
        
        public static async Task<BsonValue> OrgUsersAsync()
        {
            string q = "{aggregate:'users',pipeline:[{$match:{$and:[{'status':{$ne:'حذف شده'}},{'roles':'سازمانی'}]}},{$sort : { 'name' : 1 }}]}";
            var users = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            return users.GetValue("result");
        }

        /*public static string GetSHA1HashData(string data)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));
            StringBuilder returnValue = new StringBuilder();
            for (int i = 0; i < hashData.Length; i++)
                returnValue.Append(hashData[i].ToString());
            return returnValue.ToString();
        }
        public static bool ValidateSHA1HashData(string inputData, string storedHashData)
        {
            string getHashInputData = GetSHA1HashData(inputData);
            if (string.Compare(getHashInputData, storedHashData) == 0)
                return true;
            else
                return false;
        }*/
        public static string ConvertG2JDateText(DateTime GDate,bool withtime=false)
        {
            PersianCalendar objPersianCalendar = new PersianCalendar();
            var result= objPersianCalendar.GetYear(GDate).ToString("0000") + "/" + 
                objPersianCalendar.GetMonth(GDate).ToString("00") + "/" + 
                objPersianCalendar.GetDayOfMonth(GDate).ToString("00");
            if (withtime)
                result += " " + objPersianCalendar.GetHour(GDate).ToString("00") + ":" +
                     objPersianCalendar.GetMinute(GDate).ToString("00") + ":" +
                     objPersianCalendar.GetSecond(GDate).ToString("00");
            return result;
        }
        public static DateTime ConvertJDateText2GDate(string JDateText)
        {
            PersianCalendar objPersianCalendar = new PersianCalendar();
            string[] strJDate = JDateText.Split('/');
            DateTime JDATE = objPersianCalendar.ToDateTime(int.Parse(strJDate[0]), int.Parse(strJDate[1]), int.Parse(strJDate[2]), 0, 0, 0, 0);
            return JDATE;
        }
        /*public static BsonDocument UserInfo(string id=null)
        {
            id = id ?? System.Web.HttpContext.Current.User.Identity.Name;
            string q = "{aggregate:'users',pipeline:[{$match:{_id:ObjectId('" + id + "')}},{$limit:1}]}";
            var model = NoSql.Instance.RunCommand<BsonDocument>(q);
            return model.GetValue("result")[0].AsBsonDocument;
        }*/
        public static async Task<BsonDocument> UserInfoAsync(string id=null)
        {
            if(System.Web.HttpContext.Current.Items["USERINFOASYNC"]!=null)
                return (BsonDocument)System.Web.HttpContext.Current.Items["USERINFOASYNC"];
            //id = id ?? System.Web.HttpContext.Current.User.Identity.Name;
            id = id ?? My.id;// System.Web.HttpContext.Current.User.Identity.Name;
            string q = "{aggregate:'users',pipeline:[{$match:{_id:ObjectId('" + id + "')}},{$limit:1}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            System.Web.HttpContext.Current.Items["USERINFOASYNC"] = model.GetValue("result")[0].AsBsonDocument;
            return model.GetValue("result")[0].AsBsonDocument;
        }

        public static BsonDocument UserInfo(string id = null)
        {
            var cachkey = string.Format("USERINFO_{0}", id);
            if (System.Web.HttpContext.Current.Items[cachkey] != null)
                return (BsonDocument)System.Web.HttpContext.Current.Items[cachkey];
            //id = id ?? System.Web.HttpContext.Current.User.Identity.Name;
            id = id ?? My.id;// System.Web.HttpContext.Current.User.Identity.Name;
            string q = "{aggregate:'users',pipeline:[{$match:{_id:ObjectId('" + id + "')}},{$limit:1}]}";
            var model = NoSql.Instance.RunCommandAsync<BsonDocument>(q).Result;
            System.Web.HttpContext.Current.Items[cachkey] = model.GetValue("result")[0].AsBsonDocument;
            return model.GetValue("result")[0].AsBsonDocument;
        }
        public static USR My
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<USR>(System.Web.HttpContext.Current.User.Identity.Name);
            }
        }
        /******Flow******/
        public static async Task<BsonArray> UserNegareshFlowsAsync(string type,BsonDocument user=null)
        {
            if (user == null)
                user = await UserInfoAsync();
            string q = "{aggregate:'flows',pipeline:[{$match:{type:'" + type + "',steps:{$elemMatch:{title:'نگارش',$or:[{'users.id':'" + user["_id"].ToString() + "'},{roles:{$in:" + user["roles"].AsBsonArray.toJSON() + "}}]}}}},{$project:{_id:1,title:1}}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            return model.GetValue("result").AsBsonArray;
        }

        public static async Task<bool> AllowedFlowStepAsync(string fid,string step,BsonDocument user=null)
        {
            if (user == null)
                user = await UserInfoAsync();
            string q = "{aggregate:'flows',pipeline:[{$match:{_id:ObjectId('"+fid+"'),steps:{$elemMatch:{title:'"+step+"',$or:[{'users.id':'" + user["_id"].ToString() + "'},{roles:{$in:" + user["roles"].AsBsonArray.toJSON() + "}}]}}}},{$project:{_id:1,title:1}}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            return model.GetValue("result").AsBsonArray.Any();
        }

        public static async Task<bool> CanAdd(string type,string fid)
        {
            var NegareshFlows = await UserNegareshFlowsAsync(type);
            if (NegareshFlows.Any(f => f.AsBsonDocument["_id"].ToString() == fid))
                return true;
            return false;
        }
        public static async Task<bool> CanEdit(BsonDocument obj,string p)
        {
            //var me = await UserInfoAsync();
            if (obj["status"].AsString == "حذف شده")
                return false;
            if ((!obj.Contains("touser") && obj["lastactor"].AsString == My.name ) || (obj.Contains("touser") && obj["touser"].AsString == My.name ) )
                return true;
            if(!string.IsNullOrWhiteSpace(p))
            {
                string q = "{aggregate:'pages',pipeline:[{$match:{_id:ObjectId('" + p + "')}},{$limit:1}]}";
                var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
                BsonDocument page = model.GetValue("result")[0].AsBsonDocument;
                if (page["touser", ""].AsString == My.name)
                    return true;
            }
            return false;
        }

        public static async Task<BsonArray> NextFlowStepsAsync(string fid,string currstep)
        {
            string q = "{aggregate:'flows',pipeline:[{$match:{_id: ObjectId('" + fid + "')}},{$project: { steps: {$filter: { input: '$steps',as:'step',cond: {$eq:['$$step.title', '"+currstep+"']} } } } }]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            return model["result"][0]["steps"][0]["next"].AsBsonArray;
        }

        /******Flow_******/

        /******Perm******/
        /*public static async Task<bool> hasPerm(string key,string uid)
        {
            var u = await UserInfoAsync(uid);
            string q = "{count:'perms',query:{'key':'" + key + "',$or:[{'users.id':'" + uid + "',roles:{$in:" + u["roles"].toJSON() + "}}]}}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            if (model["n"].AsInt32 > 0)
                return true;
            return false;
        }*/
        public static async Task<BsonArray> userPerms(string uid)
        {
            var u = await UserInfoAsync(uid);
            BsonDocument model = null;
            if (u["uname"].AsString == "admin")
                model = await NoSql.Instance.RunCommandAsync<BsonDocument>("{aggregate:'perms',pipeline:[{$project:{key:1}}]}");
            else
                model = await NoSql.Instance.RunCommandAsync<BsonDocument>("{ aggregate: 'perms',pipeline:[{$match: {$or:[{ 'users.id':'"+uid+"'},{roles: {$in:"+ u["roles"].toJSON() + "}}]}},{$project:{key:1}}]}");
            return model["result"].AsBsonArray;
        }

        /******Perm_******/

        public static async Task<bool> SendEmail(string from, List<string> to, string subject, string body, string fromName = "", List<string> cc = null, List<string> bcc = null, List<string> attachpath = null)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(from, fromName, System.Text.Encoding.UTF8);
                    foreach (string item in to)
                    {
                        if (!string.IsNullOrEmpty(item))
                            mail.Bcc.Add(new MailAddress(item));
                    }
                    if (mail.Bcc.Count == 0)
                        return false;
                    if (attachpath != null && attachpath.Count() > 0)
                    {
                        foreach (var ap in attachpath)
                        {
                            mail.Attachments.Add(new Attachment(ap));
                        }
                    }
                    mail.Subject = subject;
                    mail.SubjectEncoding = System.Text.Encoding.UTF8;
                    mail.Body = body;
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    mail.IsBodyHtml = true;
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(Regex.Replace(body, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    mail.AlternateViews.Add(plainView);
                    mail.AlternateViews.Add(htmlView);
                    mail.Headers.Set("X-Spam-Status", "No, score=4.9 required=5.0 tests=BAYES_50,HTML_IMAGE_ONLY_04, HTML_MESSAGE,MIME_HTML_ONLY,NO_DNS_FOR_FROM,NO_RELAYS autolearn=no version=3.2.5");
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.EnableSsl = bool.Parse(GetSettingValue("emailssl").AsString);
                        client.Credentials = new System.Net.NetworkCredential(GetSettingValue("emailusername").AsString, GetSettingValue("emailpassword").AsString);
                        client.Port = int.Parse(GetSettingValue("emailport").AsString);
                        client.Host = GetSettingValue("emailhost").AsString;
                        client.Send(mail);
                    }
                }
            }
            catch (Exception exc)
            {

            }
            return true;
        }

        public static BsonValue GetSettingValue(string Key)
        {
            try
            {
                string q = "{aggregate:'settings',pipeline:[{$match:{}}]}";
                var model = NoSql.Instance.RunCommandAsync<BsonDocument>(q).Result;
                if (model.GetValue("result").AsBsonArray.Count > 0)
                    return model.GetValue("result")[0].AsBsonDocument.GetValue(Key);
            }
            catch (Exception exc)
            {
            }
            return "";
        }

        public static List<string> GetPortalCategories()
        {
            return  new List<string>();
            //string q = "{aggregate:'products',pipeline:[{$match:{status:" + (int)Rdio.Modules.Product.Common.ProductStatus.Enable + "}},{$unwind: '$categories' },{ $group : { _id : '$categories' } } ]}";
            //var _model = NoSql.Instance.RunCommandAsync<BsonDocument>(q).Result;
            //var model = _model.GetValue("result").AsBsonArray;
            //var result = new List<string>();
            //foreach (var item in model)
            //{
            //    result.Add(item.AsBsonDocument.GetValue("_id").AsString);
            //}
            //return result;
        }

    }

    public class USR
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}