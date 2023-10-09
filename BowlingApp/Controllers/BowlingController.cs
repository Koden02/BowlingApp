using Microsoft.AspNetCore.Mvc;
using BowlingApp;

namespace BowlingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BowlingController : ControllerBase
    {
        private BowlingGame _bowlingGame;

        public BowlingController()
        {
            _bowlingGame = new BowlingGame();
        }

        [HttpPost("calculateScore")]
        public IActionResult CalculateScore([FromBody] int rollNumber)
        {
            // Sanity check the values
            int validRoll = 0;
            if (rollNumber < 1) { validRoll = 1; }
            else if (rollNumber > 20) {  validRoll = 20; }

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

    }
}
