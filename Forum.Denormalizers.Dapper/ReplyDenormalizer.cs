﻿using System.Threading.Tasks;
using ECommon.Components;
using ECommon.Dapper;
using ENode.Eventing;
using ENode.Infrastructure;
using Forum.Domain.Replies;
using Forum.Infrastructure;
using ECommon.IO;

namespace Forum.Denormalizers.Dapper
{
    [Component]
    public class ReplyDenormalizer : AbstractDenormalizer,
        IMessageHandler<ReplyCreatedEvent>,
        IMessageHandler<ReplyBodyChangedEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(ReplyCreatedEvent evnt)
        {
            return TryInsertRecordAsync(connection =>
            {
                return connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    PostId = evnt.PostId,
                    ParentId = evnt.ParentId,
                    AuthorId = evnt.AuthorId,
                    Body = evnt.Body,
                    CreatedOn = evnt.Timestamp,
                    UpdatedOn = evnt.Timestamp,
                    Version = evnt.Version
                }, Constants.ReplyTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(ReplyBodyChangedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Body = evnt.Body,
                    UpdatedOn = evnt.Timestamp,
                    Version = evnt.Version
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, Constants.ReplyTable);
            });
        }
    }
}
