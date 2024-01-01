using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;
using BurgerKing.Models;

namespace BurgerKing
{
    public class WebRoleProvider: RoleProvider
    {
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string EmailOrPhone)
        {
            using (BurgerKingDBContext context = new BurgerKingDBContext())
            {
                if (IsEmail(EmailOrPhone))
                {
                    var result = (from account in context.Accounts
                                  join role in context.Roles on account.RoleId equals role.RoleId
                                  where account.Email == EmailOrPhone
                                  select role.RoleName).ToArray();

                    return result;
                }
                else if (IsPhoneNumber(EmailOrPhone))
                {
                    var result = (from account in context.Accounts
                                  join role in context.Roles on account.RoleId equals role.RoleId
                                  where account.Phone == EmailOrPhone
                                  select role.RoleName).ToArray();

                    return result;
                }
                else
                {
                    // Xử lý trường hợp không phải email hoặc số điện thoại
                    return new string[] { "DefaultRole" };
                }
            }
        }

        private bool IsEmail(string input)
        {
            return new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(input);
        }

        private bool IsPhoneNumber(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^\d{10,15}$");
        }


        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}