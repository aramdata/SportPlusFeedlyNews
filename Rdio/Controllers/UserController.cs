using MongoDB.Bson;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using UC = Rdio.Util.Common;
using Rdio.Util;
using Rdio.Models;
using System.Web.Security;
using System.Collections.Generic;
using CaptchaMvc.HtmlHelpers;

namespace Rdio.Controllers
{
    public class UserController : Controller
    {
        [Authorize]
        public async Task<ActionResult> Manage(string name,string uname,int page=1)
        {
            await UC.InitializeAsync(this);
            if (!(ViewBag.myPerms as BsonArray).Any(p => p["key"].AsString == "perm_superuser"))
                return View("AccessDenied");
            name = name.ToStringFarsi();
            var limit = 50;
            var skip = limit * (page - 1);
            ViewBag.page = page;
            ViewBag.plimit = limit;
            ViewBag.RowIndexStart = (page * limit) - limit;
            ViewBag.name = name;
            ViewBag.uname = uname;
            string q = "{aggregate:'users',pipeline:[{$match:{$and:[{'status':{$ne:'حذف شده'}},";
            if (!string.IsNullOrWhiteSpace(name))
                q += "{name:{$regex:'" + name + "'}},";
            if (!string.IsNullOrWhiteSpace(uname))
                q += "{'uname':'" + uname + "'},";
            q = q.Trim(',') + "]}}{$sort : { '_id' : -1 }},{$skip:" + skip.ToString() + "},{$limit:" + limit.ToString() + "}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            ViewBag.active_users = "active";
            return View(model.GetValue("result"));
        }

        [HttpGet,Authorize]
        public async Task<ActionResult> New()
        {
            await UC.InitializeAsync(this);
            ViewBag.roles = await UC.AllRolesAsync();
            ViewBag.categories = await UC.AllCategoriesAsync();
            ViewBag.active_users = "active";
            return View();
        }

        [HttpPost,Authorize]
        public async Task<ActionResult> New(string name,string uname,string password,string email,string[] roles,string[] categories)
        {
            await UC.InitializeAsync(this);
            uname = (uname ?? "").ToLower();
            email = (email ?? "").ToLower();
            name = name.ToStringFarsi();
            if (string.IsNullOrWhiteSpace(name))
                TempData["MSG"] = "لطفا نام و نام خانوادگی را وارد کنید";
            else if (string.IsNullOrWhiteSpace(uname))
                TempData["MSG"] = "لطفا نام کاربری را وارد کنید";
            else if (string.IsNullOrWhiteSpace(password))
                TempData["MSG"] = "لطفا رمزعبور را وارد کنید";
            else if (string.IsNullOrWhiteSpace(email))
                TempData["MSG"] = "لطفا پست الکترونیک را وارد کنید";
            else if (roles==null)
                TempData["MSG"] = "لطفا حدافل یک نقش  را انتخاب کنید";
            else if (categories==null)
               TempData["MSG"] = "لطفا حدافل یک سرویس  را انتخاب کنید";
            else if ((await NoSql.Instance.RunCommandAsync<BsonDocument>("{aggregate:'users',pipeline:[{$match:{uname:'" + uname + "'}},{$limit:1}]}")).GetValue("result").AsBsonArray.Count>0)
            {
                TempData["MSG"] = "این نام کاربری هم اکنون در سیستم موجود است";
            }
            else if ((await NoSql.Instance.RunCommandAsync<BsonDocument>("{aggregate:'users',pipeline:[{$match:{name:'" + name + "'}},{$limit:1}]}")).GetValue("result").AsBsonArray.Count > 0)
            {
                TempData["MSG"] = "کاربری با این نام و نام خانوادگی هم اکنون در سیستم موجود است";
            }
            else 
            {
                var collection = NoSql.Instance.GetCollection<BsonDocument>("users");
                var bdoc = new BsonDocument { { "name", name }, { "uname", uname }, { "password", password }, { "email", email }, { "status","فعال"}, { "createdat", DateTime.Now} };
                if(roles.Length>0)
                    bdoc.Add("roles", new BsonArray(roles));
                if (categories.Length > 0)
                    bdoc.Add("categories", new BsonArray(categories));
                await collection.InsertOneAsync(bdoc);
                return RedirectToAction("Manage");
            }

            ViewBag.name = name;
            ViewBag.uname = uname;
            ViewBag.email = email;
            ViewBag.roles = await UC.AllRolesAsync();
            ViewBag.categories = await UC.AllCategoriesAsync();
            return View();
        }

