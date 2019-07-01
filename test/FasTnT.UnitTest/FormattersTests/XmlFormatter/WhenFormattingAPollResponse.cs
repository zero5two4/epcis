﻿using FasTnT.Formatters.Xml;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.XmlFormatter
{
    [TestClass]
    public class WhenFormattingAPollResponse : BaseUnitTest
    {
        public XmlResponseFormatter Formatter { get; set; }
        public PollResponse GetVendorVersionResponse { get; set; }
        public XDocument Result { get; set; }

        public override void Arrange()
        {
            Formatter = new XmlResponseFormatter();
            GetVendorVersionResponse = new PollResponse { QueryName = "SimpleEventQuery", SubscriptionId = "subscriptionID" };
        }

        public override void Act() => Result = Formatter.Format(GetVendorVersionResponse);

        [Assert]
        public void TheXMLDocumentShouldNotBeNull()
        {
            Assert.IsNotNull(Result);
        }

        [Assert]
        public void TheXMLDocumentShouldContainAnEPCISQueryDocumentRoot()
        {
            Assert.AreEqual("EPCISQueryDocument", Result.Root.Name.LocalName);
        }

        [Assert]
        public void TheXMLDocumentRootShouldContainTheEPCISAttributes()
        {
            Assert.IsNotNull(Result.Root.Attributes().Where(x => x.Name == "creationDate").FirstOrDefault());
            Assert.IsNotNull(Result.Root.Attributes().Where(x => x.Name == "schemaVersion").FirstOrDefault());
        }

        [Assert]
        public void TheXMLDocumentShouldContainAnEPCISBodyElement()
        {
            Assert.IsNotNull(Result.Root.Element("EPCISBody"));
        }

        public void TheXMLDocumentShouldContainAGetVendorVersionResultElement()
        {
            Assert.IsNotNull(Result.Root.Element("EPCISBody").Element(XName.Get("QueryResults", "urn:epcglobal:epcis-query:xsd:1")).Value);
        }
    }
}
