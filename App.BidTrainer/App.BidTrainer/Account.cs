using System;
using System.Collections.Generic;
using System.Text;

namespace App.BidTrainer
{
    public struct Account
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public int NumberOfBoardsPlayed { get; set; }
        public int NumberOfCorrectBoards { get; set; }
        public TimeSpan TimeElapsed { get; set; }
    }
}
