using Abp.AspNetCore.SignalR.Hubs;
using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Hubs
{
    public class CalculateSalaryHub: AbpHubBase, ISingletonDependency
    {

        public CalculateSalaryHub()
        {
        }

        public void SendMessage(Object message)
        {
            try
            {
                if (Clients == null)
                {
                    return;
                }
                Clients.All.SendAsync("getMessage", message);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
               
        }

        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Logger.Debug("A client connected to MyChatHub: " + Context.ConnectionId);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            Logger.Debug("A client disconnected from MyChatHub: " + Context.ConnectionId);
        }
    }
}
