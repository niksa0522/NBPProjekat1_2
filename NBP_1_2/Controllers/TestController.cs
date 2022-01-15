using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using NBP_1_2.Models;
using NBP_1_2.Neo4J;
using Neo4jClient;

namespace NBP_1_2.Controllers
{
    public class TestController : Controller
    {

        private readonly IGraphClient client;
        public TestController(IGraphClient client)
        {
            this.client = client;
        }

        public IActionResult Index()
        {
            IEnumerable<Player> list = Neo4J.Neo4JLogic.ReturnPlayers(client);
            return View(list);
        }
        public IActionResult Welcome(string name, int numTimes = 1)
        {
            ViewData["Message"] = "Hello " + name;
            ViewData["NumTimes"] = numTimes;

            return View();
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Player list = Neo4J.Neo4JLogic.ReturnPlayer(client, id);
            if (list == null)
            {
                return NotFound();
            }
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, [Bind("username,password,money")] Player player) 
        {
            
            if (id == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                bool check = Neo4J.Neo4JLogic.ModifyPlayer(client, player.username, player.password);
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
        
    }
}
