using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Validation;
using System.ComponentModel.DataAnnotations;

namespace BCA_Car_Auction.DTOs
{
    public class CarRequest
    {
        [Required]
        public CarType Type { get; set; }

        [Required]
        public string Manufacturer { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [YearDateValidation(1900)]
        public int Year { get; set; }

        [Required]
        public decimal StartBid { get; set; }

        public int? NumberOfDoors { get; set; }
        public int? NumberOfSeats { get; set; }
        public double? LoadCapacityTons { get; set; }
    }
}
