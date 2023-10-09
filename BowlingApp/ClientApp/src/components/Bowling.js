import React, { useState } from 'react';
import axios from 'axios';

const BowlingGame = () => {
    const [diceRollResult, setDiceRollResult] = useState(0);

    const rollDice = () => {
        const randomNumber = Math.floor(Math.random() * 20) + 1;
        setDiceRollResult(randomNumber);

        // Take this random rolled number and send it to the API for it to decide what the result is.
        axios.post('/api/calculateScore', { randomNumber })
            .then(response => {
                // Add the code to update the UI here with the result here.
            })
            .catch(error => {
                // Handle/display errors

            });
    };

    return (
        <div>
            <h1>Bowling App</h1>
            <button onClick={rollDice}>Roll the ball!</button>
            {diceRollResult && <p>Dice Roll Result: {diceRollResult}</p>}
            {/* Everything else goes here or above */}
        </div>
    );
};

export default BowlingGame;