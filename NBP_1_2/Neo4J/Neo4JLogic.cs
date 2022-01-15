using Neo4jClient;
using Neo4jClient.Cypher;
using NBP_1_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Redis;
using NBP_1_2.Redis;

namespace NBP_1_2.Neo4J
{
    public class Neo4JLogic
    {
        static public IGraphClient GetClient(IServiceProvider provider)
        {
                var client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "sifra123");
                client.ConnectAsync().Wait();
                return client;
        }
        #region FootballPlayer
        public static List<FootballPlayer> GetFootballPlayers(IGraphClient client)
        {

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:FootballPlayer) return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballPlayer> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            foreach (FootballPlayer p in players)
            {
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:FootballPlayer)-[:playesIn]->(m:FootballTeam) WHERE n.id='" + p.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam team = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
                p.playesIn = team;
            }
            return players;
        }
        public static void AddFootballPlayer(IGraphClient client,string name, string birthday, string pozition, int value, int number)
        {
            FootballPlayer p = new FootballPlayer();
            p.name = name;
            p.birthday = birthday;
            p.pozition = pozition;
            p.value = value;
            p.number = number;

            string maxId = getFPMaxID(client);
            try
            {
                int mId = Int32.Parse(maxId);
                mId++;
                p.id = (mId).ToString();
            }
            catch (Exception exception)
            {
                p.id = "";
            }

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("name", name);
            queryDict.Add("birthday", birthday);
            queryDict.Add("pozition", pozition);
            queryDict.Add("value", value);
            queryDict.Add("number", number);

            var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:FootballPlayer {id:'" + p.id + "', name:'" + p.name
                                                            + "', birthday:'" + p.birthday + "', pozition:'" + p.pozition
                                                            + "', value:'" + p.value + "', number:'" + p.number
                                                            + "'}) return n",
                                                            queryDict, CypherResultMode.Set);

            List<FootballPlayer> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();

        }

        public static void DeleteFootballPlayer(IGraphClient client,string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:FootballPlayer) and exists(n.id) and n.id ='" + id + "' detach delete n",
                                                            queryDict, CypherResultMode.Projection);
            ((IRawGraphClient)client).ExecuteCypher(query);
        }

        public static bool ModifyFootballPlayer(IGraphClient client,string id, string name, string birthday, string pozition, int value, int number)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:FootballPlayer{id:'" + id + "'}) return n", queryDict, CypherResultMode.Set);
            List<FootballPlayer> player = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();

            if (player.Count() == 0)
            {
                return false;
            }
            FootballPlayer p = player.FirstOrDefault();
            query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:FootballPlayer {id:'" + p.id + "'}) SET n.name='" + name
                                                            + "', n.birthday='" + birthday + "', n.pozition='" + pozition
                                                            + "', n.value=" + value + ", n.number=" + number
                                                            + " return n",
                                                            queryDict, CypherResultMode.Set);
            player = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            return true;
        }

        public static FootballPlayer GetPlayer(IGraphClient client,string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:FootballPlayer) and n.id = '" + id + "' return n",
                                                queryDict, CypherResultMode.Set);
            FootballPlayer player = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList().FirstOrDefault();
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:FootballPlayer)-[:playesIn]->(m:FootballTeam) WHERE n.id='" + player.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam team = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
            player.playesIn = team;
            return player;
        }
        
        public static FootballPlayer GetPlayerName(IGraphClient client,string name)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:FootballPlayer) and n.name= '" + name + " return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballPlayer> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            return players.FirstOrDefault();
        }
        public static List<FootballPlayer> GetPlayers(IGraphClient client,string name)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:FootballPlayer) and n.name=~ '.*" + name + ".*' return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballPlayer> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            
            return players;
        }
        public static List<FootballPlayer> GetPlayersTeam(IGraphClient client, string name)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:FootballPlayer)-[:playesIn]->(m:FootballTeam) WHERE m.name=~ '.*" + name + ".*' return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballPlayer> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();

            return players;
        }
        public static List<FootballPlayer> GetPlayersTeamAndName(IGraphClient client, string name,string pname)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:FootballPlayer)-[:playesIn]->(m:FootballTeam) WHERE m.name=~ '.*" + name + ".*' and n.name=~ '.*" + pname + ".*' return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballPlayer> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();

            return players;
        }
        #endregion
        #region Player

        public static bool ModifyPlayer(IGraphClient client,string username, string password)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:Player{username:'" + username + "'}) return n", queryDict, CypherResultMode.Set);
            List<Player> player = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();

            if (player.Count() == 0)
            {
                return false;
            }
            Player p = player.FirstOrDefault();
            query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Player {username:'" + username + "'}) SET n.password='" + password + "' return n", queryDict, CypherResultMode.Set);
            player = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
            return true;
        }

        public static Player AddPlayer(IGraphClient client,string username, string password,string userType)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("username", username);
            queryDict.Add("password", password);
            queryDict.Add("money", 150);

            var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Player {username:'" + username + "', password:'" + password + "',money:" + 150 + ", userType:'"+userType+"'}) return n",
                                                queryDict, CypherResultMode.Set);
            List<Player> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
            return players.FirstOrDefault();
        }
        public static Player Login(IGraphClient client, string username, string password)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("username", username);
            queryDict.Add("password", password);
            queryDict.Add("money", 150);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Player) WHERE n.username='" + username + "' AND n.password='" + password + "' return n",
                                                queryDict, CypherResultMode.Set);
            List<Player> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
            return players.FirstOrDefault();
        }
        public static Player Register(IGraphClient client, string username, string password)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("username", username);
            queryDict.Add("password", password);
            queryDict.Add("money", 150);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Player) WHERE n.username='" + username + "' AND n.password='" + password + "' return n",
                                                queryDict, CypherResultMode.Set);
            List<Player> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
            if (players.Count == 0)
            {
                return AddPlayer(client, username, password,"Standard");
            }
            return null;
        }
        public static bool DeletePlayer(IGraphClient client,string username)
        {

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Player) and exists(n.username) and n.username ='" + username + "' detach delete n",
                                                            queryDict, CypherResultMode.Projection);
            ((IRawGraphClient)client).ExecuteCypher(query);
            return true;
        }
        public static IEnumerable<Player> ReturnPlayers(IGraphClient client)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Player) return n",
                                                            queryDict, CypherResultMode.Set);
            IEnumerable<Player> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
            return players;
        }
        public static Player ReturnPlayer(IGraphClient client,string username)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Player) and exists(n.username) and n.username ='" + username + "' return n",
                                                            queryDict, CypherResultMode.Set);
            Player players = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList().FirstOrDefault();
            return players;
        }
        
        public static void AddFriend(IGraphClient client,string username1, string username2)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:Player {username:'" + username1 + "'})" +
                "MATCH (p2:Player {username:'" + username2 + "'})" +
                "CREATE (p1)-[:isFriends]->(p2) return p1", queryDict,
                                                            CypherResultMode.Set);

            List<Player> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
        }
        public static List<FootballPlayer> GetPlayersInTeam(IGraphClient client, string username)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:Player)-[:hasInTeam]->(fp:FootballPlayer) where p1.username='"+username+"' return fp", queryDict,
                                                            CypherResultMode.Set);

            List<FootballPlayer> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            return actors;
        }
        public static void RemoveFriend(IGraphClient client,string username1, string username2)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:Player{username:'" + username1 + "'})-[r:isFriends]->(m:Player{username:'" + username2 + "'}) delete r",
                                                            queryDict, CypherResultMode.Projection);
            ((IRawGraphClient)client).ExecuteCypher(query);
        }

        public static void AddPlayerToTeam(IGraphClient client,string username, string playerID,RedisClient redis)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            var findPlayerquery = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:Player {username:'" + username + "'}) return p1", queryDict, CypherResultMode.Set);
            Player p = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(findPlayerquery).ToList().FirstOrDefault();

            var findFootballPlayerquery = new Neo4jClient.Cypher.CypherQuery("MATCH (p2:FootballPlayer {id:'" + playerID + "'}) return p2", queryDict, CypherResultMode.Set);
            FootballPlayer fp = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(findFootballPlayerquery).ToList().FirstOrDefault();

            if (p.money >= fp.value)
            {
                p.money = p.money - fp.value;
                var query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:Player {username:'" + username + "'}) SET p1.money=" + p.money + " return p1 "
                    , queryDict, CypherResultMode.Set);
                List<Player> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();

                query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:Player {username:'" + username + "'})" +
                "MATCH (p2:FootballPlayer {id:'" + playerID + "'})" +
                "CREATE (p1)-[:hasInTeam]->(p2) return p1", queryDict,
                                                            CypherResultMode.Set);

                actors = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
                RedisLogic.PlayerAddedToTeam(redis, playerID);
            }
        }
        public static List<Player> GetFriends(IGraphClient client, string username)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Player)-[:isFriends]->(p:Player) WHERE n.username='"+username+"' RETURN p ",
                                                            queryDict, CypherResultMode.Set);
            List<Player> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
            return players;
        }

        public static void RemovePlayerFromTeam(IGraphClient client,string username, string playerID, RedisClient redis)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            var findPlayerquery = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:Player {username:'" + username + "'}) return p1", queryDict, CypherResultMode.Set);
            Player p = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(findPlayerquery).ToList().FirstOrDefault();

            var findFootballPlayerquery = new Neo4jClient.Cypher.CypherQuery("MATCH (p2:FootballPlayer {id:'" + playerID + "'}) return p2", queryDict, CypherResultMode.Set);
            FootballPlayer fp = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(findFootballPlayerquery).ToList().FirstOrDefault();

            p.money = p.money + fp.value;
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:Player {username:'" + username + "'}) SET p1.money=" + p.money + " return p1",
                queryDict, CypherResultMode.Set);
            List<Player> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();

            query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:Player {username:'" + username + "'})-[r:hasInTeam]->(p2:FootballPlayer {id:'" + playerID + "'}) delete r",
             queryDict, CypherResultMode.Set);

            RedisLogic.PlayerRemovedFromTeam(redis, playerID);

            ((IRawGraphClient)client).ExecuteCypher(query);
        }

        public static void FollowTeam(IGraphClient client,string username, string teamID)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:Player {username:'" + username + "'})" +
                "MATCH (p2:FootballTeam {id:'" + teamID + "'})" +
                "CREATE (p1)-[:follows]->(p2) return p1", queryDict,
                                                            CypherResultMode.Set);

            List<Player> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
        }
        public static void UnfollowTeam(IGraphClient client,string username, string teamID)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:Player {username:'" + username + "'})-[r:follows]->(p2:FootballTeam {id:'" + teamID + "'}) delete r", queryDict,
                                                            CypherResultMode.Set);

            ((IRawGraphClient)client).ExecuteCypher(query);
        }


        public static List<Player> RecommendFriends(IGraphClient client,string username)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:Player{username:'" + username + "'})-[:isFriends]->(friends)-[:isFriends]->(friendsOfFriends)" +
                " WHERE NOT (n)-[:isFriends]->(friendsOfFriends) AND n <> friendsOfFriends RETURN friendsOfFriends  ",
                                                            queryDict, CypherResultMode.Set);
            List<Player> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
            return players;
        }
        public static List<FootballPlayer> RecommendPlayersFromFriends(IGraphClient client,string username)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:Player{username:'" + username + "'})-[:isFriends]->(friends)-[:hasInTeam]->(Players)" +
                " WHERE NOT (n)-[:hasInTeam]->(Players) RETURN Players ",
                                                            queryDict, CypherResultMode.Set);
            List<FootballPlayer> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            return players;
        }
        public static List<FootballPlayer> RecommendPlayerFromFollowedTeams(IGraphClient client,string username)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:Player{username:'" + username + "'})-[:follows]->(friends)<-[:playesIn]-(Players)" +
                " WHERE NOT (n)-[:hasInTeam]->(Players) RETURN Players ",
                                                            queryDict, CypherResultMode.Set);
            List<FootballPlayer> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            return players;
        }
        public static List<Player> GetPlayersWhoHaveFPInTeam(IGraphClient client,string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:Player)-[:hasInTeam]->(fp:FootballPlayer{id:'" + id + "'})" +
                " RETURN n ",
                                                            queryDict, CypherResultMode.Projection);
            List<Player> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
            return players;
        }
        #endregion
        #region FootballTeam
        public static void AddFootballTeam(IGraphClient client,string name, string homeTown, string stadiumName)
        {
            FootballTeam t = new FootballTeam();
            t.name = name;
            t.homeTown = homeTown;
            t.stadiumName = stadiumName;
            string maxID = getFTMaxID(client);
            try
            {
                int mId = Int32.Parse(maxID);
                mId++;
                t.id = (mId).ToString();
            }
            catch (Exception exception)
            {
                t.id = "";
            }

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("name", name);
            queryDict.Add("homeTown", homeTown);
            queryDict.Add("stadiumName", stadiumName);

            var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:FootballTeam {id:'" + t.id + "', name:'" + t.name
                                                            + "', homeTown:'" + t.homeTown + "', stadiumName:'" + t.stadiumName
                                                            + "'}) return n",
                                                            queryDict, CypherResultMode.Set);

            List<FootballTeam> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList();
        }

        public static bool ModifyFootballTeam(IGraphClient client,string id, string name, string homeTown, string stadiumName)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:FootballTeam{id:'" + id + "'}) return n", queryDict, CypherResultMode.Set);
            List<FootballTeam> player = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList();

            if (player.Count() == 0)
            {
                return false;
            }
            FootballTeam t = player.FirstOrDefault();
            query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:FootballTeam {id:'" + t.id + "'}) SET n.name='" + name
                                                            + "', n.homeTown='" + homeTown + "', n.stadiumName='" + stadiumName
                                                            + "' return n",
                                                            queryDict, CypherResultMode.Set);
            player = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList();
            return true;
        }

        public static void DeleteFootballTeam(IGraphClient client, string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:FootballTeam) and exists(n.id) and n.id ='" + id + "' detach delete n",
                                                            queryDict, CypherResultMode.Projection);
            ((IRawGraphClient)client).ExecuteCypher(query);
        }
        public static FootballTeam GetTeam(IGraphClient client,string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:FootballTeam) and n.id = '" + id + "' return n",
                                                queryDict, CypherResultMode.Set);
            FootballTeam team = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
            query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:FootballPlayer)-[:playesIn]->(m:FootballTeam) WHERE m.id = '" + id + "' return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballPlayer> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            team.players = players;
            return team;
        }
        public static FootballTeam GetTeamName(IGraphClient client,string name)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:FootballTeam) and n.name= '" + name + " return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballTeam> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList();
            return players.FirstOrDefault();
        }
        public static List<FootballTeam> GetTeams(IGraphClient client,string name)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:FootballTeam) and n.name=~ '.*" + name + ".*' return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballTeam> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList();
            return players;
        }
        public static List<FootballTeam> GetFootballTeams(IGraphClient client)
        {

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:FootballTeam) return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballTeam> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList();
            return players;
        }

        public static bool AddFootballPlayerToTeam(IGraphClient client,string fpID, string teamID)
        {


            Dictionary<string, object> queryDict = new Dictionary<string, object>();


            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:FootballPlayer {id:'" + fpID + "'})-[r:playesIn]->(p2:FootballTeam) return p2", queryDict,
                                                            CypherResultMode.Set);
            FootballTeam team = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
            if (team != null)
            {
                if (team.id == teamID)
                    return false;
                RemoveFootballPlayerFromTeam(client, fpID, team.id);
            }
            



            query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:FootballPlayer {id:'" + fpID + "'})" +
                "MATCH (p2:FootballTeam {id:'" + teamID + "'})" +
                "CREATE (p1)-[:playesIn]->(p2) return p1", queryDict,
                                                            CypherResultMode.Set);

            List<FootballPlayer> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            if (actors.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void RemoveFootballPlayerFromTeam(IGraphClient client,string fpID, string teamID)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:FootballPlayer {id:'" + fpID + "'})-[r:playesIn]->(p2:FootballTeam {id:'" + teamID + "'}) delete r", queryDict,
                                                            CypherResultMode.Set);

            ((IRawGraphClient)client).ExecuteCypher(query);
        }
        public static void RemoveFootballPlayerFromAnyTeam(IGraphClient client, string fpID)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (p1:FootballPlayer {id:'" + fpID + "'})-[r:playesIn]->(p2:FootballTeam) delete r", queryDict,
                                                            CypherResultMode.Set);

            ((IRawGraphClient)client).ExecuteCypher(query);
        }

        #endregion


        #region Round
        public static void AddRound(IGraphClient client,string HomeTeam, string awayTeam, string matchDate, int attendance,int roundNum)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            Round r = new Round();
            r.matchDate = matchDate;
            r.attendance = attendance;

            queryDict.Add("matchDate", matchDate);
            queryDict.Add("attendance", attendance);

            string maxID = Neo4JLogic.getFTMaxID(client);
            try
            {
                int mId = Int32.Parse(maxID);
                mId++;
                r.id = (mId).ToString();
            }
            catch (Exception exception)
            {
                r.id = "";
            }

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (ht:FootballTeam {id:'" + HomeTeam + "'})" +
                "MATCH (at:FootballTeam {id:'" + awayTeam + "'})" +
                "CREATE (n:Round { id:'" + r.id + "', matchDate:'" + matchDate + "',attendance:" + attendance + ", roundNum:"+roundNum+"})" +
                "CREATE (n)-[:homeTeam]->(ht) CREATE (n)-[:awayTeam]->(at) return n",
                                                            queryDict, CypherResultMode.Set);

            List<Round> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<Round>(query).ToList();

        }
        public static bool ModifyRound(IGraphClient client,string id, string HomeTeam, string awayTeam, string matchDate, int attendance, int roundNum)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:Round{id:'" + id + "'}) return n", queryDict, CypherResultMode.Set);
            List<Round> player = ((IRawGraphClient)client).ExecuteGetCypherResults<Round>(query).ToList();

            if (player.Count() == 0)
            {
                return false;
            }
            query = new Neo4jClient.Cypher.CypherQuery("match (n:Round)-[r]-() where n.id='" + id + "' delete r", queryDict, CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(query);
            query = new Neo4jClient.Cypher.CypherQuery("MATCH (ht:FootballTeam {id:'" + HomeTeam + "'})" +
                "MATCH (at:FootballTeam {id:'" + awayTeam + "'})" +
                "MATCH (n:Round { id:'" + id + "'}) SET n.matchDate='" + matchDate + "',n.attendance=" + attendance + ", n.roundNum="+ roundNum + " " +
                "CREATE (n)-[:homeTeam]->(ht) CREATE (n)-[:awayTeam]->(at) return n",
                                                            queryDict, CypherResultMode.Set);

            List<Round> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<Round>(query).ToList();
            return true;
        }
        public static List<PlayedIn> GetPlayersPlayedInRound(IGraphClient client, string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:FootballPlayer)-[r:playedIn]->(m:Round) where m.id = '" + id + "' return r",
                                                queryDict, CypherResultMode.Set);
            List<PlayedIn> players = ((IRawGraphClient)client).ExecuteGetCypherResults<PlayedIn>(query).ToList();
            query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:FootballPlayer)-[r:playedIn]->(m:Round) where m.id = '" + id + "' return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballPlayer> fplayers = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            for(int i = 0; i < fplayers.Count(); i++)
            {
                players[i].player = fplayers[i];
            }
            return players;
        }


        public static void DeleteRound(IGraphClient client,string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Round) and exists(n.id) and n.id ='" + id + "' detach delete  n",
                                                            queryDict, CypherResultMode.Projection);
            ((IRawGraphClient)client).ExecuteCypher(query);
        }

        public static void DeleteScoreForRound(IGraphClient client, RedisClient redisClient,string id)
        {
            //Ponestalo vremena
        }
        public static Round GetRound(IGraphClient client,string id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Round) and n.id = '" + id + "' return n",
                                                queryDict, CypherResultMode.Set);
            Round players = ((IRawGraphClient)client).ExecuteGetCypherResults<Round>(query).ToList().FirstOrDefault();
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:Round)-[:homeTeam]->(m:FootballTeam) where n.id='" + players.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam home = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
            players.homeTeam = home;
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:Round)-[:awayTeam]->(m:FootballTeam) where n.id='" + players.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam away = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
            players.awayTeam = away;
            return players;
        }
        public static List<Round> GetRoundNum(IGraphClient client, int num)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Round) and n.roundNum = " + num + " return n",
                                                queryDict, CypherResultMode.Set);
            List<Round> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Round>(query).ToList();
            foreach (Round r in players)
            {
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:Round)-[:homeTeam]->(m:FootballTeam) where n.id='" + r.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam home = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
                r.homeTeam = home;
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:Round)-[:awayTeam]->(m:FootballTeam) where n.id='" + r.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam away = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
                r.awayTeam = away;
            }
            return players;
        }
        public static List<Round> GetRoundDate(IGraphClient client,string date)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Round) WHERE date(datetime(n.matchDate))=date('" + date + "') return n",
                                                queryDict, CypherResultMode.Set);
            List<Round> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Round>(query).ToList();
            foreach (Round r in players)
            {
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:Round)-[:homeTeam]->(m:FootballTeam) where n.id='" + r.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam home = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
                r.homeTeam = home;
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:Round)-[:awayTeam]->(m:FootballTeam) where n.id='" + r.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam away = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
                r.awayTeam = away;
            }
            return players;
        }
        public static List<Round> GetRoundDateNum(IGraphClient client, string date, int roundNum)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Round) WHERE date(datetime(n.matchDate))=date('" + date + "') and n.roundNum = " + roundNum + " return n",
                                                queryDict, CypherResultMode.Set);
            List<Round> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Round>(query).ToList();
            foreach (Round r in players)
            {
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:Round)-[:homeTeam]->(m:FootballTeam) where n.id='" + r.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam home = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
                r.homeTeam = home;
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:Round)-[:awayTeam]->(m:FootballTeam) where n.id='" + r.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam away = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
                r.awayTeam = away;
            }
            return players;
        }
        public static List<Round> GetRounds(IGraphClient client)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Round) return n",
                                                queryDict, CypherResultMode.Set);
            List<Round> players = ((IRawGraphClient)client).ExecuteGetCypherResults<Round>(query).ToList();
            foreach(Round r in players)
            {
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:Round)-[:homeTeam]->(m:FootballTeam) where n.id='"+r.id+"' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam home = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
                r.homeTeam = home;
                query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) MATCH (n:Round)-[:awayTeam]->(m:FootballTeam) where n.id='" + r.id + "' return m",
                                                queryDict, CypherResultMode.Set);
                FootballTeam away = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballTeam>(query).ToList().FirstOrDefault();
                r.awayTeam = away;
            }
            return players;
        }
        public static void AddPlayerToRound(IGraphClient client,string roundID, string playerID, float score,RedisClient redisClient)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("score", score);
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (r:Round {id:'" + roundID + "'})" +
                "MATCH (p:FootballPlayer {id:'" + playerID + "'})" +
                "CREATE (p)-[:playedIn{score:" + score + "}]->(r) return p",
                                                            queryDict, CypherResultMode.Set);

            List<FootballPlayer> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Player)-[:hasInTeam]->(m:FootballPlayer) WHERE m.id='"+playerID+" return n",
                                                            queryDict, CypherResultMode.Set);
            List<Player> playerFollowing = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
            foreach(Player p in playerFollowing)
            {
                RedisLogic.AddScoreToUser(redisClient, p.username, score);
            }

        }
        public static bool ChangePlayerRoundScore(IGraphClient client,string roundID, string playerID, float score)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("score", score);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (m:FootballPlayer{id:'" + playerID + "'})-[r]->(n:Round{id:'" + roundID + "'}) return n", queryDict, CypherResultMode.Set);
            List<Round> player = ((IRawGraphClient)client).ExecuteGetCypherResults<Round>(query).ToList();

            if (player.Count() == 0)
            {
                return false;
            }
            query = new Neo4jClient.Cypher.CypherQuery(
                "MATCH (p:FootballPlayer {id:'" + playerID + "'})-[r:playedIn]->(m:Round {id:'" + roundID + "'}) SET r.score=" +
                +score + "return p",
                                                            queryDict, CypherResultMode.Set);

            List<FootballPlayer> actors = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            return true;
        }
        public static void DeletePlayerFromRound(IGraphClient client,string roundID, string playerID, RedisClient redisClient)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("match (n:FootballPlayer{id:'" + playerID + "'})-[r:playedIn]-(m:Round {id:'" + roundID + "'}) return r", queryDict, CypherResultMode.Set);
            PlayedIn playedIn = ((IRawGraphClient)client).ExecuteGetCypherResults<PlayedIn>(query).ToList().FirstOrDefault();

            query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Player)-[:hasInTeam]->(m:FootballPlayer) WHERE m.id='" + playerID + " return n",
                                                            queryDict, CypherResultMode.Set);
            List<Player> playerFollowing = ((IRawGraphClient)client).ExecuteGetCypherResults<Player>(query).ToList();
            foreach (Player p in playerFollowing)
            {
                RedisLogic.RemoveScoreFromUser(redisClient, p.username, playedIn.score);
            }
            query = new Neo4jClient.Cypher.CypherQuery("match (n:FootballPlayer{id:'" + playerID + "'})-[r:playedIn]-(m:Round {id:'" + roundID + "'}) delete r", queryDict, CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(query);
        }

        public static List<FootballPlayer> GetPlayersWhoPlayedInRound(IGraphClient client,string roundID)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:FootballPlayer)-[:playedIn]->(r:Round{id:'" + roundID + "'}) return n",
                                                queryDict, CypherResultMode.Set);
            List<FootballPlayer> players = ((IRawGraphClient)client).ExecuteGetCypherResults<FootballPlayer>(query).ToList();
            return players;
        }
        #endregion



        public static string getFPMaxID(IGraphClient client)
        {
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:FootballPlayer) where exists(n.id) return max(n.id)",
                                                new Dictionary<string, object>(), CypherResultMode.Set);

            String maxId = ((IRawGraphClient)client).ExecuteGetCypherResults<String>(query).ToList().FirstOrDefault();

            return maxId;
        }

        public static string getFTMaxID(IGraphClient client)
        {
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:FootballTeam) where exists(n.id) return max(n.id)",
                                                new Dictionary<string, object>(), CypherResultMode.Set);

            String maxId = ((IRawGraphClient)client).ExecuteGetCypherResults<String>(query).ToList().FirstOrDefault();

            return maxId;
        }
        public static string getRoundMaxID(IGraphClient client)
        {
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n:Round) where exists(n.id) return max(n.id)",
                                                new Dictionary<string, object>(), CypherResultMode.Set);

            String maxId = ((IRawGraphClient)client).ExecuteGetCypherResults<String>(query).ToList().FirstOrDefault();

            return maxId;
        }
    }
}

