using System;
using System.Linq;

namespace QueueService {

    internal class ThreadErrorEventArgs : EventArgs {

        private readonly Exception ex;

        internal ThreadErrorEventArgs(Exception ex) {
            this.ex = ex;
        }

        internal Exception Exception {
            get {
                return this.ex;
            }
        }
    }
}