        [HttpGet,Authorize]
        public async Task<ActionResult> Edit(string id)
        {
            await UC.InitializeAsync(this);
            string q = "{aggregate:'users',pipeline:[{$match:{_id:ObjectId('" + id + "')}},{$limit:1}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            ViewBag.roles = await UC.AllRolesAsync();
            ViewBag.categories = await UC.AllCategoriesAsync();
            ViewBag.active_users = "active";
            return View(model.GetValue("result")[0]);
        }

        [HttpPost,Authorize]
        public async Task<ActionResult> Edit(string id,string name,string email, string[] roles,string[] categories)
        {
            email = (email ?? "").ToLower();
            name = name.ToStringFarsi();
            //if (string.IsNullOrWhiteSpace(name))
            //    TempData["MSG"] = "لطفا نام و نام خانوادگی را وارد کنید";
           
            if (string.IsNullOrWhiteSpace(email))
                TempData["MSG"] = "لطفا پست الکترونیک را وارد کنید";
            
            
            else
            {
                //string q = "{update:'users',updates:[{q:{_id:ObjectId('"+id+ "')},u:{$set:{name:'" + name+"',email:'"+email+"',roles:[" + string.Join(",", roles.Select(r=>"'"+r+"'"))  +"]}}}]}";
                string q = "{update:'users',updates:[{q:{_id:ObjectId('" + id+ "')},u:{$set:{email:'"+email+"',roles:"+(roles?? new string[] { }).toJSON()+ ",categories:" + (categories ?? new string[] { }).toJSON() + "}}}]}";
                await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
                return RedirectToAction("Manage");
            }
            ViewBag.name = name;
            ViewBag.email = email;
            ViewBag.roles = await UC.AllRolesAsync();
            ViewBag.categories = await UC.AllCategoriesAsync();
            return View();
        }

