# 🎮 Last Wave — Top-Down Survivor Shooter

> Inspired by ``Survivor.io`` and arcade bullet-heaven games, this Unity project captures the tension of endless waves.  
> Survive, level up, and build powerful synergies to push through the final wave!

---

## 🧩 Features

- Endless waves with escalating enemy modifiers  
- Auto-aim and auto-fire weapons, just focus on positioning  
- Randomized upgrade pool built on **ScriptableObjects**  
- Temporary boosts: health orbs, regeneration zones, and damage bursts  
- Full menu flow: splash, hub, loading, pause, and defeat screen  

---

## 🎮 Controls

| Action | Keys |
|--------|------|
| Move | WASD |
| Shoot | Automatic targeting |
| Pause | ESC |

---

## 🛠️ Tech Stack

- Unity 6 6000.0+ (URP 17)  
- C#  
- Unity Input System 1.14  
- ScriptableObjects for items and waves  
- Object Pooling for enemies and projectiles  
- Unity Timeline 
- TextMeshPro UI

---

## 🧠 Architecture Overview

- **GameManager** – coordinates states, pacing, and reward cadence  
- **WaveManager** – schedules enemy batches and bosses  
- **Player_Movement** – directional input, health system, and regeneration  
- **Player_attack** – auto-aim targeting and weapon spawning  
- **Weapon** – projectile behaviour, damage, and collisions  
- **Enemy** – finite-state AI, damage callbacks, and loot drops  
- **DamageNumberManager** / **HealNumberManager** – floating combat text feedback  
- **UpgradeCanvas** – draft-and-pick upgrade flow after each wave  

---

## 📸 Screenshots

<img width="2559" height="1439" alt="Arena" src="https://github.com/user-attachments/assets/a598051f-cd18-45b2-a1ca-5dea3f56c7bd" />
<img width="1915" height="1076" alt="Upgrade Screen" src="https://github.com/user-attachments/assets/2ae216a5-2a4b-4c10-9dbc-7c7f090fb217" />
<img width="2559" height="1439" alt="Wave Combat" src="https://github.com/user-attachments/assets/4d63e011-53fb-4525-a9c6-99d228b949f4" />
<img width="2559" height="1439" alt="Ability Showcase" src="https://github.com/user-attachments/assets/8cb8df32-7ecf-4050-b270-3becf4d10811" />

---

## 📦 Links

- 🎮 [Play on itch.io](https://aerunstudio.itch.io/last-wave)  
- 📹 [Gameplay Video on YouTube](https://youtu.be/i4TX6WOEg3A)

---

## 📜 License

MIT License — free to use with credit.
