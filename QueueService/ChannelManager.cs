using System;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System.Threading;
using RabbitMQ.Client.Events;

namespace QueueService {

    internal class ChannelManager : ThreadInterface {

        private IModel model;
        private readonly ChannelEntity entityChannel;
        private readonly String channelId;
        private readonly WorkerProcessPool workerProcessPool;

        internal ChannelManager(IModel model, ChannelEntity entityChannel) {
            this.entityChannel = entityChannel;
            LogService.WriteInfo("Starting subcription channel: {0}", entityChannel.ToString());
            this.channelId = entityChannel.ToString();
            this.ConfigureModel(model);
            this.workerProcessPool = new WorkerProcessPool(entityChannel.WorkerThreadMax, () => this.SetSignal(), entityChannel);
        }

        private void ConfigureModel(IModel model) {
            model.QueueDeclare(entityChannel.QueueInfo.QueueName, entityChannel.Durable, false, false, null);
            model.QueueBind(entityChannel.QueueInfo.QueueName, entityChannel.QueueInfo.ExchangeMq, entityChannel.QueueInfo.RoutingKey, null);
            this.model = model;
        }

        internal void RecreateChannel(IModel model) {
            this.Close();
            this.ConfigureModel(model);
            this.InitRetry();
        }
  
        internal void Close() {
            Util.TryExecute(() => {
                if (this.model != null)
                    this.model.Close();
            });
        }

        protected override void RunProcess() {

            this.workerProcessPool.Run();

            try {

                using (Subscription sub = new Subscription(this.model, this.entityChannel.QueueInfo.QueueName, false)) {

                    while (this.IsRunning == ThreadInterface.CONST_TRUE) {

                        BasicDeliverEventArgs result;

                        if (sub.Next(10000, out result)) {
                            this.workerProcessPool.SendMessage(result.Body);

                            sub.Ack();

                            this.WaitSignal(); // Espera a que se libere un slot
                        }
                    }
                }

                Util.TryExecute(() => this.workerProcessPool.CloseAndWait(true));
            }
            catch (Exception ex) {
                Util.TryExecute(() => this.workerProcessPool.CloseAndWait(false));
                this.IncrementRetry();
                new ServiceException(ex, "Error into channel: {0}. Number retry: {1}", this.channelId, this.Retry);
                this.HasError = ThreadInterface.CONST_TRUE;
                Util.TryExecute(() => this.OnErrorEventHandler(ex));
            }
        }

        private void IncrementRetry() {
            Interlocked.Increment(ref this.retry);
        }

        internal void InitRetry() {
            Interlocked.Exchange(ref this.retry, 0);
        }

        private long retry;
        private long Retry {
            get {
                return Interlocked.Read(ref this.retry);
            }
        }

        internal bool IsRetryOverflow {
            get {
                return this.Retry > entityChannel.FaultRetry;
            }
        }

        public bool HasRecreateChannel {
            get {
                return this.HasError == ThreadInterface.CONST_TRUE && !this.IsRetryOverflow;
            }
        }
    }
}
