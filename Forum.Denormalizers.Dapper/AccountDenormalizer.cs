﻿using System.Threading.Tasks;
using ECommon.Components;
using ECommon.Dapper;
using ENode.Infrastructure;
using Forum.Domain.Accounts;
using Forum.Infrastructure;
using ECommon.IO;

namespace Forum.Denormalizers.Dapper
{
    [Component]
    public class AccountDenormalizer : AbstractDenormalizer, IMessageHandler<NewAccountRegisteredEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(NewAccountRegisteredEvent evnt)
        {
            return TryInsertRecordAsync(connection =>
            {
                return connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    Name = evnt.Name,
                    Password = evnt.Password,
                    CreatedOn = evnt.Timestamp,
                    UpdatedOn = evnt.Timestamp,
                    Version = evnt.Version
                }, Constants.AccountTable);
            });
        }
    }
}
