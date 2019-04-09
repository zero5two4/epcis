﻿using FasTnT.Formatters;
using FasTnT.Model.Responses;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Subscriptions
{
    public class HttpSubscriptionResultSender : ISubscriptionResultSender
    {
        private readonly FormatterProvider _formatterFactory;

        public HttpSubscriptionResultSender(FormatterProvider formatterFactory)
        {
            _formatterFactory = formatterFactory;
        }
        public async Task Send(string destination, IEpcisResponse epcisResponse, string contentType, CancellationToken cancellationToken)
        {
            var responseFormatter = _formatterFactory.GetFormatter<IEpcisResponse>(contentType) as IResponseFormatter;
            var request = WebRequest.Create($"{destination}{GetCallbackUrl(epcisResponse)}");
            request.Method = "POST";
            request.ContentType = responseFormatter.ToContentTypeString();

            using (var stream = await request.GetRequestStreamAsync())
            {
                await responseFormatter.Write(epcisResponse, stream, cancellationToken);
            }

            var response = await request.GetResponseAsync();
            // TODO: use response
        }

        private string GetCallbackUrl(IEpcisResponse response)
        {
            if(response is PollResponse) return "CallbackResults";
            if ((response is ExceptionResponse res) && res.Exception == "QueryTooLargeException") return "CallbackQueryTooLargeException";

            return "CallbackImplementationException"; // In every other case.
        }
    }
}
