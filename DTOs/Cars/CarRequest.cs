using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Validation;
using System.ComponentModel.DataAnnotations;

namespace BCA_Car_Auction.DTOs.Cars
{
    [CarRequestValidation]
    public class CarRequest
    {

        [Required]
        public CarType Type { get; set; }

        public CarStatus Status { get; set; } = CarStatus.Available;

        [Required]
        [NonEmptyStringValidator]
        public string Manufacturer { get; set; } = string.Empty;

        [Required]
        [NonEmptyStringValidator]
        public string Model { get; set; } = string.Empty;

        [Required]
        [YearDateValidation(1900)]
        public int Year { get; set; }

        [Required]
        [PositiveNumberDecimal]
        public decimal StartBid { get; set; }

        public int? NumberOfDoors { get; set; }

        public int? NumberOfSeats { get; set; }

        public double? LoadCapacityTons { get; set; }

        [PositiveNumberInt]
        [Required]
        public int UserIdOwner { get; set; }
    }
}
