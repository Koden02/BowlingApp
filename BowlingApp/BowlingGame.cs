﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace BowlingApp
{

    public class BowlingGame
    {
        private class Roll
        {
            private int PinsKnockedDown { get; set; }
            private bool RollTaken { get; set; }
            public Roll()
            {
                PinsKnockedDown = 0; // Default to 0
                RollTaken = false;
            }

            public int TakeRoll(int skillRoll, int pinsRemaining)
            {
                // We are going to just make it simple, 1d11 - 1 and that number is the number of pens dropped.
                // We can change this later
                PinsKnockedDown = Math.Min(pinsRemaining, skillRoll);
                RollTaken = true;

                return PinsDowned();

            }

            public int PinsDowned()
            {
                // Use this to report the number of pins knocked down.
                if (RollTaken)
                {
                    return PinsKnockedDown;
                }
                else { return -1; }
            }

            public int PinsDownedCount()
            {
                // Use this to report the number of pins knocked down for counting, don't use -1
                return PinsKnockedDown;
            }

            public bool WasTaken()
            {
                return RollTaken;
            }
        }

        private class Frame
        {
            public Roll FirstRoll { get; set; }
            public Roll SecondRoll { get; set; }
            public bool isSpare { get; set; }
            public bool isStrike { get; set; }
            public int PinsRemaining { get; set; }
            public bool isFinished { get; set; }

            public Frame()
            {
                this.FirstRoll = new Roll();
                this.SecondRoll = new Roll();
                this.isSpare = false;
                this.isStrike = false;
                this.isFinished = false;
                this.PinsRemaining = 10;
            }

            public virtual int RollBall(int rollSkill)
            {
                int pinsKnockedDown = 0;
                if (this.FirstRoll.WasTaken())
                {
                    pinsKnockedDown = this.SecondRoll.TakeRoll(rollSkill, this.PinsRemaining);

                }
                else
                {
                    pinsKnockedDown = this.FirstRoll.TakeRoll(rollSkill, this.PinsRemaining);
                }

                RemovePins(pinsKnockedDown);

                if (this.PinsRemaining == 0 || this.SecondRoll.WasTaken())
                {
                    this.isFinished = true;
                    if (this.SecondRoll.WasTaken() && this.PinsRemaining == 0)
                        this.isSpare = true;
                    else if (this.FirstRoll.WasTaken() && this.PinsRemaining == 0)
                        this.isStrike = true;
                }

                return pinsKnockedDown;

            }
            public virtual void RemovePins(int pinsToRemove)
            {
                this.PinsRemaining -= pinsToRemove;
                if (this.PinsRemaining < 0)
                    this.PinsRemaining = 0; //sanity check
            }

            public bool isFrameFinished()
            {
                return this.isFinished;
            }
        }

        private class FinalFrame : Frame
        {
            public Roll ThirdRoll { get; set; }
            public bool isFollowupRoll { get; set; }
            public bool isSecondStrike { get; set; }
            public bool isThirdStrike { get; set; }
            public bool isSecondSpare { get; set; } // In this particular case, you can only get a spare on second or third, so the first is implied.
            public bool isCleared { get; set; } // This is for if all the pins are knocked down so that the system knows that they were reset previously
            public FinalFrame() : base()
            {
                this.ThirdRoll = new Roll();
                this.isFollowupRoll = false;
                this.isSecondStrike = false;
                this.isThirdStrike = false;
                this.isSecondSpare = false;
                this.isCleared = false;
            }

            public override void RemovePins(int pinsToRemove)
            {
                this.PinsRemaining -= pinsToRemove;
                if (this.PinsRemaining < 0)
                    this.PinsRemaining = 0; // sanity check

                if (this.PinsRemaining == 0)
                {
                    this.PinsRemaining = 10; // on the final frame when it's 0 it will replace the pins.
                    //this.isFollowupRoll = false;
                    this.isCleared = true;
                }
                else if (this.isFollowupRoll)
                {
                    this.isFinished = true;
                }
                else 
                {
                    this.isFollowupRoll = true;
                    this.isCleared = false;  // Redundent but there as a safty, at least for now.
                }
                if (this.ThirdRoll.WasTaken())
                {
                    this.isFinished = true;
                }
            }

            public override int RollBall(int rollSkill)
            {
                if (this.isFinished)
                    return 0;

                int pinsKnockedDown = 0;
                if (this.FirstRoll.WasTaken())
                {
                    if (this.SecondRoll.WasTaken() && (this.isSpare || this.isStrike))
                    {
                        pinsKnockedDown = this.ThirdRoll.TakeRoll(rollSkill, this.PinsRemaining);
                    }
                    else if (!this.SecondRoll.WasTaken())
                    {
                        pinsKnockedDown = this.SecondRoll.TakeRoll(rollSkill, this.PinsRemaining);
                    }

                }
                else
                {
                    pinsKnockedDown = this.FirstRoll.TakeRoll(rollSkill, this.PinsRemaining);
                }

                RemovePins(pinsKnockedDown);

                if (this.isCleared)
                {
                    this.isCleared = false; // Set to false, we don't need this to flag unless it changes again.
                    if (!this.isFollowupRoll)
                    {
                        if (this.ThirdRoll.WasTaken())
                            this.isThirdStrike = true;
                        else if (this.SecondRoll.WasTaken())
                        {
                            this.isSecondStrike = true;
                            this.isFollowupRoll = false;
                        }
                        else if (this.FirstRoll.WasTaken())
                        {
                            this.isStrike = true;
                            this.isFollowupRoll = false;
                        }
                    }
                    else
                    {
                        if (this.ThirdRoll.WasTaken())
                            this.isSecondSpare = true;
                        else if (this.SecondRoll.WasTaken())
                        {
                            this.isSpare = true;
                            this.isFollowupRoll= false;
                        }
                    }
                }

                return pinsKnockedDown;
            }
        }
        private class FrameData
        {
            public int Frame { get; set; }
            public string? Roll1 { get; set; }
            public string? Roll2 { get; set; }
            public string? Roll3 { get; set; }
        }

        private List<Frame> Frames { get; set; } = new List<Frame>();
        private bool isOver { get; set; } = false;
        public BowlingGame()
        {
            NewGame();

        }
        public bool IsGameOver()
        {
            return isOver;
        }
        public void NewGame()
        {
            this.Frames = new List<Frame>();
            this.isOver = false;
            AddFrame(); // Do an initial AddFrame to add in the first frame of the game.
        }
        public int TakeTurn(int rollSkill)
        {
            int frameIndex = this.Frames.Count - 1;
            int PinsDowned = this.Frames[frameIndex].RollBall(rollSkill);

            if (this.Frames[frameIndex].isFinished)
            {
                AddFrame();
            }

            return PinsDowned;
        }

        private bool AddFrame()
        {
            int frameCount = this.Frames.Count;
            if (frameCount >= 10)
            {
                // A game of bowling shouldn't be more than 10 frames, don't do anything.
                this.isOver = true;
                return false;
            }
            else if (frameCount < 9)
            {
                Frames.Add(new Frame());
            }
            else if (frameCount == 9)
            {
                Frames.Add(new FinalFrame());
            }

            return true;

        }

        public string styleScoreJson()
        {
            List<FrameData> frameData = new List<FrameData>();
            int count = 0;
            foreach (Frame frame in Frames)
            {
                count++;
                string roll1 = "";
                string roll2 = "";
                string roll3 = "";
                bool final = false;

                if (frame.FirstRoll.PinsDowned() != -1)
                {
                    roll1 = frame.FirstRoll.PinsDowned().ToString();
                }
                if (frame.SecondRoll.PinsDowned() != -1)
                {
                    roll2 = frame.SecondRoll.PinsDowned().ToString();
                }
                if (frame is FinalFrame finalFrame)
                {
                    final = true;

                    if (finalFrame.ThirdRoll.PinsDowned() != -1)
                    {
                        roll3 = finalFrame.ThirdRoll.PinsDowned().ToString();
                    }
                }

                if (roll1 == "10")
                {
                    roll1 = "X";
                    if(!final) 
                        roll2 = "";
                }
                if (frame.isSpare)
                {
                    roll2 = "/";
                }
                if (final)
                {
                    if (roll2 == "10")
                    {
                        roll2 = "X";
                    }
                    if (frame is FinalFrame finalFrame2 && finalFrame2.isSecondSpare)
                    {
                        roll3 = "/";
                    }
                    else if (roll3 == "10")
                    {
                        roll3 = "X";
                    }
                }
                frameData.Add(new FrameData
                {
                    Frame = count,
                    Roll1 = roll1,
                    Roll2 = roll2,
                    Roll3 = roll3,
                 
                }) ;
            }
            string jsonString = JsonSerializer.Serialize(frameData);
            return jsonString;
        }

        public int CalculateTotalScore()
        {
            List<int> scoreList = CalculateTotalScoreList();
            return scoreList.Last();
        }

        public List<int> CalculateTotalScoreList()
        {
            int score = 0;
            List<int> scoreList = new List<int>();

            for (int frameNumber = 1; frameNumber <= Frames.Count; frameNumber++)
            {
                Frame currentFrame = Frames[frameNumber - 1];

                if (currentFrame is FinalFrame finalFrame)
                {
                    score += finalFrame.FirstRoll.PinsDownedCount() + finalFrame.SecondRoll.PinsDownedCount() + finalFrame.ThirdRoll.PinsDownedCount();
                }
                else if (currentFrame.isStrike)
                {
                    score += 10;

                    if (frameNumber < Frames.Count)
                    {
                        Frame nextFrame = Frames[frameNumber];

                        if (nextFrame is FinalFrame finalNextFrame)
                        {
                            score += finalNextFrame.FirstRoll.PinsDownedCount();
                            score += finalNextFrame.SecondRoll.PinsDownedCount();
                        }
                        else
                        {
                            score += nextFrame.FirstRoll.PinsDownedCount();

                            if (nextFrame.isStrike)
                            {
                                Frame nextNextFrame = Frames[frameNumber + 1];
                                score += nextNextFrame.FirstRoll.PinsDownedCount();
                            }
                            else
                            {
                                score += nextFrame.SecondRoll.PinsDownedCount();
                            }
                        }
                    }
                }
                else if (currentFrame.isSpare)
                {
                    score += 10;

                    if (frameNumber < Frames.Count)
                    {
                        Frame nextFrame = Frames[frameNumber];
                        score += nextFrame.FirstRoll.PinsDownedCount();
                    }
                }
                else
                {
                    // If neither strike nor spare, add the total number of pins knocked down in the frame to the score
                    score += currentFrame.FirstRoll.PinsDownedCount() + currentFrame.SecondRoll.PinsDownedCount();
                }

                scoreList.Add(score);
            }

            return scoreList;
        }

        public string scoreJson()
        {
            List<FrameData> frameData = new List<FrameData>();
            int count = 0;
            foreach (Frame frame in Frames)
            {
                count++;
                string roll1 = "";
                string roll2 = "";
                string roll3 = "";

                bool secondSpare = false;

                if (frame.FirstRoll.PinsDowned() != -1)
                {
                    roll1 = frame.FirstRoll.PinsDowned().ToString();
                }
                if (frame.SecondRoll.PinsDowned() != -1)
                {
                    roll2 = frame.SecondRoll.PinsDowned().ToString();
                }
                if (frame is FinalFrame finalFrame && finalFrame.ThirdRoll.PinsDowned() != -1)
                {
                    roll3 = finalFrame.ThirdRoll.PinsDowned().ToString();
                    secondSpare = finalFrame.isSecondSpare;
                }



                frameData.Add(new FrameData
                {
                    Frame = count,
                    Roll1 = roll1,
                    Roll2 = roll2,
                    Roll3 = roll3,
                });
            }
            string jsonString = JsonSerializer.Serialize(frameData);
            return jsonString;
        }

    }
}

