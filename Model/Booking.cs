namespace ArisHotel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Booking
    {
        public int BookingId { get; set; }

        public int RoomId { get; set; }

        public int CreatedByUserId { get; set; }

        [Required]
        [StringLength(100)]
        public string GuestName { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        public virtual User User { get; set; }

        public virtual Room Room { get; set; }
    }
}
