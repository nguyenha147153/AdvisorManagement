﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using AdvisorManagement.Middleware;
using AdvisorManagement.Models;

namespace AdvisorManagement.Areas.Admin.Controllers
{
    // auth
    [LoginFilter]
    public class AccountUsersController : Controller
    {
        // check
        private CP25Team09Entities db = new CP25Team09Entities();
        private MenuMiddleware serviceMenu = new MenuMiddleware();
        private AccountMiddleware serviceAccount = new AccountMiddleware();
        private MailServicesMiddleware serviceMail = new MailServicesMiddleware();
        private string routePermission = "Admin/AccountUsers";
        private string picture;
        public void init()
        {
            ViewBag.menu = serviceMenu.getMenu(User.Identity.Name);
            ViewBag.avatar = serviceAccount.getAvatar(User.Identity.Name);
        } 
        //GET: Admin/AccountUsers
        public ActionResult Index()
        {
            if (serviceAccount.getPermission(User.Identity.Name, routePermission))
            {
                this.init();
                var accountUser = db.AccountUser.Include(a => a.Role).Where(x => x.id_role != 1 && x.id_role != 3).OrderBy(y => y.id_role);
                ViewBag.Name = serviceAccount.getTextName(User.Identity.Name);
                ViewBag.RoleName = serviceAccount.getRoleTextName(User.Identity.Name);
                return View(accountUser.ToList());
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.ProxyAuthenticationRequired);
            }
        }
        // API
        // GET: Admin/AccountUsers/Details/5
        [HttpGet]
        public ActionResult Details(int? id)    
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                AccountUser user = db.AccountUser.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                return Json(new { success = true,
                    detail_code = user.user_code,
                    detail_name = user.user_name,
                    detail_street = user.address,
                    detail_mail = user.email,
                    detail_phone = user.phone,
                    detail_img = user.img_profile != null ?  user.img_profile.ToString() : "/Images/imageProfile/avata.png",
                    id_detailUser = user.id, message = "Lấy thông tin thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Lấy thông tin thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CreateApi([Bind(Include = " email,user_name,phone,address ")]AccountUser account)
        {
            
            if (serviceAccount.getPermission(User.Identity.Name, routePermission)  )
            {
                if (account.email == null)
                {
                    return Json(new { success = false, message = "Vui lòng điền mail" });
                }
                if (account.user_name == null)
                {
                    return Json(new { success = false, message = "Vui lòng điền tên cố vấn" });
                }
                if (account.phone == null)
                {
                    return Json(new { success = false, message = "Vui lòng điền số điện thoại" });
                }
                if (account.address == null)
                {
                    return Json(new { success = false, message = "Vui lòng điền địa chỉ" });
                }
                if (!serviceAccount.IsValidEmail(account.email))
                {
                    return Json(new { success = false, message = "Email không đúng định dạng" });
                }
                if (!serviceAccount.IsPhoneNumberValid(account.phone))
                {
                    return Json(new { success = false, message = "Số điện thoại không đúng định dạng" });
                }
                var userCheck = db.AccountUser.Where(x => x.email == account.email).ToList().Count();
                if (userCheck > 0)
                {
                    return Json(new { success = false, message = "Email tồn tại trong hệ thông" });
                }
                string mess =  serviceAccount.AdvisorProfile(account.email, account);
                return Json(new { success = true, message = mess });

            }
            else
            {
                return Json(new { success = false, message = "Sai phân quyền" });
            }
        }
        // DELETE API
        public ActionResult DeleteApi(int id)
        {
            if (serviceAccount.getPermission(User.Identity.Name, routePermission))
            {
                var userCheck = db.AccountUser.FirstOrDefault(x => x.id == id);
                if (userCheck.id == null)
                {
                    return Json(new { success = false, message = "Cố vấn không tồn tại trong hệ thông" });
                }
                var classCheck = db.VLClass.Where(y => y.advisor_code == userCheck.user_code).ToList().Count();
                var adCheck = db.Advisor.FirstOrDefault(x => x.advisor_code == userCheck.user_code);
                if (classCheck > 0)
                {
                    adCheck.status_id = 0;
                    db.Entry(adCheck).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Tài khoản tạm khoá do giảng viên có chủ nhiệm lớp" });
                }
                else
                {
                    if (adCheck != null)
                    {
                        db.Advisor.Remove(adCheck);
                        db.SaveChanges();
                    }
                    db.AccountUser.Remove(userCheck);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Tài khoản cố vấn đã xoá khỏi hệ thống"});
                }

            }
            else
            {
                return Json(new { success = false, message = "Sai phân quyền" });
            }
        }
        // EDIT API 
        [HttpPost]
        public ActionResult EditApi([Bind(Include = " id,email,user_name,phone,address ")] AccountUser account)
        {

            if (serviceAccount.getPermission(User.Identity.Name, routePermission))
            {
                var edtUser = db.AccountUser.FirstOrDefault(x => x.id == account.id);
                if (account.user_name == null)
                {
                    return Json(new { success = false, message = "Vui lòng điền tên cố vấn" });
                }
                if (account.phone == null)
                {
                    return Json(new { success = false, message = "Vui lòng điền số điện thoại" });
                }
                if (account.address == null)
                {
                    return Json(new { success = false, message = "Vui lòng điền địa chỉ" });
                }
                if (!serviceAccount.IsPhoneNumberValid(account.phone))
                {
                    return Json(new { success = false, message = "Số điện thoại không đúng định dạng" });
                }
              
                if (edtUser == null)
                {
                    return Json(new { success = false, message = "Email không tồn tại trong hệ thông" });
                }
                edtUser.user_name = account.user_name;
                edtUser.phone = account.phone;
                edtUser.address = account.address;
                db.Entry(edtUser).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công tài khoản của cố vấn" });
            }
            else
            {
                return Json(new { success = false, message = "Sai phân quyền" });
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
