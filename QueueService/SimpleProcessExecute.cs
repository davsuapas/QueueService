using System;
using System.Linq;
using System.Xml;

namespace QueueService {

    public abstract class SimpleProcessExecute : IProcessExecute {

        protected SimpleProcessExecute() {
        }

        public abstract bool Execute(byte[] message);

        public void SetConfiguration(XmlNodeList configNodeList) {
        }
    }
}
