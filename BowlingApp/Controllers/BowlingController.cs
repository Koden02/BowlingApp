using Microsoft.AspNetCore.Mvc;
using BowlingApp;
using Microsoft.AspNetCore.Cors;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace BowlingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BowlingController : ControllerBase
    {
        private static BowlingGame _bowlingGame = new BowlingGame();

        public BowlingController()
        {
        }

        [HttpPost("calculateScore")]
        public IActionResult CalculateScore([FromBody] JsonElement body)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(body);
            JObject jo = JObject.Parse(json);
            JToken jToken = jo["rollNumber"];
            int rollNumber = (int)jToken;
            // Sanity check the values
            int validRoll;
            if (rollNumber < 1) { validRoll = 1; }
            else if (rollNumber > 20) { validRoll = 20; }
            else validRoll = rollNumber;

            // Calculate the number of pins knocked down based on the random number
            // Implement your game logic here

            int pinsKnockedDown = CalculatePinsKnockedDown(validRoll);

            return Ok(new { pinsKnockedDown });
        }

        private int CalculatePinsKnockedDown(int randomNumber)
        {
            return _bowlingGame.TakeTurn(randomNumber);
        }

        [HttpGet("newGame")]
        public IActionResult NewGame()
        {
            // Reset the game to start from scratch.
            _bowlingGame.NewGame();

            return Ok(new { message = "New Game Started."});
        }

        [HttpGet("getScoreTable")]
        public IActionResult getScoreTable()
        {
            return Ok(_bowlingGame.scoreJson().ToLower());
        }

        [HttpGet("getTotalScore")]
        public IActionResult getTotalScore()
        {
            int totalScore = _bowlingGame.CalculateTotalScore();
            return Ok( new { totalScore } );
        }

        [HttpGet("getScoreList")]
        public IActionResult getScoreList() 
        {
            return Ok(_bowlingGame.CalculateTotalScoreList());
        }
    }
}
