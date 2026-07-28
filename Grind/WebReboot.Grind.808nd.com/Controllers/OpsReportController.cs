using MongoDB.Driver;
using Revel._808nd.com.OperationsReport.Models;
using Revel._808nd.com.OperationsReport.Mongo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebReboot.Grind._808nd.com.Controllers
{
    public class OpsReportController : Controller
    {
        // GET: OpsReport
        public ActionResult WhereAreWe()
        {
            var _connectionString = ConfigurationManager.ConnectionStrings["GrindMongoOps"].ToString();
            var _databaseName = MongoUrl.Create(_connectionString).DatabaseName;

            MongoClient mongoClient;
            IMongoDatabase mongoDb;
            IMongoCollection<OpsReportHourlyWrapper> collection;

            mongoClient = new MongoClient(_connectionString);//use local
            mongoDb = mongoClient.GetDatabase(_databaseName);
            collection = mongoDb.GetCollection<OpsReportHourlyWrapper>(OpsMongoDbStrings.OpsReportCollectionName);

            var resocrd = collection.AsQueryable().Where(x => x.containerStart < new DateTime(2018, 05, 01)).OrderByDescending(x => x.containerEnd).FirstOrDefault().containerEnd;

            ViewBag.Date = resocrd;
            return View();
        }
    }
}