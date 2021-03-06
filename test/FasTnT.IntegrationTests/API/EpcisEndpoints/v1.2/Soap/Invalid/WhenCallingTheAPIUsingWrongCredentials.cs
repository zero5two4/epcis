﻿using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FasTnT.IntegrationTests.API.EpcisEndpoints.v1._2.XML.Invalid
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class WhenCallingTheAPIUsingWrongCredentials : BaseIntegrationTest
    {
        public override async Task Act()
        {
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtaW46SW52YWxpZFB3ZA==");
            Result = await Client.PostAsync("/v1_2/Query.svc", new StringContent(File.ReadAllText("Requests/Queries/Poll.xml"), Encoding.UTF8, "application/xml"));
        }

        [Assert]
        public void ItShouldReturnHttp401Unauthorized() => Assert.AreEqual(HttpStatusCode.Unauthorized, Result.StatusCode);
    }
}
