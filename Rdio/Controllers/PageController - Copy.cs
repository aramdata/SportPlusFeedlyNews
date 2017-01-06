using MongoDB.Bson;
using NSR.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using UC = NSR.Util.Common;
using NSR.Util;

namespace NSR.Controllers
{
    public class PageController : Controller
    {
        [HttpGet,Authorize]
        public async Task<ActionResult> New()
        {
            await UC.InitializeAsync(this);
            ViewBag.active_addpage = "active";
            return View();
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> New(string newspaper, string ptitle, string date, int version=0,int pagenumber=0)
        {
            newspaper = newspaper.ToStringFarsi();
            ptitle = ptitle.ToStringFarsi();
            if (version==0)
                TempData["MSG"] = "لطفا نسخه را وارد کنید";
            else if (pagenumber==0)
                TempData["MSG"] = "لطفا شماره صفحه را وارد کنید";
            else if(string.IsNullOrWhiteSpace(newspaper))
                TempData["MSG"] = "لطفا عنوان روزنامه را وارد کنید";
            else if (string.IsNullOrWhiteSpace(ptitle))
                TempData["MSG"] = "لطفا عنوان صفحه را وارد کنید";
            else if (string.IsNullOrWhiteSpace(date))
                TempData["MSG"] = "لطفا تاریخ روزنامه را وارد کنید";
            else
            {
                var collection = NoSql.Instance.GetCollection<BsonDocument>("pages");
                //var uinfo = await UC.UserInfoAsync();
                var bdoc = new BsonDocument { { "newspaper", newspaper }, { "title", ptitle }, { "date", date }, { "version", version }, { "pagenumber", pagenumber }, { "status", "نگارش" }, { "lastactiontime", DateTime.Now.Ticks }, { "lastactor", UC.My.name /*uinfo["name"].AsString*/ } };
                if (Request.Files.Count > 0)
                {
                    var filename = Guid.NewGuid();
                    if(Request.Files["pdf"]!=null && Request.Files["pdf"].ContentLength > 1)
                    {
                        Request.Files["pdf"].SaveAs(Server.MapPath(string.Format("~/content/page/{0}.pdf", filename)));
                        bdoc.Add("pdf", string.Format("~/content/page/{0}.pdf", filename));
                    }
                    if (Request.Files["indd"] != null && Request.Files["indd"].ContentLength > 1)
                    {
                        Request.Files["indd"].SaveAs(Server.MapPath(string.Format("~/content/page/{0}.indd", filename)));
                        bdoc.Add("indd", string.Format("~/content/page/{0}.indd", filename));
                    }
                    if (Request.Files["jpg"] != null && Request.Files["jpg"].ContentLength > 1)
                    {
                        Request.Files["jpg"].SaveAs(Server.MapPath(string.Format("~/content/page/{0}.jpg", filename)));
                        bdoc.Add("jpg", string.Format("~/content/page/{0}.jpg", filename));
                    }
                }
                var historyArray = new BsonArray();
                historyArray.Add(new BsonDocument { { "date", DateTime.Now.Ticks }, { "actor", UC.My.name /*uinfo["name"].AsString*/ }, { "action", "ثبت" } });
                bdoc.Add("history", historyArray);
                var actors = new BsonArray();
                actors.Add(/*uinfo["_id"].ToString()*/ UC.My.name);
                bdoc.Add("actors", actors);
                await collection.InsertOneAsync(bdoc);
                return RedirectToAction("Index");
            }
            ViewBag.active_addpage = "active";
            ViewBag.newspaper = newspaper;
            ViewBag.ptitle = ptitle;
            ViewBag.date = date;
            ViewBag.version = version;
            ViewBag.pagenumber = pagenumber;
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Index(string pid, string ptitle, int page = 1, bool inbox = false)
        {
            ptitle = ptitle.ToStringFarsi();
            await UC.InitializeAsync(this);
            var limit = 20;
            var skip = limit * (page - 1);
            ViewBag.page = page;
            ViewBag.plimit = limit;
            ViewBag.RowIndexStart = (page * limit) - limit;
            ViewBag.pid = pid;
            ViewBag.ptitle = ptitle;
            string q = "{aggregate:'pages',pipeline:[{$sort : { '_id' : -1 }},{$skip:" + skip.ToString() + "},{$limit:" + limit.ToString() + "},{$match:{$and:[{'status':{$ne:'حذف شده'}},";
            if (!string.IsNullOrWhiteSpace(ptitle) || !string.IsNullOrWhiteSpace(pid))
            {
                if (!string.IsNullOrWhiteSpace(ptitle))
                    q += "{title:{$regex:'" + ptitle + "'}},";
                if (!string.IsNullOrWhiteSpace(pid))
                    q += "{_id:ObjectId('" + pid + "')},";
            }
            q = q.Trim(',') + "]}}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            ViewBag.active_pages = "active";
            return View(model.GetValue("result"));
        }


        [HttpGet,Authorize]
        public async Task<ActionResult> Edit(string id)
        {
            await UC.InitializeAsync(this);
            string q = "{aggregate:'pages',pipeline:[{$match:{_id:ObjectId('" + id + "')}},{$limit:1}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            BsonDocument page = model.GetValue("result")[0].AsBsonDocument;
            if (page.Contains("openedby") && !string.IsNullOrWhiteSpace(page["openedby"].AsString))
            {
                ViewBag.locker = page["openedby"].AsString;
                return View("Locked");
            }
            //var uinfo = await UC.UserInfoAsync();
            q = "{update:'pages',updates:[{q:{_id:ObjectId('" + id + "')},u:{$set:{openedby:'" + (ViewBag.ME as BsonDocument)["name"].AsString + "'}}}]}";
            //await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            ViewBag.active_pages = "active";
            return View(page);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> Remove(string id)
        {
            string q = "{delete:'pages',deletes:[{q:{_id:ObjectId('" + id + "')},limit:1}]}";
            //string q = "{aggregate:'users',pipeline:[{$match:{_id:ObjectId('" + id + "')}},{$limit:1}]}";
            //var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            //q = "{update:'users',updates:[{ q: { _id: ObjectId('" + id + "')} ,u: {$set: { status: 'حذف شده',uname:'" + model.GetValue("result")[0]["uname"].AsString + ";" + DateTime.Now.Ticks.ToString() + "'} } }]}";
            await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            return RedirectToAction("Index");
        }


    }
}