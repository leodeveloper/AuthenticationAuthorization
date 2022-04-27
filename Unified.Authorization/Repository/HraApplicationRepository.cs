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
    public class HraApplicationRepository : IHraApplicationRepository
    {

        readonly IOptions<UserManagerConStrings> _appsetting;

        public HraApplicationRepository(IOptions<UserManagerConStrings> appsetting)
        {
            _appsetting = appsetting;
        }

        private IDbConnection GetDbConnection()
        {
            return new SqlConnection(_appsetting.Value.DBConnectionString);
        }

        /// <summary>
        /// Get all applications from the database
        /// </summary>
        /// <param name="conString"></param>
        /// <returns></returns>
        public async Task<Webresponse<IList<HraApplications>>> GetAll()
        {
            var result = new Webresponse<IList<HraApplications>>
            {
                data = new List<HraApplications>(),
                status = APIStatus.processing
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    var data = await con.QueryAsync<HraApplications>("Select * from HraApplications", commandType: CommandType.Text);
                    result.data = data.AsList();
                    result.status = APIStatus.success;
                }
            }
            catch (Exception ex)
            {
                Log.Error("HraApplicationRepository:GetAll:: Error message => {0}", ex.Message);

                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }

    }
}
