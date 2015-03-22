﻿using System;
using ENode.Eventing;

namespace Forum.Domain.Posts
{
    /// <summary>表示帖子已创建的领域事件
    /// </summary>
    [Serializable]
    public class PostCreatedEvent : DomainEvent<string>
    {
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public string SectionId { get; private set; }
        public string AuthorId { get; private set; }

        private PostCreatedEvent() { }
        public PostCreatedEvent(Post post, string subject, string body, string sectionId, string authorId) : base(post)
        {
            Subject = subject;
            Body = body;
            SectionId = sectionId;
            AuthorId = authorId;
        }

        public PostCreatedEvent(string id, int version, string subject, string body, string sectionId, string authorId)
            : base(id, version)
        {
            Subject = subject;
            Body = body;
            SectionId = sectionId;
            AuthorId = authorId;
        }
    }
}
