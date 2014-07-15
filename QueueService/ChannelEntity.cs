using System;
using System.Linq;
using System.Xml;

namespace QueueService {

    public class ChannelEntity {

        public ChannelEntity() {
            this.queueInfo = new QueueInfoEntity(); 
            this.FaultRetry = 1;
            this.WorkerThreadMax = 2;
        }

        private readonly QueueInfoEntity queueInfo;
        public QueueInfoEntity QueueInfo {
            get {
                return this.queueInfo;
            }
        }

        public string Name { get; set; }

        public bool Durable { get; set; }
        public string Process{ get; set; }
        public int FaultRetry { get; set; }
        public int WorkerThreadMax { get; set; }
        public int InSecondProcessMaxTimeWarning { get; set; }
        public XmlNodeList ConfiguractionSection{ get; set; }

        public override string ToString() {
            return String.Format("ChannelEntity -> Name: {0}, QueueName: {1}, Process: {2}, WorkerThreadMax: {3}, InSecondProcessMaxTimeWarning: {4}", this.Name, this.QueueInfo.QueueName, this.Process, this.WorkerThreadMax, this.InSecondProcessMaxTimeWarning);
        }
    }
}
