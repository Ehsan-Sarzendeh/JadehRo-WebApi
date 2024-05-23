using System.ComponentModel.DataAnnotations;
using JadehRo.Database.Entities.Base;
using JadehRo.Database.Entities.Users;

namespace JadehRo.Database.Entities.Trip;

public class TripReq : BaseEntity
{
    public TripReqStatus Status { get; set; }

    public DateTime SeenDateTime { get; set; }
    public DateTime? ReqDateTime { get; set; }
    public DateTime? AcceptOrRejectDateTime { get; set; }

    [StringLength(1000)]
    public string? ReqDescription { get; set; }

    public required long TripId { get; set; }
    public required Trip Trip { get; set; }

    public required long UserId { get; set; }
    public required User User { get; set; }
}

public enum TripReqStatus
{
    Seen = 0,
    Pending = 1,
    Accept = 2,
    Reject = 3
}