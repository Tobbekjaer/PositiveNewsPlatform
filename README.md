# Positive News Platform

Dette projekt er en afgrænset prototype udviklet som en del af faget *Databases for Developers*.

Platformen bruges til at demonstrere, hvordan en **polyglot persistence-tilgang** kan anvendes i et read-heavy system ved at kombinere flere databasetyper, hver valgt ud fra deres styrker og konkrete use cases.

## Centrale koncepter
- SQL som transaktionel write model og single source of truth
- MongoDB som denormaliseret read model
- Redis som key-value cache til ofte læste data
- Object storage (MinIO) til lagring af binære mediefiler
- CQRS til adskillelse af read og write operations
- Eventual consistency mellem write og read models
- Clean Architecture med klar ansvarsadskillelse

## Arkitektur
Systemet er implementeret som en enkelt ASP.NET Core Web API bygget efter Clean Architecture-principperne, opdelt i Domain, Application, Infrastructure og API-lag.

Projektet er bevidst afgrænset med fokus på **databasedesign, konsistens-trade-offs og persistence-strategier** frem for funktionel bredde.
