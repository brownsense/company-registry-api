using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.api.Recommendations
{
    public class Rating
    {
        public Stars RatingValue { get; set; }
        public int Entity { get; set; }
        public int RatedBy { get; set; }
        public string Comment { get; set; }
        public int Id { get; set; }
    }

    public enum Stars
    {
        None = 0,
        One,
        Two,
        Three,
        Four,
        Five
    }
}
