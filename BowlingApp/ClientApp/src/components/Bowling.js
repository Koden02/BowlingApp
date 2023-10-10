import React, { useState, useEffect } from 'react';
import './styles.css';

const BowlingGame = () => {
    const [diceRollResult, setDiceRollResult] = useState(0);
    const [pinsKnockedDown, setPinsKnockedDown] = useState(null);

    const [tableData, setTableData] = useState([
        { frame: 1, roll1: '', roll2: '', roll3: ''},
    ]);

    const [calculateTotalScoreList, setCalculateTotalScoreList] = useState([])

    const [totalScore, setTotalScore] = useState(0);

    const [overrideNumber, setOverrideNumber] = useState('');
    const [isOverrideEnabled, setIsOverrideEnabled] = useState(false);

    const [styleTableData, setStyleTableData] = useState([
        { frame:1, roll1: '', roll2: '', roll3: ''

        }
    ]); // These are different and will have the stylized outputs

    const handleOverrideChange = (event) => {
        const inputValue = event.target.value;
        // Ensure the input value is between 0 and 10
        if (inputValue === '' || (parseInt(inputValue) >= 0 && parseInt(inputValue) <= 10)){
            setOverrideNumber(inputValue);
        }
    };

    const toggleOverride = () => {
        setIsOverrideEnabled(!isOverrideEnabled);
    };


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

                let array = JSON.parse(scoreTableData);

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

    const fetchStyleScoreTable = () => {
        fetch('https://localhost:7156/api/bowling/getStyleScoreTable', {
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
            .then(styleScoreTableData => {
                // Handle the response and update the UI with the score table data
                console.log(styleScoreTableData); // Log the server response
                // Update your UI or state with scoreTableData

                let array = JSON.parse(styleScoreTableData);

                if (Array.isArray(array)) {
                    setStyleTableData(array);
                } else {
                    console.error('Invalid data format received:', styleScoreTableData);
                }
            })
            .catch(error => {
                // Handle/display errors for getScoreTable endpoint
                console.error('Error:', error);
            });
    };


    const rollDice = () => {
        let randomNumber = Math.floor(Math.random() * 11);
        if (isOverrideEnabled && overrideNumber !== '') {
            // Use the override number if override mode is enabled and a valid number is provided
            randomNumber = parseInt(overrideNumber);
        }
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
                fetchScoreList();
                fetchStyleScoreTable();


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
                fetchScoreList();
                fetchStyleScoreTable();
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

    const fetchScoreList = () => {
        fetch('https://localhost:7156/api/bowling/getScoreList', {
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
            .then(scoreListData => {
                // Handle the response and update the UI with the score list data
                console.log(scoreListData); // Log the server response
                setCalculateTotalScoreList(scoreListData);
            })
            .catch(error => {
                // Handle/display errors for getScoreList endpoint
                console.error('Error:', error);
            });
    };

    // Fetch the initial score table data when the component is mounted
    useEffect(() => {
        fetchScoreTable();
        fetchTotalScore();
        fetchScoreList();
        fetchStyleScoreTable();
    }, []); // Empty dependency array ensures this effect runs once after the initial render


    return (
        <div className="table-container">
            <h1>Bowling App</h1>
            <p><button className="button-56" onClick={startNewGame}>Start New Game</button></p>
            <p><button className="button-56" onClick={rollDice}>Roll the ball!</button></p>
            <div className="override-container">
                <label>
                    Override Number:
                    <input
                        type="number"
                        value={overrideNumber}
                        onChange={handleOverrideChange}
                        disabled={!isOverrideEnabled}
                        placeholder="Enter value (0-10)"
                    />
                </label>
                <button onClick={toggleOverride}>
                    {isOverrideEnabled ? 'Disable Override' : 'Enable Override'}
                </button>
            </div>
            <p></p>
            <p></p>
            {pinsKnockedDown == -1 && <h2>Game Over!</h2>}
            {diceRollResult > 0 && <p>Dice Roll Result: {diceRollResult}</p>}
            {pinsKnockedDown != null && pinsKnockedDown != -1 && <p>Pins Knocked Down: {pinsKnockedDown}</p>}
            {/* Everything else goes here or above */}
            <p></p>
            <p></p>
            <h2>Score:</h2>
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
                        <tr key={row.frame}>
                            <td>{row.frame}</td>
                            <td>{row.roll1}</td>
                            <td>{row.roll2}</td>
                            <td>{row.roll3}</td>
                        </tr>
                    )) }
                </tbody>
            </table>
            <p>Total Score: {totalScore}</p>
            <p></p>
            <p></p>
            {/* Bowling score sheet */}
            <div className="score-sheet">
                {styleTableData.map((frame, index) => (
                    <div key={frame.frame} className="frame">
                        <div className="frame-number">{frame.frame}</div>
                        <div className="rolls">
                            <div className="roll">{frame.roll1}</div>
                            <div className="roll">{frame.roll2}</div>
                            {frame.roll3 != "" && <div className="roll">{frame.roll3}</div> }
                        </div>
                        <div className="score">{calculateTotalScoreList[index]}</div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default BowlingGame;