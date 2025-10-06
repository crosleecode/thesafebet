# thesafebet
This project is being done as a Senior Capstone assignment at Metro State University - Fall 2025

🎰 SafeBet Casino Web Application

A comprehensive online casino web application featuring multiple casino games built with ASP.NET Core MVC.

 🎮 Available Games
- **Blackjack** - Classic card game with dealer AI
- **Roulette** - American-style roulette with comprehensive betting options

 🛠️ Technical Stack

# **Roulette Game Implementation**
- **Framework**: ASP.NET Core 8.0 MVC
- **Backend Language**: C# (.NET 8.0)
- **Frontend**: Razor Views (CSHTML) with server-side rendering
- **Styling**: CSS3 with Bootstrap 5 integration
- **Client-side Scripting**: Minimal JavaScript for UI enhancements only
- **State Management**: Server-side with static game state
- **Form Handling**: Traditional MVC form submissions (POST requests)
- **Data Models**: 
  - `RouletteGame` - Main game state management
  - `Bet` - Individual bet tracking
  - `GameHistory` - Game history records
  - `RouletteViewModel` - View model for data binding
- **Controller**: `RouletteController` with actions for:
  - `Index()` - Display game interface
  - `PlaceBet()` - Handle bet placement
  - `Spin()` - Execute game spin and calculate results
  - `ClearBets()` - Clear all current bets
  - `ToggleOdds()` - Toggle odds display
- **Features**:
  - Real-time winning result highlighting with golden pulsing animation
  - Comprehensive betting table (straight up, red/black, even/odd, dozens, columns)
  - Game history tracking
  - Responsive design for mobile and desktop
  - Proper MVC pattern adherence with server-side logic

 **General Application Stack**
- **Framework**: ASP.NET Core 8.0
- **Architecture**: Model-View-Controller (MVC)
- **Database**: In-memory state management
- **UI Framework**: Bootstrap 5
- **Version Control**: Git with GitHub
- **Development Environment**: Visual Studio / VS Code compatible

 🚀 Getting Started
1. Clone the repository
2. Ensure .NET 8.0 SDK is installed
3. Run `dotnet restore` to restore packages
4. Run `dotnet run` to start the application
5. Navigate to `http://localhost:5236`

 📁 Project Structure
```
SafeBet/
├── Controllers/
│   ├── BlackjackController.cs
│   └── RouletteController.cs
├── Models/
│   └── RouletteGame.cs
├── Views/
│   ├── Home/
│   ├── Blackjack/
│   └── Roulette/
│       └── Index.cshtml
└── wwwroot/
    └── css/
```
