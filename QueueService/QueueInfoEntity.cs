using System;
using System.Linq;

namespace QueueService {

    public class QueueInfoEntity {

        public QueueInfoEntity() {
            this.Exchange = CONST_EXCHANGE_DIRECT;
        }

        public static QueueInfoEntity CreateWithExchangeDirect(string queueName) {
            return new QueueInfoEntity() { Exchange = QueueInfoEntity.CONST_EXCHANGE_DIRECT, QueueName = queueName};
        }

        public const string CONST_EXCHANGE_DIRECT = "direct";
        public const string CONST_EXCHANGE_TOPIC = "topic";

        public string QueueName { get; set; }
        public string Exchange { get; set; }

        public string ExchangeMq {
            get {
                switch (this.Exchange) {
                    case CONST_EXCHANGE_DIRECT:
                        return "amq.direct";
                    case CONST_EXCHANGE_TOPIC:
                        return "amq.topic";
                    default:
                        throw new ArgumentException("El canal no es valido para el sistema de subcripciones");
                }
            }
        }

        private string routingKey;
        public string RoutingKey {
            get {
                if (Exchange == CONST_EXCHANGE_DIRECT)
                    return this.QueueName;
                else
                    return this.routingKey;
            }
            set {
                this.routingKey = value;
            }
        }

        public override string ToString() {
            return String.Format(" Publish queue info: QueName: {0} ; ExchangeName: {1} ; RouteKey: {2} ", this.QueueName, this.Exchange, this.routingKey);
        }
    }
}
