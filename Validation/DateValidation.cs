using System.ComponentModel.DataAnnotations;

namespace BCA_Car_Auction.Validation
{
    public class YearDateValidation : ValidationAttribute
    {
        public YearDateValidation(int minYear)
        {
            MinYear = minYear;
        }

        public int MinYear { get; }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;
            else if (value is int year)
            {
                var currentYear = DateTime.Now.Year;
                //verifies if the year is between min and current
                return year >= MinYear && year <= currentYear;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be between {MinYear} and {DateTime.Now.Year}.";
        }
    }
}
