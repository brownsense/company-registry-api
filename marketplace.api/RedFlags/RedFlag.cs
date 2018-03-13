using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.api.RedFlags
{
    public class RedFlag
    {
        public string Comment { get; set; }
        public int EntityId { get; set; }
        public int Id { get; set; }
        public RedFlagStatus Status { get; set; }
        public int Submitter { get; set; }
    }

    public enum RedFlagStatus
    {
        None = 0,
        Pending,
        AwaitingEvaluation,
        AwaitingEntityInput,
        ResolvedInvalid,
        Resolved,
        ResolutionFailed
    }
}
