using System;
using System.Linq;
using System.Configuration;
using System.Xml;
using System.Collections.Generic;
using System.Diagnostics;

namespace QueueService {

    public class SubcriptionConfigurationSectionHandler : IConfigurationSectionHandler {

        public object Create(object parent, object configContext, XmlNode section) {

            if (!EventLog.SourceExists(LogService.CONST_EVENTLOG_SOURCENAME)) {
                EventLog.CreateEventSource(LogService.CONST_EVENTLOG_SOURCENAME, LogService.CONST_EVENTLOG_LOGNAME);
            }

            var connections = new List<ConnectionEntity>();

            foreach (XmlNode childConnection in section.ChildNodes) {

                var connection = new ConnectionEntity();

                connection.Name = Util.GetValueOrDefault("Name", () => childConnection.Attributes["Name"].Value, string.Empty, true);
                connection.Uri = Util.GetValueOrDefault("Uri", () => childConnection.Attributes["Uri"].Value, string.Empty, true);

                foreach (XmlNode childChannel in childConnection.ChildNodes) {

                    var channel = new ChannelEntity();

                    channel.Name = Util.GetValueOrDefault("Name", () => childChannel.Attributes["Name"].Value, string.Empty, true);
                    channel.Durable = Util.GetValueOrDefault<bool>("Durable", () => bool.Parse(childChannel.Attributes["Durable"].Value), false, false);
                    channel.QueueInfo.Exchange = Util.GetValueOrDefault("Exchange", () => childChannel.Attributes["Exchange"].Value, QueueInfoEntity.CONST_EXCHANGE_DIRECT, false);
                    channel.FaultRetry = Util.GetValueOrDefault<int>("FaultRetry", () => int.Parse(childChannel.Attributes["FaultRetry"].Value), 5, false);
                    channel.QueueInfo.QueueName = Util.GetValueOrDefault("QueueName", () => childChannel.Attributes["QueueName"].Value, string.Empty, true);
                    channel.QueueInfo.RoutingKey = Util.GetValueOrDefault("RoutingKey", () => childChannel.Attributes["RoutingKey"].Value, string.Empty, false);
                    channel.WorkerThreadMax = Util.GetValueOrDefault<int>("WorkerThreadMax", () => int.Parse(childChannel.Attributes["WorkerThreadMax"].Value), 5, false);
                    channel.InSecondProcessMaxTimeWarning = Util.GetValueOrDefault<int>("InSecondProcessMaxTimeWarning", () => int.Parse(childChannel.Attributes["InSecondProcessMaxTimeWarning"].Value), 180, false);
                    channel.Process = Util.GetValueOrDefault("Process", () => childChannel.Attributes["Process"].Value, string.Empty, true);
                    channel.ConfiguractionSection = childChannel.ChildNodes;

                    connection.AddChannel(channel);
                }

                connections.Add(connection);
            }

            return connections;
        }
    }
}
