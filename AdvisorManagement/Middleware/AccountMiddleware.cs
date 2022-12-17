﻿using AdvisorManagement.Models;
using AdvisorManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AdvisorManagement.Middleware
{
    public class AccountMiddleware
    {

        private CP25Team09Entities db = new CP25Team09Entities();
     

        // Get Role
        public string getRoles(string roles)
        {
            var sql = db.AccountUser.FirstOrDefault(x => x.user_code == roles);
            if (sql != null)
            {
                return db.Role.SingleOrDefault(x => x.id == sql.id).role_name;
            }
            return db.Role.SingleOrDefault(x => x.id == 4).role_name;
        }

        // Register Profile
        public void UserProfile(ClaimsIdentity logData)
        {
            var objectTest = logData;
            string[] data = objectTest.Claims.Where(x => x.Type == "name").First().Value.Split('-');
            AccountInitModel itemAccount = new AccountInitModel();
            string [] mailGet = logData.Name.Trim().Split('@');

            if (mailGet[mailGet.Length - 1] == "vlu.edu.vn")
            {
                itemAccount.role_id = 2;
                itemAccount.user_name = data[0].Trim();
                itemAccount.user_code = mailGet[0] + "_cntt";
                writeRecordUser(itemAccount, logData.Name);
                Advisor advisor = new Advisor();
                advisor.advisor_code = itemAccount.user_code;
                advisor.account_id = getID();
                advisor.status_id = 1;
                db.Advisor.Add(advisor);
            }
            else
            {
                itemAccount.role_id = 3;
                itemAccount.user_code = data[0].Trim();
                itemAccount.user_name = data[1].Trim();
                writeRecordUser(itemAccount, logData.Name);
                Student student = new Student();
                student.student_code = itemAccount.user_code;
                student.account_id = getID();
                student.status_id = 1;
                db.Student.Add(student);
            }
            db.SaveChanges();
        }

        // write init user
        public void writeRecordUser(AccountInitModel data,string mail)
        {
            AccountUser accountUser = new AccountUser();
            accountUser.id = getID()+1;
            accountUser.email = mail;
            accountUser.id_role = data.role_id;
            accountUser.user_code = data.user_code;
            accountUser.user_name = data.user_name;
            accountUser.create_time = DateTime.Now;
            accountUser.update_time = DateTime.Now;
            db.AccountUser.Add(accountUser);
            db.SaveChanges();
        }
        // get new id 
        public int getID()
        {
            var lastRecordUser =  db.AccountUser.
               OrderByDescending(s => s.id).
               Select(user => new
               {
                   Id = user.id,
               }).FirstOrDefault();
            return (lastRecordUser != null) ? (int)lastRecordUser.Id : 0;
        }
        // get Roles for Prodile
        public bool getPermission(string userMail,string routeAccess)
        {
            var seeMenu = (from pq in db.AccountUser
                           join mnrole in db.RoleMenu on pq.id_role equals mnrole.id_role
                           join mn in db.Menu on mnrole.id_menu equals mn.id
                           where pq.email == userMail && pq.id_role == mnrole.id_role && mn.action_link == routeAccess
                           select new Models.ViewModel.MenuItem
                           {      
                               actionLink = mn.action_link
                           });
            return (seeMenu.FirstOrDefault(x => x.actionLink == routeAccess) != null ) ? true : false;

        }
    }
}