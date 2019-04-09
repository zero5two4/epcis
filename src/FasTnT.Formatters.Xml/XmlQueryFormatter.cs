﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using FasTnT.Formatters.Xml.Requests;
using FasTnT.Formatters.Xml.Validation;
using FasTnT.Model.Queries;

namespace FasTnT.Formatters.Xml
{
    public class XmlQueryFormatter : IQueryFormatter
    {
        public async Task<EpcisQuery> Read(Stream input, CancellationToken cancellationToken)
        {
            var document = await XmlDocumentParser.Instance.Load(input, cancellationToken);

            if (document.Root.Name.LocalName == "EPCISQueryDocument")
            {
                var element = document.Root.Element("EPCISBody").Elements().FirstOrDefault();

                if (element != null)
                {
                    if (element.Name == XName.Get("GetQueryNames", EpcisNamespaces.Query))
                        return new GetQueryNames();
                    if (element.Name == XName.Get("GetSubscriptionIDs", EpcisNamespaces.Query))
                        return new GetSubscriptionIds { QueryName = element.Element("queryName").Value };
                    if (element.Name == XName.Get("GetStandardVersion", EpcisNamespaces.Query))
                        return new GetStandardVersion();
                    if (element.Name == XName.Get("GetVendorVersion", EpcisNamespaces.Query))
                        return new GetVendorVersion();
                    if (element.Name == XName.Get("Poll", EpcisNamespaces.Query))
                        return XmlQueryParser.Parse(element);
                    if (element.Name == XName.Get("Subscribe", EpcisNamespaces.Query))
                        return XmlSubscriptionParser.ParseSubscription(element);
                    if (element.Name == XName.Get("Unsubscribe", EpcisNamespaces.Query))
                        return XmlSubscriptionParser.ParseUnsubscription(element);
                    throw new Exception($"Element not expected: '{element?.Name?.LocalName ?? null}'");
                }

                throw new Exception($"EPCISBody element must contain the query type.");
            }

            throw new Exception($"Element not expected: '{document.Root.Name.LocalName}'");
        }

        public async Task Write(EpcisQuery entity, Stream output, CancellationToken cancellationToken)
        {
            XDocument document = Write((dynamic)entity);
            var bytes = Encoding.UTF8.GetBytes(document.ToString(SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces)); 

            await output.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        private XDocument Write(GetQueryNames query) => Query(XName.Get("GetQueryNames", EpcisNamespaces.Query));
        private XDocument Write(GetSubscriptionIds query) => Query(XName.Get("GetSubscriptionIDs", EpcisNamespaces.Query));
        private XDocument Write(GetStandardVersion query) => Query(XName.Get("GetStandardVersion", EpcisNamespaces.Query));
        private XDocument Write(GetVendorVersion query) => Query(XName.Get("GetVendorVersion", EpcisNamespaces.Query));
        private XDocument Write(Poll query) => throw new NotImplementedException();

        private XDocument Query(XName queryName) => new XDocument(XName.Get("EPCISQueryDocument", EpcisNamespaces.Query), new XElement("EPCISBody", new XElement(queryName)));
    }
}
