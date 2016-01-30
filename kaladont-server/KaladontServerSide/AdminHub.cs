using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// It handles connection jobs at server
/// </summary>
namespace KaladontServerSide
{
    [Authorize(Roles = "Admin")]
    public class AdminHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        public List<Player> _players = new List<Player>();
        public List<PlayerGroup> _playerGroups = new List<PlayerGroup>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void SendMessageToAll(string name, string message)
        {
            Clients.All.showMessageToAll(message);
        }

        /// <summary>
        /// Sends the message to the specified user
        /// </summary>
        /// <param name="id">Users id</param>
        /// <param name="message">Message that will be sent</param>
        public void sendToUser(string id, string message)
        {
            string name = Context.User.Identity.Name;
            foreach (var connectionId in _connections.GetConnections(id))
            {
                Clients.Client(connectionId).addChatMessage(name + ": " + message);
            }
        }

        /// <summary>
        /// Handles the job when user connects
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            string id = Context.ConnectionId;
            _connections.Add(name,id);
            _players.Add(new Player(name,id));
            return base.OnConnected();
        }

        /// <summary>
        /// Handles the job when user disconnects
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;
            string id = Context.ConnectionId;
            _connections.Remove(name,id);
            _players.Remove(new Player(name,id));
            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Handles the job when user reconnects
        /// </summary>
        /// <returns></returns>
        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;
            string id = Context.ConnectionId;
            if (!_connections.GetConnections(name).Contains(id))
            {
                _connections.Add(name,id);
            }
            _players.Add(new Player(name, id));
            return base.OnReconnected();
        }

        /// <summary>
        /// Simply joins the player to the group
        /// <param name="name">Represents the groups name to which the player is going to be joined</param>
        /// </summary>
        public void joinGame(string name)
        {
            String id = Context.ConnectionId;
            Player p = new Player("","");
            _players.ForEach(player=> {
                if(player.id.Equals(id))
                {
                    p = player;
                }
            });
            _playerGroups.ForEach(group=>
            {
                if(group._name.Equals(name))
                {
                    group.addPlayer(p);
                }
            });
        }

        /// <summary>
        /// This method is called by user who wants to host a new game
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="isCro"></param>
        public void hostGame(string name, string id, bool isCro)
        {
            _playerGroups.Add(new PlayerGroup(_playerGroups.Count()+"",isCro));
        }

        /// <summary>
        /// This method is invoked by the group maker to start the game
        /// <param name="name">Specifies the group name</param>
        /// </summary>
        public void startGame(string name)
        {
            _playerGroups.ForEach(group=> {
                if(group._name.Equals(name))
                {
                    new Logic(group);
                }
            });
        }

    }
}
