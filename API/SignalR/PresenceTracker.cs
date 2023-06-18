namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUser = new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string username, string connectioId)
        {
            bool isOnline = false;
            lock(OnlineUser)
            {
                if(OnlineUser.ContainsKey(username))
                {
                    OnlineUser[username].Add(connectioId);
                }
                else
                {
                    OnlineUser.Add(username, new List<string>{connectioId});  
                    isOnline = true;  
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectioId)
        {
            bool isOffline = false;
            lock(OnlineUser)
            {
                if(!OnlineUser.ContainsKey(username)) return Task.FromResult(isOffline);

                OnlineUser[username].Remove(connectioId);

                if(OnlineUser[username].Count == 0)
                {
                    OnlineUser.Remove(username);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUser)
            {
                onlineUsers = OnlineUser.OrderBy(k => k.Key).Select(k => k.Key).ToArray();    
            }

            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionsForUser(string username)
        {   
            List<string> connectionIds;

            lock(OnlineUser)
            {
                connectionIds = OnlineUser.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
    }
}