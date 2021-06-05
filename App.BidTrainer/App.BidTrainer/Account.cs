﻿using System;
using System.Collections.Generic;
using System.Text;

namespace App.BidTrainer
{
    public struct Account
    {
        public string id { get; set; }
        public string username { get; set; }
        public int numberOfBoardsPlayed { get; set; }
        public int numberOfCorrectBoards { get; set; }
        public TimeSpan timeElapsed { get; set; }
    }
}