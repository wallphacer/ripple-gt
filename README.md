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

### EF Core

I decided to use EF Core / an ORM over something like Dapper, or writing my own SQL files to generate / create the tables.

I think this increases the developer experience (for the most part, EF Core has it's own headaches), and for a two hour assessment it also allowed me to focus almost entirely on writing code and getting things correct internally.

In a production system there's be a lot to talk about with regards to database management and the best way to access it, but I think EF Core handles 90% of what needs to be done.

### Preventing Overselling

## Changes I Would Make

### Configuration Of Secrets

For the purposes of this I'm taking a few liberties with this.
Obviously in production I wouldn't be passing users and passwords directly to our database configuration in docker.
Nor would I be setting the connection strings so directly in the appsettings either.

## Next Steps
