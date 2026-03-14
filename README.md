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

### Validation

### Swagger Documentation

### Logging & Observability

### Users & Auth

---

## AI Usage
