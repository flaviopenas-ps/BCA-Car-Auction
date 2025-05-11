using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using BCA_Car_Auction.Models.Auctions;
using BCA_Car_Auction.Models.Vehicles;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BCA_Car_Auction.Validation
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Error Message: {Message}, Ocurred at: {Time}",
                exception.Message, DateTime.UtcNow);

            ProblemDetails problemDetails = new ProblemDetails()
            {
                Title = exception.GetType().Name,
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

            return true;
        }
    }

    public static class ValidationExtensions
    {
        // Static logger that must be set by the consuming code
        public static ILogger? Logger { get; set; }

        private static void LogError(string contextMessage, Exception ex)
        {
            Logger?.LogError(ex, contextMessage);
        }

        public static string ThrowIfNullOrEmpty(this string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                var ex = new ArgumentNullException($"{paramName} cannot be null or empty", paramName);
                LogError($"Validation failed: parameter '{paramName}' is null or empty.", ex);
                throw ex;
            }
            return value;
        }

        public static T ThrowIfNull<T>(
        [NotNull] this T? value,
        string message) where T : class
        {
            if (value == null)
            {
                var ex = new Exception(message);
                LogError($"Validation failed: reference type is null. Message: {message}", ex);
                throw ex;
            }

            return value;
        }

        public static T ThrowIfNull<T>(this T? value, string message) where T : struct
        {
            if (!value.HasValue)
            {
                var ex = new Exception(message);
                LogError($"Validation failed: nullable value type has no value. Message: {message}", ex);
                throw ex;
            }
            return value.Value;
        }

        public static int ThrowIfIntTooLow(this int bidAmount, int minimumBid, string paramName)
        {
            if (bidAmount <= minimumBid)
            {
                var message = $"Number ({bidAmount}) must be greater than ({minimumBid}).";
                var ex = new ArgumentOutOfRangeException(paramName, message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return bidAmount;
        }

        public static decimal ThrowIfBidTooLow(this decimal bidAmount, decimal minimumBid, string paramName)
        {
            if (bidAmount <= minimumBid)
            {
                var message = $"Bid amount ({bidAmount}) must be greater than current bid ({minimumBid}).";
                var ex = new ArgumentOutOfRangeException(paramName, message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return bidAmount;
        }

        public static bool ThrowIfOnAuction(this Car car)
        {
            if (car.Status == CarStatus.OnAuction)
            {
                string message = $"Car with ID {car.Id} is already on auction.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }

            return true;
        }

        public static bool ThrowIfIlegalBid(this bool isIlegal)
        {
            if (isIlegal == true)
            {
                string message = $"You can't bid on your own auction";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex,"Check Error: {message}", message);
                throw ex;
            }

            return true;
        }

        public static bool ThrowIfCarNotFound(this Car car)
        {
            if (car == null)
            {
                string message = "Car not found.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return true;
        }

        public static void ThrowIfCarAlreadyInAuction(this Car car)
        {
            string message = $"Car already {car.Id} already in Auction.";
            var ex = new Exception(message);
            ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
            throw ex;
        }

        public static bool ThrowIfCarNotAvaiable(this Car car)
        {
            if (car?.Status != CarStatus.Available)
            {
                string message = "Car should be in an avaiable state.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return true;
        }

        public static bool ThrowIfCarNotOnAuction(this Car car)
        {
            if (car?.Status != CarStatus.OnAuction) 
            {
                string message = "Car should be in an auction state.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return true;
        }

        public static bool ThrowIfCarAlreadySold(this Car car)
        {
            if (car?.Status == CarStatus.Sold) 
            {
                string message = "Car already sold.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return true;
        }

        public static bool ThrowIfAuctionNotFound(this Auction auction)
        {
            if (auction == null)
            {
                string message = "Auction not found.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return true;
        }

        public static bool ThrowIfAuctionIllegalBid(this Auction auction, decimal bidAmount)
        {
            if (bidAmount <= auction?.CurrentBid) 
            {
                string message = "Illegal bid amount.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return true;
        }

        public static bool ThrowIfAuctionIsClosed(this Auction auction)
        {
            if (auction?.IsActive == false) 
            {
                string message = "Auction is closed.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return true;
        }

        public static bool ThrowIfAuctionFailOnCreation(this Auction auction)
        {
            if (auction == null) 
            {
                string message = "Failed to create auction.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return true;
        }

        public static bool ThrowIfAuctionFailOnClosing(this Auction auction)
        {
            if (auction?.IsActive == true) 
            {
                string message = "Failed to close auction.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
                throw ex;
            }
            return true;
        }

        public static void ThrowIfAuctionFailOnClosing()
        {
            string message = "Failed to create an auction.";
            var ex = new Exception(message);
            ValidationExtensions.Logger?.LogWarning(ex, "Check Error: {message}", message);
            throw ex;
        }

    }
}
