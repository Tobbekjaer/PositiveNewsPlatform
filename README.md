# Positive News Platform

This project is a small, database-focused prototype built for the *Databases for Developers* course exam.

The platform demonstrates how a **polyglot persistence approach** can be used in a read-heavy system by combining multiple database technologies, each chosen for a specific purpose.

## Core Concepts
- SQL as the transactional write model and single source of truth
- MongoDB as a denormalized read model
- Redis as a key-value cache for frequently accessed data
- Object storage (MinIO) for binary media files
- CQRS to separate read and write operations
- Eventual consistency between write and read models
- Clean Architecture with clear separation of responsibilities

## Architecture
The system is implemented as a single ASP.NET Core Web API using Clean Architecture, with separate layers for Domain, Application, Infrastructure and API.  
All components can be run locally using Docker.

The project is intentionally scoped to focus on **databasedesign, consistency trade-offs and persistence strategies**, rather than feature completeness.
