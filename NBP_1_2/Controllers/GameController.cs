using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NBP_1_2.Neo4J;
using NBP_1_2.Redis;
using NBP_1_2.Models;
using Neo4jClient;
using ServiceStack.Redis;

namespace NBP_1_2.Controllers
{
    public class GameController : Controller
    {
        private readonly IGraphClient client;
        private readonly RedisClient redisClient;
        public GameController(IGraphClient client, RedisClient redisClient)
        {
            this.client = client;
            this.redisClient = redisClient;
        }

        public IActionResult Index(string? username)
        {
            if(username == null)
            {
                List<String> lb = RedisLogic.GetLeaderboard(redisClient);
                List<GameModel> model = new List<GameModel>();
                List<String> info = new List<string>();
                List<String> score = new List<string>();
                for (int i = 0; i < lb.Count; i = i + 2)
                {
                    GameModel m = new GameModel();
                    m.info = lb[i];
                    m.score = lb[i + 1];
                    model.Add(m);
                }

                return View(model);
            }
            else
            {
                //username = HttpContext.Session.GetString("username");
                List<Player> friends = Neo4JLogic.GetFriends(client, username);

                List<String> persons = new List<String>();
                persons.Add(username);
                foreach(Player p in friends)
                {
                    persons.Add(p.username);
                }

                List<String> lb = RedisLogic.GetLeaderboardForUsers(redisClient,persons.ToArray());
                List<GameModel> model = new List<GameModel>();
                List<String> info = new List<string>();
                List<String> score = new List<string>();
                for (int i = 0; i < lb.Count; i = i + 2)
                {
                    GameModel m = new GameModel();
                    m.info = lb[i];
                    m.score = lb[i + 1];
                    model.Add(m);
                }

                return View(model);
            }
        }
        public IActionResult GetFriends()
        {
            string username = HttpContext.Session.GetString("username");
            return RedirectToAction("Index", new { username = username });
        }

        public IActionResult Team()
        {
            string username = HttpContext.Session.GetString("username");
            if (username == null)
            {
                return Redirect("/Login/Index");
            }
            Player p = Neo4JLogic.ReturnPlayer(client, username);
            if (p == null)
            {
                return NotFound();
            }
            p.score = RedisLogic.GetScoreFromUser(redisClient, p.username);
            p.inTeam = Neo4JLogic.GetPlayersInTeam(client, p.username);
            return View(p);
        }
        public IActionResult AddToTeam(string? id)
        {
            string username = HttpContext.Session.GetString("username");
            if (id == null)
            {
                return NotFound();
            }
            Neo4JLogic.AddPlayerToTeam(client, username, id,redisClient);
            return Redirect("/Game/Team");
        }
        public IActionResult Delete(string? id)
        {
            string username = HttpContext.Session.GetString("username");
            if (id == null)
            {
                return NotFound();
            }
            Neo4J.Neo4JLogic.RemovePlayerFromTeam(client, username, id, redisClient);
            return Redirect("/Game/Team");
        }
    }
}
