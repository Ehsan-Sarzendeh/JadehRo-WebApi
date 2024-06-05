using System.ComponentModel.DataAnnotations;
using JadehRo.Database.Entities.Base;
using JadehRo.Database.Entities.Users;

namespace JadehRo.Database.Entities.Trip;

public class TripReq : BaseEntity
{
    public TripReqStatus Status { get; set; }

    public DateTime ReqDateTime { get; set; }
    public DateTime? AcceptOrRejectDateTime { get; set; }

    [StringLength(1000)]
    public string Address { get; set; }

	[StringLength(1000)]
    public string ReqDescription { get; set; }

    [StringLength(1000)]
    public string AcceptOrRejectDescription { get; set; }

	public int PersonCount { get; set; }

    public long TripId { get; set; }
    public Trip Trip { get; set; }

    public long UserId { get; set; }
    public User User { get; set; }
}

public enum TripReqStatus
{
    Pending = 1,
    Accept = 2,
    Reject = 3,
    Cancel = 4
}