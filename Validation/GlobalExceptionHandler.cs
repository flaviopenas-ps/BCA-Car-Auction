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

    public enum BidResult
    {
        CarNotFound,
        CarAlreadyExists,
        CarAlreadySold,
        AuctionNotFound,
        Failed,
        AlreadySold,
        IlegalBid,
        AuctionIsClosed,
        FailOnCreation,
        FailOnClosing,
        AuctionClosed,
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
                var ex = new ArgumentException($"{paramName} cannot be null or empty", paramName);
                LogError($"Validation failed: parameter '{paramName}' is null or empty.", ex);
                throw ex;
            }
            return value;
        }

        public static T ThrowIfNull<T>(this T value, string message) where T : class
        {
            if (value is null)
            {
                var ex = new ArgumentNullException(message);
                LogError($"Validation failed: reference type is null. Message: {message}", ex);
                throw ex;
            }
            return value;
        }

        public static T ThrowIfNull<T>(this T? value, string message) where T : struct
        {
            if (!value.HasValue)
            {
                var ex = new ArgumentException(message);
                LogError($"Validation failed: nullable value type has no value. Message: {message}", ex);
                throw ex;
            }
            return value.Value;
        }

        //CarNotFound,
        //CarAlreadyExists,
        //CarAlreadySold,
        //AuctionNotFound,
        //Failed,
        //AlreadySold,
        //IlegalBid,
        //AuctionIsClosed,
        //FailOnCreation,
        //FailOnClosing,
        //AuctionClosed,

        public static decimal ThrowIfBidTooLow(this decimal bidAmount, decimal minimumBid, string paramName)
        {
            if (bidAmount <= minimumBid)
            {
                var ex = new ArgumentOutOfRangeException(paramName, $"Bid amount must be greater than current bid of {minimumBid}.");
                LogError($"Validation failed: bid amount ({bidAmount}) is not greater than minimum ({minimumBid}).", ex);
                throw ex;
            }
            return bidAmount;
        }

        public static bool ValidateNotOnAuction(this Car car)
        {
            if (car.Status == CarStatus.OnAuction)
            {
                string message = $"Car with ID {car.Id} is already on auction.";
                var ex = new Exception(message);
                ValidationExtensions.Logger?.LogWarning("Check Error: {message}", message);
                throw ex;
            }

            return true;
        }
    }
}
