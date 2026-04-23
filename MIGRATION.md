# Migration: Collaborator Folders → Feature Modules

This document records every script path change made during the
`refactor(architecture): move collaborator-based scripts to feature modules` refactor.

Unity script references (scene/prefab GUIDs) are **not affected** because each `.cs.meta`
file was moved alongside its `.cs` file — the GUID inside the meta file stays identical.

> **If you see "Missing Script" warnings after pulling:** Open Unity and run
> **Assets → Reimport All** to let Unity re-scan all `.meta` files.

---

## Path Map

| Old Path (relative to `Assets/`) | New Path (relative to `Assets/Scripts/`) |
|---|---|
| `Collaborators/Darik/Scripts/Interface/IHittable.cs` | `Core/Interfaces/IHittable.cs` |
| `Collaborators/Park_Woo_Young/Scripts/Inteface/IInteractable.cs` | `Core/Interfaces/IInteractable.cs` |
| `Collaborators/IlDoo/Scripts/Items/Consumable.cs` | `Core/Interfaces/IConsumable.cs` |
| `Collaborators/Darik/Scripts/StateMachine/StateBase.cs` | `Core/StateMachine/StateBase.cs` |
| `Collaborators/Darik/Scripts/StateMachine/StateMachine.cs` | `Core/StateMachine/StateMachine.cs` |
| `Collaborators/IlDoo/Extensions/Extension.cs` | `Core/Extensions/Extension.cs` |
| `Collaborators/Darik/Scripts/Managers/ResourceManager.cs` | `Infrastructure/ResourceManager.cs` |
| `Collaborators/Darik/Scripts/Managers/PoolManager.cs` | `Infrastructure/PoolManager.cs` |
| `Configs/GameSettings.cs` | `Infrastructure/GameSettings.cs` |
| `Configs/CustomProperty.cs` | `Infrastructure/CustomProperty.cs` |
| `Collaborators/Darik/Scripts/Managers/GameManager.cs` | `Managers/GameManager.cs` |
| `Collaborators/LDW/Managers/UIManager.cs` | `Managers/UIManager.cs` |
| `Collaborators/IlDoo/Scripts/Player.cs` | `Gameplay/Player/Player.cs` |
| `Collaborators/IlDoo/Scripts/PlayerMover.cs` | `Gameplay/Player/PlayerMover.cs` |
| `Collaborators/IlDoo/Scripts/PlayerShooter.cs` | `Gameplay/Player/PlayerShooter.cs` |
| `Collaborators/IlDoo/Scripts/PlayerHealth.cs` | `Gameplay/Player/PlayerHealth.cs` |
| `Collaborators/IlDoo/Scripts/PlayerInteractor.cs` | `Gameplay/Player/PlayerInteractor.cs` |
| `Collaborators/IlDoo/Scripts/PlayerDeathCam.cs` | `Gameplay/Player/PlayerDeathCam.cs` |
| `Collaborators/IlDoo/Scripts/PlayerMelee.cs` | `Gameplay/Player/PlayerMelee.cs` |
| `Collaborators/IlDoo/Scripts/FPSCameraController.cs` | `Gameplay/Player/FPSCameraController.cs` |
| `Collaborators/Darik/Scripts/Enemies/Enemy.cs` | `Gameplay/Enemy/Enemy.cs` |
| `Collaborators/Darik/Scripts/Enemies/Enemy_Rifle.cs` | `Gameplay/Enemy/Enemy_Rifle.cs` |
| `Collaborators/Darik/Scripts/Enemies/Enemy_Blade.cs` | `Gameplay/Enemy/Enemy_Blade.cs` |
| `Collaborators/Darik/Scripts/Enemies/Enemy_Sniper.cs` | `Gameplay/Enemy/Enemy_Sniper.cs` |
| `Collaborators/Darik/Scripts/EnemyBullets/EnemyBullet.cs` | `Gameplay/Enemy/EnemyBullet.cs` |
| `Collaborators/Darik/Scripts/HitEffect.cs` | `Gameplay/Enemy/HitEffect.cs` |
| `Collaborators/Darik/Scripts/Managers/EnemyManager.cs` | `Gameplay/Enemy/EnemyManager.cs` |
| `Collaborators/IlDoo/Scripts/Gun.cs` | `Gameplay/Weapons/Gun.cs` |
| `Collaborators/IlDoo/Scripts/GunData.cs` | `Gameplay/Weapons/GunData.cs` |
| `Collaborators/IlDoo/Scripts/GunViewOverlay.cs` | `Gameplay/Weapons/GunViewOverlay.cs` |
| `Collaborators/IlDoo/Scripts/Items/Item.cs` | `Gameplay/Items/Item.cs` |
| `Collaborators/IlDoo/Scripts/Items/HealthPack.cs` | `Gameplay/Items/HealthPack.cs` |
| `Collaborators/IlDoo/Scripts/Items/Ammunition.cs` | `Gameplay/Items/Ammunition.cs` |
| `Collaborators/IlDoo/Scripts/Items/ItemSpawner.cs` | `Gameplay/Items/ItemSpawner.cs` |
| `Collaborators/Park_Woo_Young/Scripts/DisruptorState.cs` | `Gameplay/Objective/DisruptorState.cs` |
| `Collaborators/Park_Woo_Young/Scripts/Emp.cs` | `Gameplay/Objective/Emp.cs` |
| `Collaborators/Park_Woo_Young/Scripts/MedicalRoom.cs` | `Gameplay/Objective/MedicalRoom.cs` |
| `Collaborators/Darik/Scripts/Managers/GameSceneManager.cs` | `Networking/GameSceneManager.cs` |
| `Collaborators/LDW/UI/Scripts/BaseUI.cs` | `UI/Base/BaseUI.cs` |
| `Collaborators/LDW/UI/Scripts/SceneUI.cs` | `UI/Base/SceneUI.cs` |
| `Collaborators/LDW/UI/Scripts/PopUpUI.cs` | `UI/Base/PopUpUI.cs` |
| `Collaborators/LDW/UI/Scripts/WeaponStat.cs` | `UI/HUD/WeaponStat.cs` |
| `Collaborators/LDW/UI/Scripts/PlayerStatus.cs` | `UI/HUD/PlayerStatus.cs` |
| `Collaborators/LDW/UI/Scripts/TeamStat.cs` | `UI/HUD/TeamStat.cs` |
| `Collaborators/LDW/UI/Scripts/Crosshair.cs` | `UI/HUD/Crosshair.cs` |
| `Collaborators/LDW/UI/Scripts/Progress.cs` | `UI/HUD/Progress.cs` |
| `Collaborators/LDW/UI/Scripts/PlayerGameSceneUI.cs` | `UI/HUD/PlayerGameSceneUI.cs` |
| `Collaborators/IlDoo/Scripts/TeamHealthSync/TeamHealth.cs` | `UI/HUD/TeamHealth.cs` |
| `Collaborators/LDW/UI/Scripts/GameOver.cs` | `UI/Popups/GameOver.cs` |
| `Collaborators/LDW/UI/Scripts/PausePopUpUI.cs` | `UI/Popups/PausePopUpUI.cs` |
| `Collaborators/LDW/Lobby/Scripts/LobbyManager.cs` | `Lobby/LobbyManager.cs` |
| `Collaborators/LDW/Lobby/Scripts/LobbyPanel.cs` | `Lobby/LobbyPanel.cs` |
| `Collaborators/LDW/Lobby/Scripts/RoomPanel.cs` | `Lobby/RoomPanel.cs` |
| `Collaborators/LDW/Lobby/Scripts/LoginPanel.cs` | `Lobby/LoginPanel.cs` |
| `Collaborators/LDW/Lobby/Scripts/MenuPanel.cs` | `Lobby/MenuPanel.cs` |
| `Collaborators/LDW/Lobby/Scripts/RegisterPanel.cs` | `Lobby/RegisterPanel.cs` |
| `Collaborators/LDW/Lobby/Scripts/RoomEntry.cs` | `Lobby/RoomEntry.cs` |
| `Collaborators/LDW/Lobby/Scripts/PlayerEntry.cs` | `Lobby/PlayerEntry.cs` |
| `Collaborators/LDW/Lobby/Scripts/StatePanel.cs` | `Lobby/StatePanel.cs` |
| `Collaborators/LDW/Lobby/Scripts/LobbyTest.cs` | `Lobby/LobbyTest.cs` |
| `Collaborators/LDW/Lobby/Scripts/LobbyCameraMove.cs` | `Lobby/LobbyCameraMove.cs` |
| `Collaborators/LDW/Managers/DataManager.cs` | `Data/DataManager.cs` |
| `Collaborators/LDW/Data/Scripts/PlayerData.cs` | `Data/PlayerData.cs` |
| `Collaborators/LDW/Data/Scripts/DataContents.cs` | `Data/DataContents.cs` |
| `Collaborators/LDW/Data/Scripts/DataContents2.cs` | `Data/DataContents2.cs` |
| `Collaborators/LDW/Data/Interfaces/ILoader.cs` | `Data/ILoader.cs` |
| `Collaborators/IlDoo/Scripts/Audio/AudioManager.cs` | `Audio/AudioManager.cs` |
| `Collaborators/IlDoo/Scripts/Audio/AudioHelper.cs` | `Audio/AudioHelper.cs` |

