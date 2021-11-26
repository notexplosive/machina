using System;

namespace Machina.Engine
{
    using System.Collections.Generic;

    public class LogBuffer
    {
        private Queue<Message> messages = new Queue<Message>();

        public void Add(object[] content)
        {
            this.messages.Enqueue(new Message(content));
        }

        public Message[] FlushAllMessages()
        {
            var result = new Message[this.messages.Count];
            this.messages.CopyTo(result, 0);
            this.messages.Clear();
            return result;
        }

        public struct Message
        {
            private readonly object[] content;
            public Message(object[] content)
            {
                this.content = content;
            }

            public object[] Content
            {
                get
                {
                    if (this.content == null)
                    {
                        return Array.Empty<object>();
                    }
                    else
                    {

                        return this.content;
                    }
                }
            }
        }
    }
}
