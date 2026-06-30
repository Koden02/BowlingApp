using Microsoft.AspNetCore.Mvc;
using BowlingApp;

namespace BowlingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BowlingController : ControllerBase
    {
        private static BowlingGame _bowlingGame = new BowlingGame();
        private static readonly object _gameLock = new object();

        public BowlingController()
        {
        }

        [HttpPost("calculateScore")]
        public IActionResult CalculateScore([FromBody] RollRequest? request)
        {
            int pinsKnockedDown = -1; // -1 will show that nothing happened.

            if (request?.RollNumber is null)
            {
                return BadRequest(new { message = "rollNumber is required." });
            }

            int validRoll = Math.Clamp(request.RollNumber.Value, 0, 10);

            lock (_gameLock)
            {
                // You should make sure the game is still running so you don't run unneeded
                if (_bowlingGame.IsGameOver())
                    return Ok(new { pinsKnockedDown });

                pinsKnockedDown = CalculatePinsKnockedDown(validRoll);
            }

            return Ok(new { pinsKnockedDown });
        }

        private int CalculatePinsKnockedDown(int skillRollNumber)
        {
            return _bowlingGame.TakeTurn(skillRollNumber);
        }

        [HttpPost("newGame")]
        public IActionResult NewGame()
        {
            lock (_gameLock)
            {
                // Reset the game to start from scratch.
                _bowlingGame.NewGame();
            }

            return Ok(new { message = "New Game Started." });
        }

        [HttpGet("getScoreTable")]
        public IActionResult getScoreTable()
        {
            lock (_gameLock)
            {
                return Ok(_bowlingGame.scoreJson().ToLower());
            }
        }

        [HttpGet("getStyleScoreTable")]
        public IActionResult getStyleScoreTable()
        {
            lock (_gameLock)
            {
                return Ok(_bowlingGame.styleScoreJson().ToLower());
            }
        }

        [HttpGet("getTotalScore")]
        public IActionResult getTotalScore()
        {
            int totalScore;
            lock (_gameLock)
            {
                totalScore = _bowlingGame.CalculateTotalScore();
            }

            return Ok(new { totalScore });
        }

        [HttpGet("getScoreList")]
        public IActionResult getScoreList()
        {
            lock (_gameLock)
            {
                return Ok(_bowlingGame.CalculateTotalScoreList());
            }
        }

        [HttpGet("getIsGameOver")]
        public IActionResult getIsGameOver()
        {
            lock (_gameLock)
            {
                return Ok(_bowlingGame.IsGameOver());
            }
        }

        public class RollRequest
        {
            public int? RollNumber { get; set; }
        }
    }
}
