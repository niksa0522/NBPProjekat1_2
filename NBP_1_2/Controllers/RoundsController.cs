using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NBP_1_2.Neo4J;
using NBP_1_2.Redis;
using NBP_1_2.Models;
using Neo4jClient;
using ServiceStack.Redis;

namespace NBP_1_2.Controllers
{
    public class RoundsController : Controller
    {
        private readonly IGraphClient client;
        private readonly RedisClient redisClient;
        public RoundsController(IGraphClient client, RedisClient redisClient)
        {
            this.client = client;
            this.redisClient = redisClient;
        }
        public async Task<IActionResult> Index(string? roundNum,string? date)
        {
            if (roundNum == null && date == null)
            {
                IEnumerable<Round> players = Neo4JLogic.GetRounds(client);

                return View(players);
            }
            if (roundNum != null && date != null)
            {
                int num = Int32.Parse(roundNum);
                IEnumerable<Round> players = Neo4JLogic.GetRoundDateNum(client, date, num);

                return View(players);
            }
            if (roundNum != null)
            {
                int num = Int32.Parse(roundNum);
                IEnumerable<Round> players = Neo4JLogic.GetRoundNum(client, num);

                return View(players);
            }
            else
            {
                IEnumerable<Round> players = Neo4JLogic.GetRoundDate(client, date);

                return View(players);
            }
        }
        public IActionResult Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Round list = Neo4J.Neo4JLogic.GetRound(client, id);
            if(list.homeTeam!=null)
                list.htID = list.homeTeam.id;
            if(list.awayTeam!=null)
                list.atID = list.awayTeam.id;
            if (list == null)
            {
                return NotFound();
            }
            List<PlayedIn> players = Neo4JLogic.GetPlayersPlayedInRound(client, id);
            list.players = players;
            return View(list);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, [Bind("id,matchDate,roundNum,attendance,htID,atID")] Round player)
        {

            if (id == null)
            {
                return NotFound();
            }
            ModelState.Remove("homeTeam");
            ModelState.Remove("awayTeam");
            ModelState.Remove("players");
            if (ModelState.IsValid)
            {
                bool check = Neo4J.Neo4JLogic.ModifyRound(client, player.id, player.htID, player.atID, player.matchDate,player.attendance,player.roundNum);
                if (check)
                {

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }
            return View(player);

        }
        [HttpPost]
        public async Task<IActionResult> AddPlayerToRound(string? pID, float score, [Bind("id,matchDate,roundNum,attendance,htID,atID")] Round player)
        {
            if (pID != null)
            {
                Neo4J.Neo4JLogic.AddPlayerToRound(client, player.id, pID, score,redisClient);
            }
            return RedirectToAction("Edit", new { id = player.id });
        }
        public async Task<IActionResult> RemovePlayerFromRound(string? pID, [Bind("id,matchDate,roundNum,attendance,htID,atID")] Round player)
        {
            if (pID != null)
            {
                Neo4J.Neo4JLogic.DeletePlayerFromRound(client, player.id, pID,redisClient);
            }
            return RedirectToAction("Edit", new { id = player.id });
        }

        public async Task<IActionResult> Add()
        {
            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("matchDate,roundNum,attendance,htID,atID")] Round player)
        {
            ModelState.Remove("id");
            ModelState.Remove("players");
            ModelState.Remove("homeTeam");
            ModelState.Remove("awayTeam");
            if (ModelState.IsValid)
            {
                Neo4J.Neo4JLogic.AddRound(client, player.htID, player.atID, player.matchDate, player.attendance, player.roundNum);
                return RedirectToAction(nameof(Index));

            }
            return View();

        }

        public async Task<IActionResult> Info(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Round list = Neo4J.Neo4JLogic.GetRound(client, id);
            if (list.homeTeam != null)
                list.htID = list.homeTeam.id;
            if (list.awayTeam != null)
                list.atID = list.awayTeam.id;
            if (list == null)
            {
                return NotFound();
            }
            List<PlayedIn> players = Neo4JLogic.GetPlayersPlayedInRound(client, id);
            list.players = players;
            return View(list);
        }
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Neo4JLogic.DeleteRound(client, id);
            return RedirectToAction(nameof(Index));
        }
    }
}