        [HttpPost,Authorize]
        public async Task<ActionResult> Remove(string id)
        {
            string q = "{delete:'users',deletes:[{q:{_id:ObjectId('" + id + "'),uname:{$ne:'admin'}},limit:1}]}";
            //string q = "{aggregate:'users',pipeline:[{$match:{_id:ObjectId('" + id + "')}},{$limit:1}]}";
            //var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            //q = "{update:'users',updates:[{ q: { _id: ObjectId('"+id+ "')} ,u: {$set: { status: 'حذف شده',uname:'"+ model.GetValue("result")[0]["uname"].AsString + ";" + DateTime.Now.Ticks.ToString() + "'} } }]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            if (model["n"].AsInt32 == 0)
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.DenyGet, Data = "این کاربر سیستمی است و نمیتواند حذف گردد" };
            else
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.DenyGet, Data = "" };
        }

        [HttpPost,Authorize]
        public async Task<ActionResult> Toggle(string id)
        {
            string q = "{aggregate:'users',pipeline:[{$match:{_id:ObjectId('" + id + "'),uname:{$ne:'admin'}}},{$limit:1}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            if(model["result"].AsBsonArray.Count==0)
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.DenyGet, Data = "این کاربر سیستمی است و نمیتواند غیرفعال گردد" };
            var currStatus = model.GetValue("result")[0]["uname"].AsString;
            if(currStatus== "فعال")
            {
                q = "{update:'users',updates:[{ q: { _id: ObjectId('" + id + "')} ,u: {$set: { status: 'غیرفعال'} } }]}";
                await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            }
            else if(currStatus == "غیرفعال")
            {
                q = "{update:'users',updates:[{ q: { _id: ObjectId('" + id + "')} ,u: {$set: { status: 'فعال'} } }]}";
                await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            }
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.DenyGet, Data = "" };
        }

        [HttpPost,Authorize]
        public async Task<ActionResult> resetpassword(string uid, string newpassword)
        {
            if (string.IsNullOrWhiteSpace(newpassword))
            {
                TempData["MSG"] = "لطفا رمز عبور جدید را وارد کنید";
            }
            else
            {
                string q = "{update:'users',updates:[{q:{_id:ObjectId('" + uid + "')},u:{$set:{password:'" + newpassword + "'}}}]}";
                var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
                TempData["SMSG"] = "رمز عبور با موفقیت تغییر یافت";
            }
            return RedirectToAction("Edit", new { id = uid });
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> changepassword(string password, string newpassword)
        {
            string q = "{update:'users',updates:[{q:{_id:ObjectId('" + UC.My.id /*User.Identity.Name*/ + "'),password:'"+password+"'},u:{$set:{password:'" + newpassword + "'}}}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            if(model["nModified"].AsInt32==0)
            TempData["MSG"] = "رمز عبور جاری نادرست است";
            else
            TempData["SMSG"] = "رمز عبور با موفقیت تغییر یافت";
            return RedirectToAction("My");
        }

        [HttpGet, Authorize]
        public async Task<ActionResult> myprofile()
        {
            await UC.InitializeAsync(this);
            return View(await UC.UserInfoAsync());
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> myprofile(string name)
        {
            name = (name ?? "").ToLower();
            if (string.IsNullOrWhiteSpace(name))
                TempData["MSG"] = "لطفا نام و نام خانوادگی را وارد کنید";
            else
            {
                string q = "{update:'users',updates:[{q:{_id:ObjectId('" + UC.My.id + "')},u:{$set:{name:'" + name + "'}}}]}";
                await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
                TempData["SMSG"] = "اطلاعات پروفایل با موفقیت ثبت شد";
            }
            return RedirectToAction("My");
        }

        [Authorize, HttpGet]
        public async Task<ActionResult> inboxcount(string type)
        {
            /********db.onlines.createIndex({"expireAt":1},{expireAfterSeconds:0})********/
            await NoSql.Instance.RunCommandAsync<BsonDocument>("{update:'onlines',updates:[{q:{user:'" + UC.My.name + "'},u:{$set:{expireAt:ISODate('" + DateTime.UtcNow.AddSeconds(60).ToString("o") + "')}},upsert:true}]}");
            var inboxCount = await NoSql.Instance.RunCommandAsync<BsonDocument>("{count:'"+ type + "',query:{status:{$ne:'حذف شده'},touser:'" + UC.My.name + "'}}");
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = inboxCount["n"].ToString() };
        }


        /*******Role*********/
        [HttpGet,Authorize]
        public async Task<ActionResult> Roles()
        {
            await UC.InitializeAsync(this);
            string q = "{aggregate:'roles',pipeline:[{$sort : { '_id' : 1 }}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            ViewBag.active_roles = "active";
            return View(model.GetValue("result"));
        }
        [HttpPost,Authorize]
        public async Task<ActionResult> AddRole(string rolename)
        {
            rolename = rolename.ToStringFarsi();
            if ((await NoSql.Instance.RunCommandAsync<BsonDocument>("{aggregate:'roles',pipeline:[{$match:{name:'" + rolename + "'}},{$limit:1}]}")).GetValue("result").AsBsonArray.Count > 0)
            {
                TempData["MSG"] = "نقشی با این نام هم اکنون در سیستم موجود است";
            }
            else
            {
                var collection = NoSql.Instance.GetCollection<BsonDocument>("roles");
                await collection.InsertOneAsync(new BsonDocument { { "name", rolename } });
            }
            return RedirectToAction("Roles");
        }
        [HttpPost,Authorize]
        public async Task<ActionResult> RemoveRole(string id,string name)
        {
            name = name.ToStringFarsi();
            string q = "{aggregate:'users',pipeline:[{$match:{roles:'" + name + "'}},{$limit:1}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            if(model.GetValue("result").AsBsonArray.Count > 0 || name=="مدیر" || name=="سازمانی")
            {
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.DenyGet, Data = "این نقش قابل حذف نیست زیرا سیستمی است و یا کاربری با این نقش در سیستم وجود دارد" };
            }
            else
            {
                q = "{delete:'roles',deletes:[{q:{_id:ObjectId('" + id + "')},limit:1}]}";
                await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.DenyGet, Data = "" };
            }
        }
        /*******Role_*********/


        public ActionResult Login(string returnUrl = "/")
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string UserName, string Password, string returnUrl, bool RememberMe = false)
        {
            returnUrl = (string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
            ViewBag.returnUrl = returnUrl;
            string q = "{aggregate:'users',pipeline:[{$match:{$and:[{uname:'" + UserName + "'},{password:'" + Password + "'}]}},{$limit:1}]}";
            var model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            if (model.GetValue("result").AsBsonArray.Count == 0)
                return View((object)"نام کاربری یا رمز عبور اشتباه میباشد");
            var user = model.GetValue("result")[0];
            if(user["status"]== "حذف شده" || user["status"]== 0)
                return View((object)"حساب کاربری شما غیرفعال است");
            FormsAuthentication.SetAuthCookie((new USR { id= user["_id"].ToString() ,name=user["name"].AsString}).toJSON() , RememberMe);
            return Redirect(returnUrl);
        }

        [Authorize,HttpGet]
        public async Task<ActionResult> Logout()
        {
            string q = "{delete:'online',deletes:[{q:{user:'"+UC.My.name+"'},limit:1}]}";
            await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            FormsAuthentication.SignOut();
            Session.Abandon();
            return Redirect("/");
        }

        /*******/
        [HttpGet]
        public async Task<ActionResult> Register()
        {
            //await UC.InitializeAsync(this);
            //ViewBag.roles = await UC.AllRolesAsync();
            //ViewBag.categories = await UC.AllCategoriesAsync();
            //ViewBag.active_users = "active";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(string name, string password, string uname, string[] roles, string[] categories)
        {
            TempData["MSGT"] = "alert-danger";
            uname = (uname ?? "").ToLower();
            name = name.ToStringFarsi();
            ViewBag.name = name;
            ViewBag.uname = uname;
            if (!this.IsCaptchaValid("Captcha is not valid"))
            {
                TempData["MSG"] = "کد امنیتی اشتباه است.";
                return View();
            }

            roles = new string[] { "مشتری" };
            if (string.IsNullOrWhiteSpace(name))
                TempData["MSG"] = "لطفا نام و نام خانوادگی را وارد کنید";
            else if (string.IsNullOrWhiteSpace(password))
                TempData["MSG"] = "لطفا رمزعبور را وارد کنید";
            else if (string.IsNullOrWhiteSpace(uname))
                TempData["MSG"] = "لطفا پست الکترونیک (نام کاربری) را وارد کنید";
            else if ((await NoSql.Instance.RunCommandAsync<BsonDocument>("{aggregate:'users',pipeline:[{$match:{uname:'" + uname + "'}},{$limit:1}]}")).GetValue("result").AsBsonArray.Count > 0)
            {
                TempData["MSG"] = "این نام کاربری هم اکنون در سیستم موجود است";
            }
            else
            {
                var id = ObjectId.GenerateNewId();
                var Activationid = Guid.NewGuid().ToString();
                var collection = NoSql.Instance.GetCollection<BsonDocument>("users");
                var bdoc = new BsonDocument { { "_id", id }, { "name", name }, { "uname", uname }, { "password", password }, { "status", 0 }, { "createdat", DateTime.Now }, { "activationid", Activationid } };
                if (roles.Length > 0)
                    bdoc.Add("roles", new BsonArray(roles));
                await collection.InsertOneAsync(bdoc);
                TempData["MSG"] = string.Format("کاربر گرامی {0} ، اطلاعات تکمیلی ثبت نام به پست الکترونیک {1} ارسال گردید.",name,uname);
                TempData["MSGT"] = "alert-success";
                string emailbody = string.Format("برای فعال سازی حساب کاربری لطفا روی لینک زیر کلیک نمایید. \n <a href='http://{0}/user/activeregister/{1}/{2}/{3}' target='_blank'>لینک فعال سازی حساب کاربری</a>",Request.Url.Authority, id.ToString(), Activationid, Guid.NewGuid());
                await Common.SendEmail(Common.GetSettingValue("email").AsString, new List<string>() { uname }, "تکمیل ثبت نام", emailbody);
                return RedirectToAction("Register");
            }
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> activeregister(string id, string activationid, string tmpActivationid)
        {
            string q = "{aggregate:'users',pipeline:[{$match:{_id:ObjectId('" + id + "'),activationid:'"+ activationid + "'}},{$limit:1}]}";
            var _model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            if (_model.GetValue("result").AsBsonArray.Count > 0)
            {
                q = "{update:'users',updates:[{q:{_id:ObjectId('" + id + "'),activationid:'" + activationid + "'},u:{$set:{status:1}}}]}";
                await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
                var model = _model.GetValue("result")[0];
                TempData["MSG"] = string.Format("کاربر گرامی {0} ، ثبت نام شما تکمیل شد.", model.AsBsonDocument.GetValue("name", ""));
                TempData["MSGT"] = "alert-success";
            }
            TempData["MSG"] = "اطلاعات نامعتبر است";
            TempData["MSGT"] = "alert-danger";
            return View("register");
        }
        [HttpGet]
        public async Task<ActionResult> resetepassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> resetepassword(string uname)
        {
            TempData["MSGT"] = "alert-danger";
            uname = (uname ?? "").ToLower();
            ViewBag.uname = uname;
            if (!this.IsCaptchaValid("Captcha is not valid"))
            {
                TempData["MSG"] = "کد امنیتی اشتباه است.";
                return View();
            }
            if (string.IsNullOrWhiteSpace(uname))
                TempData["MSG"] = "لطفا پست الکترونیک (نام کاربری) را وارد کنید";
            else
            {
                string q = "{aggregate:'users',pipeline:[{$match:{uname:'" + uname + "'}},{$limit:1}]}";
                var _model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
                if (_model.GetValue("result").AsBsonArray.Count > 0)
                {
                    var model = _model.GetValue("result")[0].AsBsonDocument;
                    TempData["MSG"] = string.Format("لینک بازیابی کلمه عبور به ایمیل {0} ارسال شد.", model.GetValue("uname", ""));
                    TempData["MSGT"] = "alert-success";
                    string emailbody = string.Format("برای بازیابی کلمه عبور لطفا روی لینک زیر کلیک نمایید. <br/> <a href='http://{0}/user/resetenewpassword/{1}/{2}/{3}' target='_blank'>لینک فعال سازی حساب کاربری</a>", Request.Url.Authority, model.GetValue("_id", ""), model.GetValue("activationid", ""), Guid.NewGuid());
                    await Common.SendEmail(Common.GetSettingValue("email").AsString, new List<string>() { model.GetValue("uname", "").AsString }, "بازیابی کلمه عبور", emailbody);
                }
                else
                {
                    TempData["MSG"] = string.Format("کاربری با ایمیل {0} در سیستم وجود ندارد.",uname);
                }
            }
            return RedirectToAction("resetepassword");
        }

        [HttpGet]
        public async Task<ActionResult> resetenewpassword(string id, string activationid, string tmpActivationid)
        {
            string q = "{aggregate:'users',pipeline:[{$match:{_id:ObjectId('" + id + "'),activationid:'" + activationid + "'}},{$limit:1}]}";
            var _model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            if (_model.GetValue("result").AsBsonArray.Count > 0)
            {
                q = "{update:'users',updates:[{q:{_id:ObjectId('" + id + "'),activationid:'" + activationid + "'},u:{$set:{status:1}}}]}";
                await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
                var model = _model.GetValue("result")[0].AsBsonDocument;
                ViewBag.activationid = model.GetValue("activationid", "");
                //TempData["MSG"] = string.Format("کاربر گرامی {0} ، ثبت نام شما تکمیل شد.", model.AsBsonDocument.GetValue("name", ""));
                //TempData["MSGT"] = "alert-success";
                return View(model);
            }
            TempData["MSG"] = "اطلاعات نامعتبر است";
            TempData["MSGT"] = "alert-danger";
            return View("resetepassword");
        }
        [HttpPost]
        public async Task<ActionResult> resetenewpasswordfrom(string id, string activationid,string password)
        {

            string q = "{aggregate:'users',pipeline:[{$match:{_id:ObjectId('" + id + "'),activationid:'" + activationid + "'}},{$limit:1}]}";
            var _model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
            if (_model.GetValue("result").AsBsonArray.Count > 0)
            {
                q = "{update:'users',updates:[{q:{_id:ObjectId('" + id + "'),activationid:'" + activationid + "'},u:{$set:{password:'"+ password + "'}}}]}";
                await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
                var model = _model.GetValue("result")[0];
                TempData["MSG"] = string.Format("کاربر گرامی {0} ،کلمه عبور تغییر کرد.", model.AsBsonDocument.GetValue("name", ""));
                TempData["MSGT"] = "alert-success";
            }
            else
            {
                TempData["MSG"] = "اطلاعات نامعتبر است";
                TempData["MSGT"] = "alert-danger";
            }
            return View("resetepassword");
        }

        [HttpGet, Authorize]
        public async Task<ActionResult> my()
        {
            await UC.InitializeAsync(this);
            return View(await UC.UserInfoAsync());
        }
    }
}