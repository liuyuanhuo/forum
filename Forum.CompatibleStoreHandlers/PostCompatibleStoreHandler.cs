using ECommon.Components;
using ECommon.Dapper;
using ENode.Domain.CompatibleStore;
using ENode.Eventing;
using ENode.Infrastructure;
using Forum.Domain.Posts;
using Forum.Domain.Replies;
using Forum.Infrastructure;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Forum.CompatibleStoreHandler
{
    [Component]
    public class PostCompatibleStoreHandler : ICompatibleStoreHandler<Post>, ICompatibleStoreHandler<Reply>
    {
        public CompatibleStyle GetCompatibleStyle(Post nullObject)
        {
            return CompatibleStyle.RepositoryOnly;
            //return CompatibleStyle.RepositoryThenEventSourcing;
            //return CompatibleStyle.EventSourcingOnly;
        }
        public DomainEventStream GetAggregateRestoreEventStream(string postId, Post nullObject)
        {
            using (var connection = new SqlConnection(ConfigSettings.ConnectionString))
            {
                dynamic post = connection.QueryList<dynamic>(new { Id = postId }, Constants.PostTable).SingleOrDefault();
                PostCreatedEvent postCreatedEvent = null;

                postCreatedEvent = new PostCreatedEvent(
                      (string)post.Id,(int)post.Version, (string)post.Subject, (string)post.Body, (string)post.SectionId, (string)post.AuthorId);
                postCreatedEvent.Version = (int)post.Version;
                postCreatedEvent.Timestamp = (DateTime)post.CreatedOn;

                PostReplyStatisticInfo postReplyStatisticInfo = new PostReplyStatisticInfo(
                    (string)post.MostRecentReplyId, (string)post.MostRecentReplierId, (DateTime)post.LastUpdateTime, (int)post.ReplyCount);
                PostReplyStatisticInfoChangedEvent postReplyStatisticInfoChangedEvent = new PostReplyStatisticInfoChangedEvent(
                      (string)post.Id, (int)post.Version, postReplyStatisticInfo);
                postReplyStatisticInfoChangedEvent.Version = (int)post.Version;
                postReplyStatisticInfoChangedEvent.Timestamp = (DateTime)post.LastUpdateTime;

                IDomainEvent[] events = new IDomainEvent[] { postCreatedEvent, postReplyStatisticInfoChangedEvent };
                DomainEventStream eventStream = new DomainEventStream(postId, postCreatedEvent.Version, events);
                return eventStream;
            }
        }
        public bool SaveAggregateRoot(Post aggregateRoot)
        {
            return true;
        }


        public CompatibleStyle GetCompatibleStyle(Reply nullObject)
        {
            return CompatibleStyle.EventSourcingOnly;
        }
        public DomainEventStream GetAggregateRestoreEventStream(string postId, Reply nullObject)
        {
            return null;
        }
        public bool SaveAggregateRoot(Reply aggregateRoot)
        {
            return true;
        }
    }
}
