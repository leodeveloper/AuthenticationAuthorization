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
    public class ModuleRepository : IModuleRepository
    {

        #region << Fields >>

        readonly IOptions<UserManagerConStrings> _appsetting;

        #endregion

        #region << Constructors >>

        public ModuleRepository(IOptions<UserManagerConStrings> appsetting)
        {
            _appsetting = appsetting;
        }

        #endregion

        #region << Methods >>

        private IDbConnection GetDbConnection()
        {
            return new SqlConnection(_appsetting.Value.DBConnectionString);
        }

        public async Task<Webresponse<IList<Module>>> Select(Guid? ApplicationId)
        {
            var result = new Webresponse<IList<Module>>
            {
                status = APIStatus.processing
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    var data = await con.QueryAsync<Module>("Module_Select", param: new { ApplicationId },
                        commandType: CommandType.StoredProcedure);

                    result.data = data.ToList();
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(ModuleRepository)}:{nameof(Select)}:: Error message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }


        #endregion


    }
}
