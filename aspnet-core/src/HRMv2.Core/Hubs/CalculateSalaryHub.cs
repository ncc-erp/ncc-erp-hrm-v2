using Abp.AspNetCore.SignalR.Hubs;
using Abp.Dependency;
using Abp.RealTime;
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

        public void Register()
        {
            Logger.Info("A client Register" + Context.ConnectionId);
        }
        
        public void SendMessage(Object message)
        {
            try
            {
                if (Clients == null)
                {
                    Logger.Info("Clients null");
                    return;
                }
                //Clients.All.SendAsync("getMessage", message);
                Clients.All.SendAsync("getNotification", message);
                Logger.Info("sent getMessage to Clients.All");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
               
        }

        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Logger.Info($"A client connected to CalculateSalaryHub, ConnectionId: {Context.ConnectionId}, UserIdentifier: {Context.UserIdentifier}");
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            Logger.Info($"A client disconnected to CalculateSalaryHub, ConnectionId: {Context.ConnectionId}, UserIdentifier: {Context.UserIdentifier}");
        }
    }
}
