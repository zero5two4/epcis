﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model;
using MoreLinq;
using StoreAction = System.Func<FasTnT.Model.EpcisEventDocument, FasTnT.Persistence.Dapper.DapperUnitOfWork, System.Threading.Tasks.Task>;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlEventStore : IEventStore
    {
        private readonly DapperUnitOfWork _unitOfWork;
        private readonly IEnumerable<StoreAction> _actions = new StoreAction[]{ StoreRequest, StoreEvents, StoreEpcs, StoreCustomFields, StoreSourceDestinations, StoreBusinessTransactions, StoreErrorDeclaration };

        public PgSqlEventStore(DapperUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Store(EpcisEventDocument request)
        {
            foreach (var action in _actions)
            {
                await action(request, _unitOfWork);
            }
        }

        private async static Task StoreRequest(EpcisEventDocument request, DapperUnitOfWork unitOfWork)
        {
            await unitOfWork.Execute(SqlRequests.StoreRequest, new { request.Id, DocumentTime = request.CreationDate, RecordTime = DateTime.UtcNow });
        }

        private async static Task StoreEvents(EpcisEventDocument request, DapperUnitOfWork unitOfWork)
        {
            request.EventList.ForEach(x => x.RequestId = request.Id);
            await unitOfWork.Execute(SqlRequests.StoreEvent, request.EventList);
        }

        private async static Task StoreEpcs(EpcisEventDocument request, DapperUnitOfWork unitOfWork)
        {
            request.EventList.ForEach(x => x.Epcs.ForEach(e => e.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreEpcs, request.EventList.SelectMany(x => x.Epcs));
        }

        private async static Task StoreCustomFields(EpcisEventDocument request, DapperUnitOfWork unitOfWork)
        {
            request.EventList.ForEach(x => x.CustomFields.ForEach(f => f.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreCustomField, request.EventList.SelectMany(x => x.CustomFields));
        }

        private async static Task StoreSourceDestinations(EpcisEventDocument request, DapperUnitOfWork unitOfWork)
        {
            request.EventList.ForEach(x => x.SourceDestinationList.ForEach(s => s.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreSourceDestination, request.EventList.SelectMany(x => x.SourceDestinationList));
        }

        private async static Task StoreBusinessTransactions(EpcisEventDocument request, DapperUnitOfWork unitOfWork)
        {
            request.EventList.ForEach(x => x.BusinessTransactions.ForEach(t => t.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreBusinessTransaction, request.EventList.SelectMany(x => x.BusinessTransactions));
        }

        private async static Task StoreErrorDeclaration(EpcisEventDocument request, DapperUnitOfWork unitOfWork)
        {
            var eventsWithErrorDeclaration = request.EventList.Where(x => x.ErrorDeclaration != null);

            eventsWithErrorDeclaration.ForEach(x => x.ErrorDeclaration.EventId = x.Id);
            eventsWithErrorDeclaration.ForEach(x => x.ErrorDeclaration.CorrectiveEventIds.ForEach(t => t.EventId = x.Id));

            await unitOfWork.Execute(SqlRequests.StoreErrorDeclaration, eventsWithErrorDeclaration.Select(x => x.ErrorDeclaration));
            await unitOfWork.Execute(SqlRequests.StoreErrorDeclarationIds, eventsWithErrorDeclaration.SelectMany(x => x.ErrorDeclaration.CorrectiveEventIds));
        }
    }
}