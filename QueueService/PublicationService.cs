using System;
using System.Linq;
using RabbitMQ.Client;

namespace QueueService {

    public class PublicationService : IDisposable {

        private readonly QueueInfoEntity queueInfoEntity;

        private readonly IConnection connection;
        private readonly IModel model;

        public PublicationService(QueueInfoEntity queueInfoEntity) : this(queueInfoEntity, null) {
        }

        public PublicationService(QueueInfoEntity queueInfoEntity, QueueProvider queueProvider) {
            this.queueInfoEntity = queueInfoEntity;

            var connectionFactory = new ConnectionFactory();

            if (queueProvider == null) {
                connectionFactory.Uri = EnvironmentProvider.GetProvider<QueueProvider>().ConnectionString;
            }
            else {
                connectionFactory.Uri = queueProvider.ConnectionString;
            }

            this.connection = connectionFactory.CreateConnection();
            this.model = connection.CreateModel();
        }

        public void Publish<T>(T message) where T : class {

            try {
                model.QueueBind(this.queueInfoEntity.QueueName, 
                    this.queueInfoEntity.ExchangeMq, this.queueInfoEntity.RoutingKey, null);

                //TODO: Cuando implementemos las notificaciones habrá que poner la confirmación model.ConfirmSelect();

                IBasicProperties basicProperties = model.CreateBasicProperties();
                model.BasicPublish(this.queueInfoEntity.ExchangeMq,
                        this.queueInfoEntity.RoutingKey,
                        basicProperties, Serializer.Serialize<T>(message));

                //TODO: Cuando implementemos las notificaciones habrá que poner la confirmación  model.WaitForConfirms()
            }
            catch (Exception ex) {
                throw new ServiceException(ex, "Error publishing message for queue: {0}", this.queueInfoEntity.ToString());
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (this.model != null)
                    this.model.Close();

                if (this.connection != null)
                    this.connection.Close();
            }
        }
    }
}
