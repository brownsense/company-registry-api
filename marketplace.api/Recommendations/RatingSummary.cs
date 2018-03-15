using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.api.Recommendations
{
    public class RatingSummary
    {
        public int FiveStars { get; set; }
        public int FourStars { get; set; }
        public int ThreeStars { get; set; }
        public int TwoStars { get; set; }
        public int OneStar { get; set; }
    }
}
