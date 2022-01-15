using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NBP_1_2.Neo4J;
using NBP_1_2.Redis;
using NBP_1_2.Models;
using Neo4jClient;
using ServiceStack.Redis;
namespace NBP_1_2.Controllers
{
    public class RecommendationsController : Controller
    {

        private readonly IGraphClient client;
        private readonly RedisClient redisClient;
        public RecommendationsController(IGraphClient client, RedisClient redisClient)
        {
            this.client = client;
            this.redisClient = redisClient;
        }

        public IActionResult Index(string? whatToDo)
        {
            List<FootballPlayer> players = new List<FootballPlayer>();
            if (whatToDo == null)
            {
                return View(players);
            }
            else
            {
                if (whatToDo == "Friends")
                {
                    string username = HttpContext.Session.GetString("username");
                    players = Neo4JLogic.RecommendPlayersFromFriends(client, username);
                    return View(players);
                }
                else if(whatToDo == "Followed")
                {
                    string username = HttpContext.Session.GetString("username");
                    players = Neo4JLogic.RecommendPlayerFromFollowedTeams(client, username);
                    return View(players);
                }
                else
                {
                    return View(players);
                }
            }    
        }
        public IActionResult GetFriends()
        {
            return RedirectToAction("Index", new { whatToDo = "Friends" });
        }
        public IActionResult GetFollowed()
        {
            return RedirectToAction("Index", new { whatToDo = "Followed" });
        }
        public IActionResult GetNewFriends()
        {
            string username = HttpContext.Session.GetString("username");
            List<Player> players = Neo4JLogic.RecommendFriends(client, username);

            return View(players);
        }
    }
}
