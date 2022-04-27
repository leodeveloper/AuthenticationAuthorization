using ApplicationPOCO;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public class Role_App_ClaimsRepository : IRole_App_ClaimsRepository
    {

        readonly IOptions<UserManagerConStrings> appsetting;

        public Role_App_ClaimsRepository(IOptions<UserManagerConStrings> appsetting)
        {
            this.appsetting = appsetting;
        }


        private IDbConnection GetDbConnection()
        {
            return new SqlConnection(appsetting.Value.DBConnectionString);
        }

        /// <summary>
        /// Returns all needed data to edit the claims for the selected (application and role)
        /// </summary>
        /// <param name="ApplicationId"></param>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public async Task<Webresponse<Role_App_ClaimsEdit>> Get(Guid ApplicationId, string RoleId)
        {

            var result = new Webresponse<Role_App_ClaimsEdit>
            {
                status = APIStatus.processing
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    // Get claim types for the selected app (role_app.ApplicationId)

                    var claimTypes = (await con.QueryAsync<ClaimType>("ClaimType_Select",
                        param: new { ApplicationId }, commandType: CommandType.StoredProcedure)).ToList();

                    var role = con.QueryFirst<ApplicationRole>($"select top 1 * from AspNetRoles where Id = '{ RoleId }'",
                        commandType: CommandType.Text);

                    var app = con.QueryFirst<HraApplications>($"select top 1 * from HraApplications where ApplicationId = '{ ApplicationId }'",
                        commandType: CommandType.Text);

                    var grantedClaims = con.Query<ClaimType>("GetGrantedClaims", param: new { RoleId, ApplicationId },
                        commandType: CommandType.StoredProcedure).ToList();

                    result.data = new Role_App_ClaimsEdit
                    {
                        App = app,
                        Role = role,
                        ClaimTypes = claimTypes,
                        GrantedClaimTypes = grantedClaims
                    };

                    result.status = APIStatus.success;
                }

            }
            catch (Exception ex)
            {
                Log.Error("Role_App_Claims:Get:: Error message => {0}", ex.Message);
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }

        public async Task<WebresponseNoData> Save(Role_App_ClaimsModify Model)
        {
            
            var result = new WebresponseNoData
            {
                status = APIStatus.processing
            };

            try
            {
                // Get the database connection
                using (var con = GetDbConnection())
                {
                    await con.ExecuteAsync("UpdateRole_Claims",
                    param: new
                    {
                        Model.RoleId,
                        Model.ApplicationId,
                        TVP_Grant = (new DataTableIntID(Model.GrantIds)).AsTableValuedParameter("dbo.IntID")
                    },
                    commandType: CommandType.StoredProcedure);
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error("RoleRepository:GetById:: Error message => {0}", ex.Message);
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }

    }

}
