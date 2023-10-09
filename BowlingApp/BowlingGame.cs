using System;
using System.Collections.Generic;

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

            public int TakeRoll(int skillRoll, int pinsRemaining, bool secondRoll)
            {
                // We are going to just make it simple, divide the 1d20 by 2 and that will be the number of pins knocked down.
                // We can change this later
                PinsKnockedDown = Math.Min(pinsRemaining, skillRoll / 2);
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

            public int RollBall(int rollSkill)
            {
                int pinsKnockedDown = 0;
                if (this.FirstRoll.WasTaken())
                {
                    pinsKnockedDown = this.SecondRoll.TakeRoll(rollSkill, this.PinsRemaining, true);

                }
                else
                {
                    pinsKnockedDown = this.FirstRoll.TakeRoll(rollSkill, this.PinsRemaining, false);
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
            public void RemovePins(int pinsToRemove)
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
            public bool isThirdSpare { get; set; } // In this particular case, you can only get a spare on second or third, so the first is implied.
            public bool isCleared { get; set; } // This is for if all the pins are knocked down so that the system knows that they were reset previously
            public FinalFrame() : base()
            {
                this.ThirdRoll = new Roll();
                this.isFollowupRoll = false;
                this.isSecondStrike = false;
                this.isThirdStrike = false;
                this.isThirdSpare = false;
                this.isCleared = false;
            }

            public new void RemovePins(int pinsToRemove)
            {
                this.PinsRemaining -= pinsToRemove;
                if (this.PinsRemaining < 0)
                    this.PinsRemaining = 0; // sanity check

                if (this.PinsRemaining == 0)
                {
                    this.PinsRemaining = 10; // on the final frame when it's 0 it will replace the pins.
                    this.isFollowupRoll = false;
                    this.isCleared = true;
                }
                else
                {
                    this.isFollowupRoll = true;
                    this.isCleared = false;  // Redundent but there as a safty, at least for now.
                }
            }

            public new int RollBall(int rollSkill)
            {
                int pinsKnockedDown = 0;
                if (this.FirstRoll.WasTaken())
                {
                    if (this.SecondRoll.WasTaken() && (this.isSpare || this.isStrike))
                    {
                        pinsKnockedDown = this.ThirdRoll.TakeRoll(rollSkill, this.PinsRemaining, this.isFollowupRoll);
                    }
                    else if (!this.SecondRoll.WasTaken())
                    {
                        pinsKnockedDown = this.SecondRoll.TakeRoll(rollSkill, this.PinsRemaining, this.isFollowupRoll);
                    }

                }
                else
                {
                    pinsKnockedDown = this.FirstRoll.TakeRoll(rollSkill, this.PinsRemaining, false);
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
                            this.isSecondStrike = true;
                        else if (this.FirstRoll.WasTaken())
                            this.isStrike = true;
                    }
                    else
                    {
                        if (this.ThirdRoll.WasTaken())
                            this.isThirdSpare = true;
                        else if (this.SecondRoll.WasTaken())
                            this.isSpare = true;
                    }
                }

                return pinsKnockedDown;
            }
        }
        private List<Frame> Frames { get; set; } = new List<Frame>();

        public BowlingGame()
        {
            NewGame();

        }
        public void NewGame()
        {
            this.Frames = new List<Frame>();
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
    }
}

