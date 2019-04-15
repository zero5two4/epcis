﻿using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using FasTnT.Model;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace FasTnT.UnitTest.XmlFormatter
{
    [TestClass]
    public class WhenParsingAnInvalidEventRequest : BaseUnitTest
    {
        public IRequestFormatter RequestParser { get; set; }
        public Stream InputFile { get; set; }
        public Request ParsedFile { get; set; }
        public Exception Catched { get; set; }
        public CaptureRequest ParsedEvents => ParsedFile as CaptureRequest;

        public override void Arrange()
        {
            RequestParser = new XmlRequestFormatter();
            InputFile = File.OpenRead("XmlFormatter/files/requests/xml/invalid_event_1.xml");
        }
        public override void Act()
        {
            try
            {
                ParsedFile = RequestParser.Read(InputFile, default).Result as CaptureRequest;
            }
            catch(Exception ex)
            {
                Catched = ex;
            }
        }

        [Assert]
        public void ItShouldThrowAnException() => Assert.IsNotNull(Catched);
    }
}