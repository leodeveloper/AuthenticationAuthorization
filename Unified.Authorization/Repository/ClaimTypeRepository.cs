using ApplicationPOCO;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public class ClaimTypeRepository : IClaimTypeRepository
    {

        readonly IOptions<UserManagerConStrings> _appsetting;

        public ClaimTypeRepository(IOptions<UserManagerConStrings> appsetting)
        {
            _appsetting = appsetting;
        }

        private IDbConnection GetDbConnection()
        {
            return new SqlConnection(_appsetting.Value.DBConnectionString);
        }


        /// <summary>
        /// proc ClaimType_Select
        /// </summary>
        /// <param name="ApplicationId">@ApplicationId uniqueidentifier = null</param>
        /// <param name="ModuleId">@ModuleId int = null</param>
        /// <param name="Non_Moduled">@ApplyModuleIdNull bit = 0</param>
        /// <returns></returns>
        public async Task<Webresponse<IList<ClaimType>>> Select(Guid? ApplicationId, int? ModuleId = null)
        {
            var result = new Webresponse<IList<ClaimType>>
            {
                status = APIStatus.processing
            };

            try
            {
                using (var con = GetDbConnection())
                {                   

                    var data = await con.QueryAsync<ClaimType>("ClaimType_Select", 
                        param:new { ApplicationId, ModuleId },
                        commandType: CommandType.StoredProcedure);

                    result.data = data.ToList();
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(ClaimTypeRepository)}:{nameof(Select)}:: Error message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }

    }

}
