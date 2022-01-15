using ServiceStack.Redis;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace NBP_1_2.Redis
{
    public class RedisLogic
    {
        private string globalCounterKey = "nekiKey";

        static private string onlineUsers = "onlineUsers";

       // readonly RedisClient redis = new RedisClient(Config.SingleHost);

       /* public RedisLogic()
        {
            //ako koristis global counter ovde napisi kod koji proverava da li postoji i ako ne postoji kreiraj ga
        }*/

        static public RedisClient GetClient(IServiceProvider provider)
        {
            var client = new RedisClient(Config.SingleHost);
            return client;
        }

        public static void UserLogin(RedisClient redis, string username)
        {
            redis.AddItemToSet(onlineUsers, username);
        }
        public static void UserLogout(RedisClient redis, string username)
        {
            redis.RemoveItemFromSet(onlineUsers, username);
        }
        public static long CurrentOnlineUsers(RedisClient redis)
        {
            long test = redis.GetSetCount(onlineUsers);
            return test;
        }
        public static void PlayerAddedToTeam(RedisClient redis, string playerID)
        {
            //redis.Increment("inTeam:" + playerID + ":player", 1);
            redis.Incr("inTeam:" + playerID + ":player");
        }
        public static void PlayerRemovedFromTeam(RedisClient redis, string playerID)
        {
            redis.Decrement("inTeam:" + playerID + ":player",1);
        }
        public static long GetNumPlayerHaveInTeam(RedisClient redis, string playerID)
        {
            long test = redis.Get<long>("inTeam:" + playerID + ":player");
            return test;
        }
        public static void AddUserToLeaderboard(RedisClient redis, string username)
        {
            byte[] usernameBytes = Encoding.ASCII.GetBytes("username");
            byte[] usernameValue = Encoding.ASCII.GetBytes(username);


            redis.HSet(username, usernameBytes, usernameValue);
            redis.ZAdd("globalLeaderboard", 0, usernameValue);
        }
        public static void AddScoreToUser(RedisClient redis, string username, float score)
        {
            byte[] usernameValue = Encoding.ASCII.GetBytes(username);

            redis.ZIncrBy("globalLeaderboard", score, usernameValue);
        }
        public static float GetScoreFromUser(RedisClient redis, string username)
        {
            byte[] usernameValue = Encoding.ASCII.GetBytes(username);

            return (float)redis.ZScore("globalLeaderboard", usernameValue);
        }
        public static void RemoveScoreFromUser(RedisClient redis, string username, float score)
        {
            byte[] usernameValue = Encoding.ASCII.GetBytes(username);

            redis.ZIncrBy("globalLeaderboard", -score, usernameValue);
        }
        public static List<String> GetLeaderboard(RedisClient redis)
        {
            byte[][] leaderboard = redis.ZRevRangeWithScores("globalLeaderboard", 0, -1);
            List<String> users = new List<string>();
            foreach (byte[] entry in leaderboard)
            {
                string user = Encoding.ASCII.GetString(entry);
                users.Add(user);
            }
            return users;
        }
        public static List<string> GetLeaderboardForUsers(RedisClient redis, string[] usersStr)
        {
            byte[][] leaderboard = redis.ZRevRangeWithScores("globalLeaderboard", 0, -1);
            List<string> users = new List<string>();
            for (int i = 0; i < leaderboard.Length; i = i + 2)
            {
                string user = Encoding.ASCII.GetString(leaderboard[i]);
                if (usersStr.Contains(user))
                {
                    users.Add(user);
                    users.Add(Encoding.ASCII.GetString(leaderboard[i + 1]));
                }
            }
            return users;
        }

        public void CreateSubscriber(RedisClient redis, string teamID)
        {
            using (var subscription = redis.CreateSubscription())
            {
                subscription.OnMessage = (channel, msg) =>
                {
                    
                };
                subscription.SubscribeToChannels(teamID);
            }
        }

        public void SubscribeToTeam(RedisClient redis, string teamID, IRedisSubscription sub)
        {
            sub.SubscribeToChannels(teamID);
        }
        public void UnsubscribeFromTeam(RedisClient redis, string teamID, IRedisSubscription sub)
        {
            sub.UnSubscribeFromChannels(teamID);
        }
        public void PublishMessage(RedisClient redis, string teamID, string message)
        {
            redis.PublishMessage(teamID, message);
        }
    }
}
