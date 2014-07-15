using System;
using System.Linq;
using System.Collections.Generic;
using RabbitMQ.Client;
using System.Threading;

namespace QueueService {

    internal class ChannelManagerPool {

        private readonly Action eventError;

        private readonly List<ChannelManager> channelManagers;

        internal ChannelManagerPool(IEnumerable<ChannelEntity> channels, IConnection mqConnection, Action eventError) {
            this.channelManagers = new List<ChannelManager>();

            foreach (var channel in channels) {
                var channelManager = new ChannelManager(mqConnection.CreateModel(), channel);
                channelManager.ErrorEvent += new ThreadInterface.ErrorEventHandler(channelManager_ErrorEvent);
                this.channelManagers.Add(channelManager);
            }
            this.eventError = eventError;
        }

        private void channelManager_ErrorEvent(object sender, ThreadErrorEventArgs e) {
            this.eventError();
        }

        internal void Run() {
            foreach (ChannelManager channelManager in this.channelManagers) {
                channelManager.Start();
                new Thread(new ThreadStart(channelManager.Run)).Start();
            }
        }

        internal void RecreateChannelWithErrors(IConnection mqConnection) {
            foreach (var channelManager in this.channelManagers.Where((c) => c.HasRecreateChannel)) {
                RecreateChannel(mqConnection, channelManager);
            }
        }

        internal void RecreateAllChannels(IConnection mqConnection) {
            foreach (ChannelManager channelManager in this.channelManagers) {
                RecreateChannel(mqConnection, channelManager);
            }
        }

        internal void CloseAndWait() {
            foreach (ChannelManager channelManager in this.channelManagers) {
                LogService.WriteInfo("Closing channel: {0}", channelManager.ToString());
                channelManager.Stop();
                channelManager.WaitStopped();
                channelManager.Close();
                LogService.WriteInfo("Closed channel: {0}", channelManager.ToString());
            }
        }
  
        private static void RecreateChannel(IConnection mqConnection, ChannelManager channelManager) {

            LogService.WriteError("Recreating channel: {0} due to one error. Review traces for to see error", channelManager.ToString());

            try {
                channelManager.RecreateChannel(mqConnection.CreateModel());
            }
            catch (Exception ex) {
                new ServiceException(ex, "Error creating rabbit channel: {0}", channelManager.ToString());
            }

            channelManager.Start();
            new Thread(new ThreadStart(channelManager.Run)).Start();
        }

        public bool IsCrashConnection {
            get {
                return this.channelManagers.Count((c) => c.IsRetryOverflow) == this.channelManagers.Count;
            }
        }
    }
}
