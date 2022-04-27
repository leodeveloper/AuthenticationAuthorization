using ApplicationPOCO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public interface IModuleRepository
    {
        Task<Webresponse<IList<Module>>> Select(Guid? ApplicationId);
    }
}