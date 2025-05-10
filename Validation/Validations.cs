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
    }
}
