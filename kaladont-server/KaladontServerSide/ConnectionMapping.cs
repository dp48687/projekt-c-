using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaladontServerSide
{
    /// <summary>
    /// Defines methods and storage for handling connections
    /// Used to add new connection, remove existing one, count them and get all connections
    /// </summary>
    /// <typeparam name="T">Parameter which will represent the type of key in dictionary of connections</typeparam>
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();

        /// <summary>
        /// Returns number of stored connections
        /// </summary>
        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        /// <summary>
        /// Adds a new connection
        /// </summary>
        /// <param name="key">Specifies the name of connection</param>
        /// <param name="connectionId">Specifies the connection id</param>
        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        /// <summary>
        /// Gets the collection of connections
        /// </summary>
        /// <param name="key">Parameter used to specify connection in a dictionary</param>
        /// <returns>Collection of connections</returns>
        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Removes the connection
        /// </summary>
        /// <param name="key">Specifies the connection that is going to be removed</param>
        /// <param name="connectionId"></param>
        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }
                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}
