import React, { useState } from 'react';


const BowlingGame = () => {
    const [diceRollResult, setDiceRollResult] = useState(0);
    const [pinsKnockedDown, setPinsKnockedDown] = useState(null);

    const rollDice = () => {
        const randomNumber = Math.floor(Math.random() * 20) + 1;
        setDiceRollResult(randomNumber);
        const jsonString = JSON.stringify({ rollNumber: randomNumber });

        // Take this random rolled number and send it to the API for it to decide what the result is.
        fetch('https://localhost:7156/api/bowling/calculateScore', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            mode: 'cors',
            cache: 'no-cache',
            credentials: 'same-origin',
            body: jsonString,
            //credentials: 'include', // Use 'include' to send credentials (cookies, etc.) with the request
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                // Handle the response and update the UI here with the result
                const pinsDowned = data.pinsKnockedDown;
                setPinsKnockedDown(pinsDowned);
                // Update the UI
            })
            .catch(error => {
                // Handle/display errors
                console.error('Error:', error);
            });
    };


    const startNewGame = () => {
        // Send a request to start a new game
        fetch('https://localhost:7156/api/bowling/newGame', {
            method: 'GET',
            headers: {
                accept: 'application/json'
            }
            //credentials: 'include', // Use 'include' to send credentials (cookies, etc.) with the request
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                // Handle the response and update the UI
                console.log(data.message); // Log the server response
            })
            .catch(error => {
                // Handle/display errors
                console.error('Error:', error);
            });
    };

    return (
        <div>
            <h1>Bowling App</h1>
            <button onClick={rollDice}>Roll the ball!</button>
            <button onClick={startNewGame}>Start New Game</button>
            {diceRollResult && <p>Dice Roll Result: {diceRollResult}</p>}
            {pinsKnockedDown !== null && <p>Pins Knocked Down: {pinsKnockedDown}</p>}
            {/* Everything else goes here or above */}
        </div>
    );
};

export default BowlingGame;