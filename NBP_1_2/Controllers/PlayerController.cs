using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NBP_1_2.Neo4J;
using NBP_1_2.Redis;
using NBP_1_2.Models;
using Neo4jClient;
using ServiceStack.Redis;

namespace NBP_1_2.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IGraphClient client;
        private readonly RedisClient redisClient;
        public PlayerController(IGraphClient client, RedisClient redisClient)
        {
            this.client = client;
            this.redisClient = redisClient;
        }
        public async Task<IActionResult> Index()
        {
            string username = HttpContext.Session.GetString("username");
            if (username == null)
            {
                return Redirect("/Login/Index");
            }
;           List<Player> friends = Neo4JLogic.GetFriends(client, username);
            foreach(Player p in friends)
            {
                p.score = RedisLogic.GetScoreFromUser(redisClient, p.username);
            }
            return View(friends);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string? password)
        {
            if(password != null)
            {
                string username = HttpContext.Session.GetString("username");
                Neo4JLogic.ModifyPlayer(client, username, password);
            }
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> AddFriend(string? fusername)
        {
            if (fusername != null)
            {
                string username = HttpContext.Session.GetString("username");
                Neo4JLogic.AddFriend(client, username, fusername);
            }
            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Info(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Player p = Neo4JLogic.ReturnPlayer(client, id);
            if (p == null)
            {
                return NotFound();
            }
            p.score = RedisLogic.GetScoreFromUser(redisClient, p.username);
            p.inTeam = Neo4JLogic.GetPlayersInTeam(client, p.username);
            return View(p);
        }
        public async Task<IActionResult> Remove(string? id)
        {
            string username = HttpContext.Session.GetString("username");
            if (id == null)
            {
                return NotFound();
            }
            Neo4JLogic.RemoveFriend(client, username, id);
            return RedirectToAction(nameof(Index));
        }
    }
}
