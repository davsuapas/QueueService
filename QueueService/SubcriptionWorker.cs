using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace QueueService {

    public class SubcriptionWorker {

        private readonly ConnectionManagerPool connectionManagerPool;

        public SubcriptionWorker(IEnumerable<ConnectionEntity> connectionsEntities) {
            this.connectionManagerPool = new ConnectionManagerPool(connectionsEntities);
        }

        public void Run() {
            LogService.WriteInfo("Running SubcriptionWorker");
            connectionManagerPool.Start();
            new Thread(new ThreadStart(this.connectionManagerPool.Run)).Start(); 
        }

        public void Stop() {
            LogService.WriteInfo("Stopping SubcriptionWorker");
            this.connectionManagerPool.Close();
            LogService.WriteInfo("Stopped SubcriptionWorker");
        }
    }
}
