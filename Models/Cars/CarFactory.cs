using BCA_Car_Auction.DTOs;
using BCA_Car_Auction.Validation;
using System;
using System.Diagnostics;

namespace BCA_Car_Auction.Models.Vehicles
{
    public interface ICarFactory
    {
        Car Create(CarRequest request);
    }

    public class CarFactory : ICarFactory
    {
        public Car Create(CarRequest request)
        {
            return request.Type switch
            {
                CarType.Truck => new Truck
                (
                    manufacturer: request.Manufacturer,
                    model: request.Model,
                    year: request.Year,
                    startBid: request.StartBid,
                    loadCapacityTons: request.LoadCapacityTons.ThrowIfNull("Load capacity is required for Trucks")
                ),
                CarType.Hatchback => new Hatchback
                (
                    request.Manufacturer,
                    request.Model,
                    request.Year,
                    request.StartBid,
                    request.NumberOfDoors.ThrowIfNull("NumberOfDoors is required for Hatchbacks")
                ),

                CarType.Sedan => new Sedan
                (
                    request.Manufacturer,
                    request.Model,
                    request.Year,
                    request.StartBid,
                    request.NumberOfDoors.ThrowIfNull("NumberOfDoors is required for Sedans")
                ),

                CarType.SUV => new SUV
                (
                    request.Manufacturer,
                    request.Model,
                    request.Year,
                    request.StartBid,
                    request.NumberOfSeats.ThrowIfNull("NumberOfSeats is required for SUV")
                ),
                _ => throw new ArgumentException("Unsupported car type")
            };
        }
    }
}