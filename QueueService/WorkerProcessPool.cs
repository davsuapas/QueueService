using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace QueueService {

    internal class WorkerProcessPool {

        private readonly List<WorkerProcessManager> workerProcessManagers;
        private readonly string channelId;
        private readonly Action activateChannel;

        internal WorkerProcessPool(int intThreadNumber, Action activateChannel, ChannelEntity channelEntity) {
            this.channelId = channelEntity.ToString();
            this.workerProcessManagers = new List<WorkerProcessManager>();
            this.activateChannel = activateChannel;

            for (int i = 0; i < intThreadNumber; i++) {
                var workerProcessManager = new WorkerProcessManager(activateChannel, channelEntity);
                workerProcessManagers.Add(workerProcessManager);
            }
        }

        internal void Run() {
            foreach (var workerProcessManager in this.workerProcessManagers) {
                workerProcessManager.Start();
                new Thread( new ThreadStart(workerProcessManager.Run)).Start();
		 	}
        }

        internal void SendMessage(byte[] message) {
            
            int countFreeSlot = 0;

            for (int i = 0; i < this.workerProcessManagers.Count; i++) {
                var workerProcessManager = workerProcessManagers[i];

                if (workerProcessManager.IsMessageNull) {

                    if (++countFreeSlot == 1) { // Solo asigno el mensaje al primero. 
                        LogService.WriteInfo("Send message to the free process with number: {0}, Channel: {1}", i, this.channelId);
                        workerProcessManager.Message = message;
                    }
                }
            }

            LogService.WriteInfo("Free slots: {0} for channel: {1}", countFreeSlot, this.channelId);

            if (countFreeSlot > 1) { // Si hay slot libre activo el canal para cojer más mensajes
                activateChannel();
            }
        }

        internal void CloseAndWait(bool dispose) {

            foreach (var workerProcessManager in this.workerProcessManagers) {

                LogService.WriteInfo("Stopping worker process: {0}", this.channelId);
                workerProcessManager.Stop();
                workerProcessManager.WaitStopped();

                if (dispose) {
                    LogService.WriteInfo("Disposing worker process: {0}", this.channelId);
                    workerProcessManager.Dispose();
                }

                LogService.WriteInfo("Stopped  worker process: {0}", this.channelId);
            }
        }
    }
}
