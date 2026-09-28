# 🛠️ Initial setup

Install everything below **before the first class**, so we can start coding right away.
Plan about an hour: Visual Studio is a big download.

---

## ⚙️ For the backend (classes 1–5)

### 1️⃣ Visual Studio 2026
1. Download **Visual Studio 2026 Community** (free) from https://visualstudio.microsoft.com ([installation guide](https://learn.microsoft.com/visualstudio/install/install-visual-studio)).
2. In the installer, select the **ASP.NET and web development** workload. It includes the **.NET 10 SDK**.
3. Check it: open a new terminal and run `dotnet --version`. You should see `10.0.x`.

> 🍎 **On macOS or Linux?** Visual Studio is Windows-only. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download), then use **VS Code** with the *C# Dev Kit* extension, or **JetBrains Rider** (free for non-commercial use).

### 2️⃣ PostgreSQL and pgAdmin 4
1. Download the installer from https://www.postgresql.org/download/ (on Windows: the EDB installer).
2. Keep these components: **PostgreSQL Server**, **pgAdmin 4** and **Command Line Tools**. You don't need Stack Builder.
3. Choose a password for the `postgres` user and **write it down**: you'll need it in class 1.
4. Keep the default port, **5432**.
5. Check it: open **pgAdmin**, click **Servers → PostgreSQL**, and enter your password. You should see a database called `postgres`.

---

## 🅰️ For the frontend (classes 6–10)

You only need these from class 6, but you can install them now.

### 3️⃣ Node.js 24 LTS
1. Download the **LTS** version from https://nodejs.org/en/download.
2. Check it: `node -v` shows `v24.x`, and `npm -v` shows a version number.

### 4️⃣ Angular CLI 22
```powershell
npm install -g @angular/cli@22
```
Always install the **`@22`** version: the course uses Angular 22, even after Angular 23 comes out.
Check it: `ng version` shows Angular CLI `22.x`.

### 5️⃣ VS Code
1. Download it from https://code.visualstudio.com.
2. Install these extensions:
   - [Angular Language Service](https://marketplace.visualstudio.com/items?itemName=Angular.ng-template): autocomplete and errors in Angular templates
   - [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode): formats the code
   - [ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint): shows code problems while you type

---

## ✅ Check everything

| Check | You should see |
|---|---|
| `dotnet --version` | `10.0.x` |
| pgAdmin → Servers → PostgreSQL | the `postgres` database, after entering your password |
| `node -v` | `v24.x` |
| `npm -v` | a version number |
| `ng version` | Angular CLI `22.x` |

## 🆘 Something doesn't work?

| Problem | Fix |
|---|---|
| `dotnet`, `node` or `ng` is *not recognized* | Close the terminal and open a new one. If it still fails, restart the computer. |
| PowerShell says *running scripts is disabled on this system* when you run `ng` | Run `Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned`, then open a new terminal. |
| `dotnet --version` shows 8.x or 9.x | The .NET 10 SDK is missing. Re-run the Visual Studio Installer and check the *ASP.NET and web development* workload. |
| You forgot the `postgres` password | Ask in class, and we'll sort it out together. |

Still stuck? Send me an email ilija.mitev3@gmail.com.