---

## New Module Layout

```
Assets/Scripts/
├── Audio/              AudioManager, AudioHelper, Sound struct
├── Core/
│   ├── Extensions/     LayerMask extension, GameObject helpers
│   ├── Interfaces/     IHittable, IInteractable, IConsumable
│   └── StateMachine/   Generic StateMachine<TState,TOwner>, StateBase
├── Data/               DataManager, PlayerData, DataContents*, ILoader
├── Gameplay/
│   ├── Enemy/          Enemy (base + variants), EnemyManager, EnemyBullet, HitEffect
│   ├── Items/          Item, HealthPack, Ammunition, ItemSpawner
│   ├── Objective/      DisruptorState, Emp, MedicalRoom
│   ├── Player/         Player, PlayerMover, PlayerShooter, PlayerHealth,
│   │                   PlayerInteractor, PlayerDeathCam, PlayerMelee, FPSCameraController
│   └── Weapons/        Gun, GunData, GunViewOverlay
├── Infrastructure/     ResourceManager, PoolManager, GameSettings, CustomProperty
├── Lobby/              LobbyManager + all lobby panels and UI entries
├── Managers/           GameManager (singleton bootstrap), UIManager
├── Networking/         GameSceneManager (Photon scene callbacks)
└── UI/
    ├── Base/           BaseUI, SceneUI, PopUpUI
    ├── HUD/            WeaponStat, PlayerStatus, TeamStat, Crosshair,
    │                   Progress, PlayerGameSceneUI, TeamHealth
    └── Popups/         GameOver, PausePopUpUI
```

---

## Notes

- **GUIDs preserved** — every `.cs.meta` file was moved with its script; no
  scene or prefab references are broken at the GUID level.
- **Namespaces unchanged** — existing namespaces (`Darik`, `ildoo`, `LDW`,
  `Park_Woo_Young`) are left intact to avoid cascading compilation errors.
  A follow-up ticket ([see issues](https://github.com/iopark/ProjectJammer/issues))
  tracks standardising namespaces to match the new module names.
- **Non-script assets** (prefabs, textures, animations under `Collaborators/`)
  remain in their original locations and are not affected by this refactor.
