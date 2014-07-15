using System;
using System.Linq;
using System.Xml;

namespace QueueService {

    public interface IProcessExecute {
        bool Execute(byte[] message);
        void SetConfiguration(XmlNodeList configNodeList);
    }
}
