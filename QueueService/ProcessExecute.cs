using System;
using System.Linq;
using System.Xml;

namespace QueueService {

    public abstract class ProcessExecute<T> : IProcessExecute where T : class {

        protected ProcessExecute() {
        }

        protected T ConfigurationSection { get; set; }

        protected abstract T GetConfiguration(XmlNodeList configNodeList);
        public abstract bool Execute(byte[] message);

        public void SetConfiguration(XmlNodeList configNodeList) {
            this.ConfigurationSection = this.GetConfiguration(configNodeList);
        }
    }
}
