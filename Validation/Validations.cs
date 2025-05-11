using BCA_Car_Auction.DTOs;
using BCA_Car_Auction.Models.Vehicles;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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

    public class PositiveNumberInt : ValidationAttribute
    {

        public int MinYear { get; }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;
            else if (value is int toValidate)
            {
                if (toValidate < 0)
                    return false;
                return true;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be a positive number.";
        }
    }

    public class PositiveNumberDouble : ValidationAttribute
    {

        public int MinYear { get; }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;
            else if (value is double toValidate)
            {
                if (toValidate < 0)
                    return false;
                return true;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be a positive number.";
        }
    }

    public class PositiveNumberDecimal : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            if (value is decimal toValidate)
            {
                return toValidate >= 0;
            }

            // If it's neither a decimal nor a nullable decimal, return false
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be a positive number.";
        }
    }

    public class NameValidator : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;
            else if (value is string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return false;
                }
                // Regular expression to allow letters from any language (Unicode) and spaces
                string pattern = @"^[\p{L}\s]+$";
                // Use the regular expression to validate the name
                return Regex.IsMatch(name, pattern);
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} can't be empty.";
        }
    }

    public class NonEmptyStringValidator : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            if (value is string str)
            {
                return !string.IsNullOrWhiteSpace(str);
            }

            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CarRequestValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not CarRequest request)
                return ValidationResult.Success;

            switch (request.Type)
            {
                case CarType.Truck:
                    if (request.LoadCapacityTons == null)
                        return new ValidationResult("LoadCapacityTons is required for Truck.");
                    if (request.NumberOfSeats != null || request.NumberOfDoors != null)
                        return new ValidationResult("NumberOfSeats and NumberOfDoors must be null for Truck.");
                    break;

                case CarType.SUV:
                    if (request.NumberOfSeats == null)
                        return new ValidationResult("NumberOfSeats is required for SUV.");
                    if (request.LoadCapacityTons != null || request.NumberOfDoors != null)
                        return new ValidationResult("LoadCapacityTons and NumberOfDoors must be null for SUV.");
                    break;

                case CarType.Sedan:
                case CarType.Hatchback:
                    if (request.NumberOfDoors == null)
                        return new ValidationResult($"NumberOfDoors is required for {request.Type}.");
                    if (request.LoadCapacityTons != null || request.NumberOfSeats != null)
                        return new ValidationResult("LoadCapacityTons and NumberOfSeats must be null for Sedan or Hatchback.");
                    break;

                default:
                    return new ValidationResult("Unsupported car type.");
            }

            return ValidationResult.Success;
        }
    }
}
