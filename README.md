
# BCA Car Auction - Project README

## Overview

This project is an in-memory ASP.NET Core Web API designed for managing a car auction system. It includes Users, Cars, Auctions, and Bids, with business logic tailored to strict domain rules and concurrency-safe operations. There is no database, UI, or authentication included by design.

## Key Project Assumptions

### General

- No delete operations.
- No updates to names, only state changes.
- No user login or authentication.
- No user interface (UI).
- No database or ORM (like Entity Framework).
- ASP.NET Core Web API as the request/response interface.
- Singleton pattern is used for core services (UserService, CarService, AuctionService).
- Concurrency is handled using `ConcurrentDictionary` and thread-safe operations.

## Domain Logic

### User

- Users are represented as entities.
- Each user can insert cars and owns the ones they add.
- Users can open/close only their own auctions.
- Users cannot place bids on their own auctions.

### Car

- Cars are stored in a `ConcurrentDictionary`.
- The Factory Design Pattern is used to create four types of car subclasses.
- User existence is verified before adding a car.
- Cars have three states: `Available`, `OnAuction`, `Sold`.

### Auction

- Closed auctions cannot be reopened.
- Only the owner can close their auction.
- Auctions cannot be opened or closed if they do not exist.
- Closing an auction marks the car as `Sold`.
- Auctions have two states: `Open` and `Closed`.

### Bid

- Bids must be higher than the latest bid.
- Auction owners cannot place a bid.
- Bids must be placed on existing cars and active auctions.
- Bids are stored in reverse order (highest bid first) within the auction.

## Technical Highlights

### Services

- All services are singleton instances to mimic persistent in-memory state:

  ```csharp
  // UserService
  public class UserService
  {
      private readonly ConcurrentDictionary<int, User> _users = new ConcurrentDictionary<int, User>();
  }
  
  // CarService
  public class CarService
  {
      private readonly ConcurrentDictionary<int, Car> _cars = new ConcurrentDictionary<int, Car>();
  }
  
  // AuctionService
  public class AuctionService
  {
      private readonly ConcurrentDictionary<int, Auction> _auctions = new ConcurrentDictionary<int, Auction>();
  }
  ```

### Concurrency

- `ConcurrentDictionary` is used for storing shared entities.
- Locks are used where necessary (e.g., for bid list modifications).

### ID Generation

- IDs are generated internally and sequentially with thread safety using:

  ```csharp
  private static int _nextId = 0;
  public int Id { get; init; }
  
  protected static int GetNextId() => Interlocked.Increment(ref _nextId);
  ```

This guarantees unique, tamper-proof IDs without external input, removing the need for ID validation logic.

## Further Improvements

- Implement an ORM system.
- Create a frontend with React/Angular.
- Improve error handling and test structure.

## Conclusion

This project adheres to a clear and consistent set of constraints and assumptions. The resulting architecture is clean, concurrency-aware, and well-suited for showcasing backend logic and service-layer design, without relying on external systems like databases or frontends.
