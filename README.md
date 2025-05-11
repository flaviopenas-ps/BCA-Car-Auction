1.  BCA Car Auction - Project README

    1.  ## Overview

> This project is an in-memory ASP.NET Core Web API for managing a car
> auction system. It includes Users, Cars, Auctions, and Bids, with
> business logic tailored to strict domain rules and concurrency-safe
> operations. No database, UI, or authentication is included by design.

1.  1.  ## Key Project Assumptions

        1.  ### General

> No delete operations

> No updates to names, only state changes

> No user login or authentication

> No user interface (UI)

> No database or ORM (like Entity Framework)

> ASP.NET Core Web API as the request/response interface

> Singleton pattern used for core services (UserService, CarService,
> AuctionService)

> Concurrency is handled using ConcurrentDictionary and thread-safe
> operations

1.  1.  ## Domain Logic

        1.  ### User

> Users are represented as entities

> Each user can insert cars and owns the ones they added

> Users can open/close only their own auctions

> Users cannot place bids on their own auctions

1.  1.  1.  ### Car

> Cars are stored in a ConcurrentDictionary

> Factory Design Pattern is used to create four types of car subclasses

> User existence is verified before adding a car

> Cars have three states: Available, OnAuction, Sold

1.  1.  1.  ### 

        2.  ### 

1.  1.  1.  ### 

        2.  ### Auction

> Closed auctions cannot be reopened

> Auctions for sold cars cannot be opened

> Only the owner can close their auction

> Cannot open/close non-existent auctions

> Closing an auction marks the car as Sold

> Auctions have two states: Open and Closed

1.  1.  1.  ### Bid

> Bids must be higher than the latest bid

> Auction owner cannot place a bid

> Bids must be placed on existing cars and active auctions

> Bids are stored in reverse order (highest bid first) within the
> auction

1.  1.  ## Technical Highlights

        1.  ### Services

> All services are singleton instances to mimic persistent in-memory
> state:

> -UserService

> -CarService

> -AuctionService

1.  1.  1.  ### Concurrency

> ConcurrentDictionary is used for storing shared entities

> Locks are used where necessary (e.g., for bid list modifications)

1.  1.  1.  ### ID Generation

> IDs are generated internally and sequentially with thread safety
> using:

> private static int \_nextId = 0;  
> public int Id { get; init; }  
> protected static int GetNextId() =\> Interlocked.Increment(ref
> \_nextId);

> This guarantees unique, tamper-proof IDs without external input,
> removing the need for ID validation logic.

>   

1.  1.  ## Further Improvements to make

> -ORM system

> -FE with React/Angular

> -Better structure for errors and Tests

1.  1.  ## Conclusion

> This project follows a clear and consistent set of constraints and
> assumptions. The resulting architecture is clean, concurrency-aware,
> and well-suited for showcasing backend logic and service-layer design
> without relying on external systems like databases or frontends.
