﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Dapper;
using StarLaiPortal.WebApi.Model;
using DevExpress.ExpressApp.Security;
using StarLaiPortal.Module.BusinessObjects.View;
using StarLaiPortal.Module.BusinessObjects;
using StarLaiPortal.Module.BusinessObjects.Sales_Order;
using DevExpress.Data.Filtering;

namespace StarLaiPortal.WebApi.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OpenSOController : ControllerBase
    {
        private IConfiguration Configuration { get; }
        IObjectSpaceFactory objectSpaceFactory;
        ISecurityProvider securityProvider;
        public OpenSOController(IConfiguration configuration, IObjectSpaceFactory objectSpaceFactory, ISecurityProvider securityProvider)
        {
            this.objectSpaceFactory = objectSpaceFactory;
            this.securityProvider = securityProvider;
            this.Configuration = configuration;
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                //using IObjectSpace newObjectSpace = objectSpaceFactory.CreateObjectSpace<vwOpenSO>();
                //ISecurityStrategyBase security = securityProvider.GetSecurity();
                //var userId = security.UserId;
                //var userName = security.UserName;
                //ApplicationUser user = newObjectSpace.GetObjectByKey<ApplicationUser>(userId);

                //List<vwOpenSO> obj = newObjectSpace.GetObjects<vwOpenSO>().ToList();
                //var rtn = obj.Select(pp => new { OID = pp.PriKey, Cart = pp.Cart, Customer = pp.Customer, ContactNo = pp.ContactNo, DocNum = pp.DocNum, CreateDate = pp.CreateDate });
                ////return Ok(rtn.ToList());
                //string json = JsonConvert.SerializeObject(rtn, Formatting.Indented);
                //return Ok(json);

                using (SqlConnection conn = new SqlConnection(Configuration.GetConnectionString("ConnectionString")))
                {
                    var val = conn.Query("exec sp_getdatalist 'OpenSO'").ToList();
                    return Ok(JsonConvert.SerializeObject(val, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpGet("oid")]
        public IActionResult Get(int oid)
        {
            try
            {
                //using IObjectSpace newObjectSpace = objectSpaceFactory.CreateObjectSpace<SalesOrderDetails>();
                //ISecurityStrategyBase security = securityProvider.GetSecurity();
                //var userId = security.UserId;
                //var userName = security.UserName;
                //ApplicationUser user = newObjectSpace.GetObjectByKey<ApplicationUser>(userId);

                //List<SalesOrderDetails> obj = newObjectSpace.GetObjects<SalesOrderDetails>(CriteriaOperator.Parse("SalesOrder=?", oid)).ToList();
                //var rtn = obj.Select(pp => new { OID = pp.Oid, ItemCode = pp.ItemCode, ItemDesc = pp.ItemDesc, Model = pp.Model, Location = pp.Location.WarehouseCode, Quantity = pp.Quantity });
                ////return Ok(rtn.ToList());
                //string json = JsonConvert.SerializeObject(rtn, Formatting.Indented);
                //return Ok(json);

                using (SqlConnection conn = new SqlConnection(Configuration.GetConnectionString("ConnectionString")))
                {
                    string json = JsonConvert.SerializeObject(new { oid = oid });
                    var val = conn.Query($"exec sp_getdatalist 'SalesOrderDetails', '{json}'").ToList();
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
