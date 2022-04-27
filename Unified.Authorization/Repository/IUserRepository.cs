using ApplicationPOCO;
using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public interface IUserRepository
    {
        Task<Webresponse<IdentityResult>> Add(ApplicationUser Identity);
        Task<Webresponse<IdentityResult>> Lock(string UserId, bool Disabled);
        Task<WebresponseNoData> Update(UserModify model);
        Webresponse<IList<ApplicationUser>> GetAll();

        Task<Webresponse<ApplicationUser>> GetById(string UserId);
        
        Task<Webresponse<UserEdit>> GetEditingData(string UserId);
        Task<Webresponse<bool>> IsPartOfRole(ApplicationUser User, string RoleName);


    }
}