using ApplicationPOCO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public interface IHraApplicationRepository
    {
        Task<Webresponse<IList<HraApplications>>> GetAll();
    }
}