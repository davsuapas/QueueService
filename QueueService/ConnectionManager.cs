using System;
using System.Linq;
using RabbitMQ.Client;

namespace QueueService {

    internal class ConnectionManager {

        private IConnection mqConnection;
        private readonly ConnectionEntity entityConnection;
        private readonly ChannelManagerPool channelManagerPool;

        internal ConnectionManager(ConnectionEntity entityConnection, Action eventError) {
            this.entityConnection = entityConnection;
            LogService.WriteInfo("Starting subcription connection: {0}", entityConnection.ToString());
            this.CreateConnectionMq();
            this.channelManagerPool = new ChannelManagerPool(entityConnection.Channels, this.mqConnection, eventError);
        }
  
        private void CreateConnectionMq() {
            var connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = this.entityConnection.Uri;

            try {
                this.mqConnection = connectionFactory.CreateConnection();
            }
            catch (Exception ex) {
                throw new ServiceException(ex, "Error creating rabbit connection for connection: {0}", this.entityConnection.ToString());
            }
        }

        internal void Run() {
            this.channelManagerPool.Run();
        }

        internal void ObserverChannelErrors() {
            this.channelManagerPool.RecreateChannelWithErrors(this.mqConnection);
        }

        internal void ObserverConnectionErrors() {
            if (this.channelManagerPool.IsCrashConnection) {
                LogService.WriteError("Crash connection. Retrying for connection: {0}", this.entityConnection.ToString());
                this.CloseConnection();
                Util.TryExecute(() => {
                    this.CreateConnectionMq();
                    this.channelManagerPool.RecreateAllChannels(this.mqConnection);
                });
            }
        }

        private void CloseConnection() {
            Util.TryExecute(() => {
                if (this.mqConnection != null)
                    this.mqConnection.Close();
            });
        }

        internal void Close() {
            Util.TryExecute(() => {
                LogService.WriteInfo("Closing connection: {0}", entityConnection.ToString());
                this.channelManagerPool.CloseAndWait();
                this.CloseConnection();
                LogService.WriteInfo("Closed connection: {0}", entityConnection.ToString());
            });
        }
    }
}
