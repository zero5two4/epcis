﻿using FasTnT.Commands.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Parsers.Soap
{
    [TestClass]
    public class WhenParsingSoapGetQueryNamesRequest : SoapParserTestBase
    {
        public override void Given()
        {
            SetRequest("<?xml version=\"1.0\" encoding=\"utf-8\"?><soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:urn=\"urn:epcglobal:epcis-query:xsd:1\"><soapenv:Body><urn:GetQueryNames/></soapenv:Body></soapenv:Envelope>");
        }

        [TestMethod]
        public void ItShouldReturnAGetQueryNamesRequest()
        {
            Assert.IsInstanceOfType(Result, typeof(GetQueryNamesRequest));
        }
    }
}
