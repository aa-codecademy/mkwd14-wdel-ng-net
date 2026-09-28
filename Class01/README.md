# 🧱 Class 01: Foundations and data

> ⚙️ Backend · class 1 of 5 · 🏠 [Course overview](../README.md) · 🛠️ [Initial setup](Initial-Setup.md) · 🗄️ [Database diagram](database-diagram.png)

From an **empty folder** to a **PostgreSQL database whose tables are created from our C# classes**.

## 🗺️ What we do today

1. 🏛️ The app, its layers and the seven projects
2. 🧩 Create the solution (`PizzaApp.slnx`, the new solution format), the projects and their references
3. 📄 Handout: the shared settings files (`global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`, `dotnet-tools.json`)
4. 🍕 The domain: `BaseEntity`, `Pizza`, `Order`, `User`, and the enums
5. 🗄️ `PizzaAppDbContext` and one configuration class per entity
6. 💉 `AddDataAccess()`: register the `DbContext`
7. 🔐 The connection string in user-secrets
8. 🐘 The first migration, then a look at the tables in pgAdmin

---

## 💡 New concepts
