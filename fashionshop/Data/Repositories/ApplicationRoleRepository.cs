using Data.Infrastructure;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IApplicationRoleRepository : IRepository<ApplicationRole>
    {
        IEnumerable<ApplicationRole> GetListRoleByGroupId(int groupId);
    }
    public class ApplicationRoleRepository : RepositoryBase<ApplicationRole>, IApplicationRoleRepository
    {
        public ApplicationRoleRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
        public IEnumerable<ApplicationRole> GetListRoleByGroupId(int groupId)
        {
            var query = from r in DbContext.ApplicationRoles
                        join rg in DbContext.ApplicationRoleGroups
                        on r.Id equals rg.RoleId
                        where rg.GroupId == groupId
                        select r;
            return query;
        }
    }
}
