# ripple-gt : Event Management

## About Me

Hello!

Thank you for taking the time to read this README! And for considering me for this role!
My name is Keith,
And I hope we get to work together in the future! :D

## Run Instructions

### Docker

This entire project is dockerised.
I did this to make it as easy as possible to run on any machine, as well as because I believe developing with it already containerised helps in the long run.
Building an application with infrastructure and the knowledge of how it well get released / deployed is in my opinion very important.

### Install Docker Desktop

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Clone the Repo

```bash
git clone <repo-url>
```

### Get To The Docker Folder

```bash
cd <repo-folder>/Docker
```

### Docker Compose

```bash
docker compose up --build
```

And that should be it! It should build and spin up automatically

### Once Deployed

API: `http://localhost:8080`
Swagger: `http://localhost:8080/swagger`

Feel free to mess around!

### Something Interesting About This Docker Deployment

I'm a fan of using chiselled images when developing / working with Docker when possible.
[Information On Chiselled Images](https://devblogs.microsoft.com/dotnet/announcing-dotnet-chiseled-containers/)

They essentially don't have a distro or a shell.
It greatly reduces attack surface, since no new tools can be installed once the image is built, and you cannot use the shell at all.
No ssh.
No cloning code.

It has some drawbacks too, but for this sort of purpose it works well!

---

## Assumptions

This is just some of the assumptions I made as part of this test.
I made these purely based on the time it would take to solve, in a real world environment I'd be directing a lot of the questions surrounding these assumptions to a customer, product owner or stake holder to get the most accurate definition of what needs to be done.

- Scale: I assumed a moderate scale, so not TicketMaster or anything similar. That sort of scale would require a vaster degree of infrastructure and orchestration.
- Single Region: This was purely to simplify the design here. A globally distributed system introduces a lot more challenges around consistency.
- Single Instance: The RowVersion concurrency would not work for horizontal scaling, or at least would introduce a lot more problems. If there were more instances it would require a lot more care and thought into how a user gets their tickets.

## Decisions

### General Structure

I decided to go for a fairly typical structure for a .NET Web Project (at least typical in my experience).

An API Layer, that handles simple validation & request / response.
A Service Layer, that handles internal business logic.
A Repository, for data access to separate database accessing from operating on the entities.
A Domain project, to contain Entity's and their logic.
A Infrastructure project, to contain the configuration for database access along with the configuration for the Entities and migrations to the database.

### Controllers over Minimal API

This is just a preference.
I find controllers more manageable, and have more experience.
For a two hour test I thought it best to do what's most comfortable.

### EF Core

I decided to use EF Core / an ORM over something like Dapper, or writing my own SQL files to generate / create the tables.

I think this increases the developer experience (for the most part, EF Core has it's own headaches), and for a two hour assessment it also allowed me to focus almost entirely on writing code and getting things correct internally.

In a production system there's be a lot to talk about with regards to database management and the best way to access it, but I think EF Core handles 90% of what needs to be done.

### Results Pattern

For returning errors / successes back to the API to map to HTTP codes, I typically prefer to use something like the Results pattern.
For a time boxed exercise like this I could have done something like throwing exception and catching but that's expensive and it never looks that great to me.

I decided to just use Results here for this, but it isn't perfect as the controller cannot differentiate between the different types of failures and respond accordingly.

This is something that would need to be addressed in the future if this was going to production.

### Indexes

I added indexes to a few fields that I assumed would be a high query field.

Event:

- Venue
- Date

PricingTier:

- EventId

Ticket:

- PricingTierId

### Preventing Overselling

The main contention behind overselling is ensuring that two writes on the final ticket don't come in at the same time.
In a perfect world, we would understand a lot about the system ahead of this:

- How many tickets are there going to be available
- How popular is this event, is it likely there'll be thousands, hundreds of thousands or millions of people attempting to purchase tickets all at the same time
- How globally distributed will this connection be

For the purposes of this test, I've made some assumptions about the system, and solved the problem in a way that I don't think is necessarily the most optimal way.
Given that this is meant to be roughly two hours though I think that's ok.

I'm going to use a RowVersion or something similar in EF Core to track changes.
This means if two people read that there's one ticket left and attempt to purchase it, whoever writes first will update RowVersion (from say 1 to 2).
The second user will attempt to insert a ticket given that the RowVersion is still 1.

This has lots of cons.
It means the user gets kicked out at purchase time, which would send me into a fit of rage.
I chose this over something else, like row locking, because this would prevent even reading if any tickets are available, which is also infuriating but I think keeping reads open as much as possible is a good thing.

In a production environment you'd use some kind of reservation system.
You'd get a ticket with a timer and if you don't purchase your ticket in time it goes back out to the rest of the people waiting for availability.

#### Why not use PostgreSQL's inbuilt type for this

I know that Postgres has it's own RowVersion handling that you can use. It's as easy to configure but you don't have to update the version yourself.

I don't like doing this personally.

- I've had to change database providers a few times on a few projects, and each time remembering to update stuff like this is a pain.
- Postgres doesn't show you this value, which means it's harder to see what version was happening at specific times. I'd rather be able to see it.

I know it's nice it does it for you, but I'd rather just do it myself for the flexibility, and to avoid trying to debug it.
I didn't want to spend 30 minutes of this two hour exericse double checking some internal PostgreSQL numbers if I was getting concurrency issues :/

---

## Changes I Would Make / Next Steps

### Configuration Of Secrets

For the purposes of this I'm taking a few liberties with this.
Obviously in production I wouldn't be passing users and passwords directly to our database configuration in docker.
Nor would I be setting the connection strings so directly in the appsettings either.

### Result Pattern

As I mentioned previously, the Result pattern I used here is incomplete.
It is returning information for sure but I've not made the endpoints aware of the types of Results coming back.

This was mostly due to time.
If I had more time I would add in more knowledge of this, maybe an enum to denote the type of error or something similar.
This would enable me to more accurately use the HTTP Response Codes to denote what's actually being returned.

### Validation

Normally I would use something like FluentValidation and collect all of the validation rules into a single class for the endpoints.
I would also use ProblemDetails to normalise the responses from the API.

For this I decided simple if statements & data attributes would be sufficient for this.

Normally I dislike using the DataAnnotations because it adds in a layer of API / Request Response knowledge to entities that I think are better not knowing anything about that.
It makes their definitions busy and I like them to be really easily read. The noise makes it a bit harder to fully understand what's going on (not impossible, just much harder).

### Swagger Documentation

There's a lot of documentation / swagger specification style attributes I have not added.
For example, the ProduceResponse attributes and Examples to help enrich the swagger documentation.

I elected to ignore that for this task to focus on features.

### Logging & Observability

I have not added any logging to this take home test.
I have also not instrumented any observability.

In a production environment this would be a much higher priority.
I typically prefer to have a walking skeleton of this sort of thing very early in the project.
It means that we are building in observability and intentional logging as we build.

I have not added any here to ensure I could focus on features.

### Users & Auth

Obviously there's absolutely zero authentication or authorization on any of this.
It would need specific users / roles based on if you were administrating the system and adding events vs a customer who was purchasing and viewing tickets.

This is not something I've even approached here but would be vital in a production application.

### Reporting

I unfortunately did not have enough time to get to reporting during the time I spent on this task.

This would be a read-only (so NoTracking()) GET endpoint that would sort out the following:

- Total tickets sold
- Revenue broken down by pricing tier
- Remaining availability per tier

I think the additional DTO's and Request / Response objects, as well as the service + tests would have required a lot of additional time for me.

### Update PricingTier & Merging

I unfortunately did not get to finish this.
One thing I did do in preparation for this feature was capture the cost paid by the user on the Ticket entity. This means that eventually, if a PricingTier was updated, the actual cost paid by the user would have still been captured on the original record to prevent alterations in costs / problems with reporting down the line.

I did a simple delete & replace for this but it was causing tracking issues with EF Core that frankly wasn't worth spending a ton of time on during this test.

## I would have solved this by merging rather than delete and replace.

## AI Usage

I did use AI for this task.
Also can I just say, I've done some other take home assignments and this one was really refreshing.
It was very open, and giving this sort of flexbility meant it was a lot more enjoyable that other's I've done!

Anyways.
I used AI in two main regards:

- Boilerplate
- Repetitive Generation

I much prefer having more control over the actual ins and outs of whats going on, and a thorough understanding of the internals of how something is being solved.
I allowed AI to generate the skeleton of many of the endpoints, as well as the skeleton of some of unit tests.
Things I never allow AI to do:

- Define Test Cases
- Fully "One Shot" features

I typically constrict the AI to more localised areas, a single file or something similar, so that I provide the context of what needs to be done as well as the safety guards.

I also made sure I wrote all of this documentation (so that my personality came through).
