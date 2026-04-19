# Project Jammer

A co-op multiplayer first-person shooter built in **Unity 2021.3.24f1** using **Photon PUN 2** for real-time networking.  
Players must protect a Disruptor device from waves of AI enemies, activating it to reach 100 % progress while staying alive.

---

## Table of Contents

- [Game Overview](#game-overview)
- [Requirements](#requirements)
- [Setup & Installation](#setup--installation)
- [Scene Flow](#scene-flow)
- [Controls](#controls)
- [Architecture Overview](#architecture-overview)
- [Folder Map](#folder-map)
- [Key Systems](#key-systems)
- [Troubleshooting](#troubleshooting)
- [Known Limitations](#known-limitations)

---

## Game Overview

| Detail | Value |
|---|---|
| Genre | Co-op Multiplayer FPS |
| Engine | Unity 2021.3.24f1 |
| Network | Photon PUN 2 |
| Players | 1–8 per room |
| Objective | Protect and charge the **Disruptor** to 100 % before all players die |

### Win / Loss Conditions

- **Win** – Disruptor progress reaches 100 %. A final EMP explosion clears remaining enemies.
- **Lose** – All players are simultaneously dead while the Disruptor is still active.

---

## Requirements

### Unity

- Unity **2021.3.24f1** (LTS) — use [Unity Hub](https://unity.com/download) to install this version.

### Unity Packages (auto-installed via Package Manager)

| Package | Version |
|---|---|
| Universal Render Pipeline (URP) | 12.1.11 |
| Input System | 1.5.1 |
| TextMeshPro | 3.0.6 |
| Animation Rigging | 1.1.1 |
| ProBuilder | 5.0.7 |
| Visual Scripting | 1.8.0 |
| Timeline | 1.6.4 |

### Photon PUN 2 (must be imported separately)

1. Download **PUN 2 – Free** from the Unity Asset Store.
2. Import into the project.
3. In **Window → Photon Unity Networking → PUN Wizard**, enter your **App ID** from [Photon Dashboard](https://dashboard.photonengine.com/).

### Backend (optional for leaderboards / auth)

- A MySQL server (tested with `MySQL 8`) is required for player authentication and data persistence.
- Connection is configured inside `DataManager.cs` — see [Known Limitations](#known-limitations) for hardcoding notes.

---

## Setup & Installation

```bash
# 1. Clone the repository
git clone https://github.com/iopark/ProjectJammer.git

# 2. Open Unity Hub → Add project from disk → select the cloned folder
# 3. Unity will import assets and packages automatically (takes several minutes on first open)
# 4. Import Photon PUN 2 from the Asset Store and enter your App ID
```

**First Run Checklist**

- [ ] Photon App ID entered (`Resources/PhotonServerSettings`)
- [ ] MySQL server running and credentials updated in `DataManager.ConnectDataBase()`
- [ ] At least one enemy/player prefab exists under `Resources/` (prefab names must match those referenced in code)
- [ ] Open the Lobby scene as the starting scene

---

## Scene Flow

```
LobbyScene (login / menu / room browser)
        │  PhotonNetwork.LoadLevel("GameScene")
        ▼
GameScene (gameplay)
        │  GameSceneManager.OnLeftRoom / OnDisconnected
        ▼
LobbyScene (return after game over / victory / disconnect)
```

### Scene List

| Scene File | Purpose | Notes |
|---|---|---|
| `LobbyScene2` | Production lobby | Login, menu, room creation & joining |
| `LobbySceneLDW` | Alternate lobby build | Earlier iteration |
| `GameScene` | Main gameplay | Disruptor defense, enemy waves |
| `PlayTestSceneVer1/2` | Internal playtesting | Skips lobby; connects directly via debug mode |

**Entry scene for playtesting**: Open `PlayTestSceneVer1` or `PlayTestSceneVer2` — the game will auto-connect using a randomised nickname and join/create a `DebugRoom`.

---

## Controls

| Action | Key / Input |
|---|---|
| Move | WASD |
| Look | Mouse |
| Jump | Space |
| Sprint | Left Shift |
| Fire | Left Mouse Button (hold for auto) |
| Zoom / ADS | Right Mouse Button |
| Reload | R |
| Interact (repair Disruptor) | F |
| Melee | Left Mouse Button (close range) |
| Pause / Cursor toggle | Escape |

---

## Architecture Overview

The project uses a **singleton GameManager** that bootstraps and owns all sub-managers at runtime via `GameSettings.Init()` (called before any scene load through `RuntimeInitializeOnLoadMethod`).

```
GameManager (DontDestroyOnLoad singleton)
├── ResourceManager   — cached Resources.Load + pooled Instantiate/Destroy
├── PoolManager       — Unity ObjectPool<GameObject> keyed by prefab name
├── DataManager       — runtime state (player dict, disruptor progress, MySQL)
├── UIManager         — canvas hierarchy, PopUpUI stack
└── EnemyManager      — enemy spawn coroutines, target selection
```

All managers are accessed through static properties on `GameManager` (e.g. `GameManager.Data`, `GameManager.UI`, `GameManager.Enemy`).

---

## Folder Map

```
Assets/
├── Collaborators/
│   ├── Darik/          — Enemy AI, managers (GameManager, ResourceManager, PoolManager,
│   │                     EnemyManager, GameSceneManager), state machine, hit effect
│   ├── IlDoo/          — Player (movement, shooting, health, melee, interaction),
│   │                     weapons (Gun, GunData), items, audio, FPS camera
│   ├── LDW/            — UI system (UIManager, PopUpUI stack, HUD widgets),
│   │                     Data layer (DataManager, PlayerData, JSON loaders),
│   │                     Lobby (LobbyManager, panels, Photon callbacks)
│   └── Park_Woo_Young/ — Objective mechanics (DisruptorState, Emp, MedicalRoom)
├── Configs/            — GameSettings (bootstrap), CustomProperty (editor attributes)
├── Scenes/             — .unity scene files
└── PolygonApocalypse/  — Third-party art asset pack
```

> **Refactor target**: The collaborator-named folders are scheduled to be reorganised into feature modules.  
> See [`MIGRATION.md`](MIGRATION.md) for the full old → new path mapping.

---

## Key Systems

### Disruptor (Objective)
`DisruptorState` increments `DataManager.DisruptorProgress` once per second while active.  
Enemies target the Disruptor when it is active; they switch to players after it is knocked offline.  
Players interact (`F`) to repair it.  Progress at 100 % triggers `DataManager.GameClear()`.

### Enemy Spawning
`EnemyManager` runs three independent coroutines (`Blade`, `Rifle`, `Sniper`) with configurable cooldown timers.  
Enemies are spawned as Photon room objects; only the master client spawns new enemies.

### Player Death & Game Over
`PlayerHealth` broadcasts death via PunRPC. `DataManager.DeathCount()` checks whether all registered players are dead, and fires `GameOver` event if so.

### Lobby & Networking
`LobbyManager` (Photon callbacks) drives a panel-based state machine (Login → Menu → Lobby → Room).  
`GameSceneManager` handles scene-side Photon events: player load synchronisation, countdown timer, and enemy/item generation on game start.

### Object Pool
`PoolManager` provides `Get`/`Release` for GameObjects and UI canvases.  
`ResourceManager.Instantiate` / `ResourceManager.Destroy` are the single entry points — they transparently route through the pool when `pooling: true`.

---

## Troubleshooting

| Problem | Likely Cause | Fix |
|---|---|---|
| Scripts show missing references in Inspector | Meta file GUID mismatch after clone | Reimport all (`Assets → Reimport All`) |
| "Failed to connect to Photon" | No App ID / incorrect region | Enter App ID in `PhotonServerSettings` |
| "DataBase connect" errors in console | MySQL server offline or wrong credentials | Update `DataManager.ConnectDataBase()` with correct server info, or comment out the call in `Start()` |
| Enemies do not spawn | Not master client, or `enemySpawnPoints` list empty | Ensure at least one spawn point Transform is wired up in `GameSceneManager` |
| Players spawn at origin | `GameManager.Data.Disruptor` is null | Confirm `DisruptorState.GameStart()` is called and Disruptor prefab is in scene |
| Game starts in debug mode unexpectedly | Launched from `PlayTestScene` directly | Use `LobbyScene2` as the build entry scene |

---

## Known Limitations

1. **Hardcoded DB credentials** — `DataManager.cs` contains a plain-text MySQL connection string (server IP, user, password). This should be moved to a build configuration asset or environment variable.
2. **No player respawn** — Dead players are deactivated for the rest of the round with no respawn flow.
3. **Master-client only damage authority** — All `TakeDamage` calculations are gated on `PhotonNetwork.IsMasterClient`; if the master client disconnects, damage stops being processed.
4. **No disconnect / reconnect handling** — A network drop during gameplay routes back to the lobby scene without preserving game state.
5. **Collaborator-scoped namespaces** — `Darik`, `ildoo`, `LDW`, `Park_Woo_Young` namespaces exist; a unified namespace convention is part of the planned refactor.
6. **Single map** — Only `GameScene` is production-ready; level variety is not yet implemented.
