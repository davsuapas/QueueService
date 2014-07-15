using System;
using System.Linq;
using System.Collections.Generic;

namespace QueueService {

    public class ConnectionEntity {
        public string Name { get; set; }
        public string Uri { get; set; }

        public override string ToString() {
            return String.Format("Connection info. Name: {0},  HostName: {1}", this.Name, this.Uri);
        }

        private readonly List<ChannelEntity> channels;

        public IEnumerable<ChannelEntity> Channels {
            get {
                return channels;
            }
        }

        public ConnectionEntity() {
            this.channels = new List<ChannelEntity>();
        }

        public void AddChannel(ChannelEntity channel) {
            this.channels.Add(channel);
        }
    }
}
