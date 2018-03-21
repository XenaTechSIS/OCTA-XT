using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using FSP.Domain.Model;
using FSP.Web.Models;

namespace FSP.Web.Helpers
{
    public class FspCommon : IDisposable
    {
        public void Dispose()
        {
        }

        public List<Contractor> GetContractors()
        {
            var returnList = new List<Contractor>();

            if (HttpContext.Current.Cache["contractors"] == null)
            {
                using (var dc = new FSPDataContext())
                {
                    returnList = dc.Contractors.ToList();
                }

                HttpContext.Current.Cache.Insert("contractors",
                    returnList,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(60));
            }
            else
            {
                returnList = (List<Contractor>) HttpContext.Current.Cache["contractors"];
            }

            return returnList;
        }

        public List<UIUser> GetContractorUsers()
        {
            var returnList = new List<UIUser>();
            if (HttpContext.Current.Cache["contractorUsers"] == null)
            {
                using (var dc = new FSPDataContext())
                {
                    var userRoleQuery = from q in dc.Users
                        join r in dc.Roles on q.RoleID equals r.RoleID
                        join c in dc.Contractors on q.ContractorID equals c.ContractorID
                        select new UIUser
                        {
                            Email = q.Email,
                            RoleName = q.Role.RoleName,
                            ContractorID = q.ContractorID
                        };
                    returnList = userRoleQuery.ToList();
                }

                HttpContext.Current.Cache.Insert("contractorUsers",
                    returnList,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(60));
            }
            else
            {
                returnList = (List<UIUser>) HttpContext.Current.Cache["contractorUsers"];
            }

            return returnList;
        }
    }
}