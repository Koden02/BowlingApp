using Microsoft.AspNetCore.Mvc;

namespace BowlingGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BowlingController : ControllerBase
    {
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
            // Implement your logic to calculate the number of pins knocked down
            // For example, you can have a switch statement to handle different random number cases
            // and return the corresponding number of pins knocked down based on your game rules.
            // Replace this with your actual game logic.
            return randomNumber; // Placeholder logic, replace with actual calculation
        }

        private void NewGame()
        {
            // Reset the game to start from scratch.

        }
    }
}
