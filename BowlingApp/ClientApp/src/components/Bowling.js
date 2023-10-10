import React, { useState, useEffect } from 'react';
import './styles.css';

const BowlingGame = () => {
    const [diceRollResult, setDiceRollResult] = useState(0);
    const [pinsKnockedDown, setPinsKnockedDown] = useState(null);

    const [tableData, setTableData] = useState([
        { id: 1, frame: 1, roll1: '', roll2: '', roll3: '' },
    ]);

    const [totalScore, setTotalScore] = useState(0);

    const fetchScoreTable = () => {
        fetch('https://localhost:7156/api/bowling/getScoreTable', {
            method: 'GET',
            headers: {
                accept: 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(scoreTableData => {
                // Handle the response and update the UI with the score table data
                console.log(scoreTableData); // Log the server response
                // Update your UI or state with scoreTableData

                var array = JSON.parse(scoreTableData);

                if (Array.isArray(array)) {
                    setTableData(array);
                } else {
                    console.error('Invalid data format received:', scoreTableData);
                }
            })
            .catch(error => {
                // Handle/display errors for getScoreTable endpoint
                console.error('Error:', error);
            });
    };

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

                // Call the getScoreTable endpoint after getting the number of pins knocked down
                fetchScoreTable();
                fetchTotalScore();


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
                fetchScoreTable();
                fetchTotalScore();
                setDiceRollResult(0);
                setPinsKnockedDown(null);
            })
            .catch(error => {
                // Handle/display errors
                console.error('Error:', error);
            });
    };

    const fetchTotalScore = () => {
        fetch('https://localhost:7156/api/bowling/getTotalScore', {
            method: 'GET',
            headers: {
                accept: 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                // Handle the response and update the UI with the total score data
                console.log(data); // Log the server response
                setTotalScore(data.totalScore); // Assuming the response has a property called totalScore
            })
            .catch(error => {
                // Handle/display errors for getTotalScore endpoint
                console.error('Error:', error);
            });
    };

    // Fetch the initial score table data when the component is mounted
    useEffect(() => {
        fetchScoreTable();
        fetchTotalScore();
    }, []); // Empty dependency array ensures this effect runs once after the initial render


    return (
        <div className="table-container">
            <h1>Bowling App</h1>
            <p><button onClick={startNewGame}>Start New Game</button></p>
            <p></p>
            <button onClick={rollDice}>Roll the ball!</button>
            <p></p>
            {diceRollResult !== 0 && <p>Dice Roll Result: {diceRollResult}</p>}
            {pinsKnockedDown !== null && <p>Pins Knocked Down: {pinsKnockedDown}</p>}
            {/* Everything else goes here or above */}
            <p></p>
            <p>Score:</p>
            {/* HTML table */}
            <table className="custom-table">
                <thead>
                    <tr>
                        <th>Frame</th>
                        <th>Roll 1</th>
                        <th>Roll 2</th>
                        <th>Roll 3</th>
                    </tr>
                </thead>
                <tbody>
                    {tableData.map((row) => (
                        <tr key={row.id}>
                            <td>{row.frame}</td>
                            <td>{row.roll1}</td>
                            <td>{row.roll2}</td>
                            <td>{row.roll3}</td>
                        </tr>
                    )) }
                </tbody>
            </table>
            <p>Total Score: {totalScore}</p>
        </div>
    );
};

export default BowlingGame;