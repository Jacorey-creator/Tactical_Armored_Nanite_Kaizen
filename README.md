# 🛡️ Tank Game

A modular Unity tank game featuring customizable tank movement, firing strategies, and player score tracking. Built for scalability and clean architecture using ScriptableObjects, interfaces, and singletons.

---

## 🚀 Features

- 🔫 **Strategy-based Tank Behavior**  
  Easily swap movement and firing strategies using interfaces.

- 🎯 **Customizable Projectile System**  
  ScriptableObjects define projectile stats like speed, damage, lifetime, and explosion radius.

- 📊 **Player Score Tracking**  
  Track kills, deaths, and waves defeated using a non-MonoBehaviour `ScoreManager`.

- 🧠 **Central GameManager**  
  Handles game state transitions, player spawning, and delegates score logic.

---

## 📁 Project Structure

Assets/ ├── Scripts/ │ ├── GameManager/ │ │ └── GameManager.cs │ ├── Player/ │ │ └── TankController.cs │ ├── Strategies/ │ │ ├── IFiringStrategy.cs │ │ ├── ITankMovementStrategy.cs │ │ └── SingleShotStrategy.cs │ ├── Projectiles/ │ │ ├── Projectile.cs │ │ └── ProjectileData.cs │ └── Score/ │ ├── ScoreManager.cs │ └── PlayerScore.cs

yaml
Copy
Edit

---

## 🧩 Architecture Overview

- `GameManager`: Manages state, spawns player, and delegates score registration.
- `ScoreManager`: Static singleton. Tracks stats for all players.
- `TankController`: Modular controller using pluggable movement/firing logic.
- `Projectile`: Moves forward, applies damage on collision, and auto-destroys after a lifetime.

---

## 🧪 Example Usage

```csharp
// Register a player
ScoreManager.Instance.RegisterPlayer(1);

// Add a kill
ScoreManager.Instance.AddKill(1);

// Get the score
PlayerScore score = ScoreManager.Instance.GetScore(1);
Debug.Log($"Kills: {score.kills}, Deaths: {score.deaths}");

✅ Requirements
Unity 2021+ (or Unity 6 recommended)

No third-party packages required

🧠 To Do
 Add multiplayer support

 UI scoreboard display

 New movement/firing strategies

 Power-ups and modifiers

👨‍💻 Author
Jacorey A. Rowe (Sleepy Apple)

📄 License
This project is licensed under the MIT License — feel free to use and modify.

yaml
Copy
Edit

---

Let me know if you want a Markdown badge section or a stylized banner too!



