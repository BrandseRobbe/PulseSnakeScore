using System;
using System.Collections.Generic;
using System.Text;

namespace PulseSnakeScoreAPI.Models
{
    class ScoreObject
    {
        public string Name { get; set; }
        public Guid ScoreId { get; set; }
        public DateTime Date { get; set; }
        public string ScoreType { get; set; }
        public double Score { get; set; }
        public int Minuten { get; set; }
        public double ScorePerMinuut { get; set; }
    }
}
