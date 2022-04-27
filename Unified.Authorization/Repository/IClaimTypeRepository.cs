using ApplicationPOCO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public interface IClaimTypeRepository
    {        
        Task<Webresponse<IList<ClaimType>>> Select(Guid? ApplicationId, int? ModuleId = null);
    }
}