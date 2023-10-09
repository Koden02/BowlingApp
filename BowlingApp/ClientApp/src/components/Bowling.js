import React, { useState } from 'react';
import axios from 'axios';

const axiosInstance = axios.create({ baseURL : 'http://localhost:5071' });


const BowlingGame = () => {
    const [diceRollResult, setDiceRollResult] = useState(0);
    const [pinsKnockedDown, setPinsKnockedDown] = useState(null);

    const rollDice = () => {
        const randomNumber = Math.floor(Math.random() * 20) + 1;
        setDiceRollResult(randomNumber);

        // Take this random rolled number and send it to the API for it to decide what the result is.
        axios.post('http://localhost:5071/api/bowling/calculateScore', { rollNumber: randomNumber }, { headers: { 'Content-Type': 'application/json' }, withCredentials: true})
            .then(response => {
                // Handle the response and update the UI here with the result
                const pinsDowned = response.data.pinsKnockedDown;
                setPinsKnockedDown(pinsDowned);
                // Update the UI
            })
            .catch(error => {
                // Handle/display errors
                console.error('Error: ', error);
            });
    };

    const startNewGame = () => {
        // Send a request to start a new game
        axios.get('http://localhost:5071/api/bowling/newGame')
            .then(response => {
                // Handle the response and update the UI
                console.log(response.data.message); // Log the server response
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