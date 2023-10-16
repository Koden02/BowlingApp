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
            int pinsKnockedDown = -1; // -1 will show that nothing happened.
            // You should make sure the game is still running so you don't run unneeded
            if (_bowlingGame.IsGameOver())
                return Ok(new {pinsKnockedDown} );
            string json = System.Text.Json.JsonSerializer.Serialize(body);
            JObject jo = JObject.Parse(json);
            JToken jToken = jo["rollNumber"];
            int rollNumber = (int)jToken;
            // Sanity check the values
            int validRoll;
            if (rollNumber < 0) { validRoll = 0; }
            else if (rollNumber > 10) { validRoll = 10; }
            else validRoll = rollNumber;

            // Calculate the number of pins knocked down based on the random number
            // Implement your game logic here

            pinsKnockedDown = CalculatePinsKnockedDown(validRoll);

            return Ok(new { pinsKnockedDown });
        }

        private int CalculatePinsKnockedDown(int skillRollNumber)
        {
            return _bowlingGame.TakeTurn(skillRollNumber);
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

        [HttpGet("getStyleScoreTable")]
        public IActionResult getStyleScoreTable()
        {
            return Ok(_bowlingGame.styleScoreJson().ToLower());
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

        [HttpGet("getIsGameOver")]
        public IActionResult getIsGameOver()
        {
            return Ok(_bowlingGame.IsGameOver());
        }
    }
}
