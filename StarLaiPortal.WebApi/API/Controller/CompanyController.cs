﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Dapper;
using StarLaiPortal.WebApi.Model;

namespace StarLaiPortal.WebApi.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private IConfiguration Configuration { get; }
        public CompanyController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                //return Ok(rtn.ToList());
                using (SqlConnection conn = new SqlConnection(Configuration.GetConnectionString("ConnectionString")))
                {
                    var val = conn.Query("exec sp_getdatalist 'Company'").ToList();
                    return Ok(JsonConvert.SerializeObject(val, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}
