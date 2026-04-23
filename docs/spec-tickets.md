# Future Stabilization Spec Tickets

These are ready-to-file GitHub issues for stabilizing and extending Project Jammer.
Copy each block's title + body into a new issue (use the **Spec Ticket** issue template).

---

## 1 — [Spec] Network: Photon disconnect & reconnect resilience

**Labels:** `spec` `networking` `bug`

### Summary
Players who experience a network drop mid-game land on the lobby with no warning and
no way to rejoin the room they were in.

### Problem / Motivation
`GameSceneManager.OnDisconnected` immediately calls `SceneManager.LoadScene(lobbySceneName)`.
There is no grace period, no retry, and no state preservation. A brief lag spike counts
as a disconnect.

### Proposed Solution
1. Add a configurable reconnect window (e.g. 10 s) with a visible countdown UI.
2. On disconnect, attempt `PhotonNetwork.ReconnectAndRejoin()` using the cached
   `RoomInfo`.
3. On successful rejoin, re-spawn the player at a spawn point and restore
   `DataManager.playerDict` entry.
4. On failure (timeout), navigate to lobby with an informative message.

### Acceptance Criteria
- [ ] Players can survive a drop of up to 10 s without losing their room slot.
- [ ] A reconnect-in-progress overlay is shown during the attempt.
- [ ] Permanent disconnect still routes to lobby with a clear error message.
- [ ] Tested with simulated Photon disconnect in the Editor.

---

## 2 — [Spec] Combat: Fully authoritative damage model

**Labels:** `spec` `gameplay` `networking`

### Summary
All damage calculations are gated on `PhotonNetwork.IsMasterClient`.
If the master client disconnects, no player can take damage and no enemy dies for the
rest of the session.

### Problem / Motivation
- `PlayerHealth.TakeDamage` returns early for non-master clients.
- `Enemy.TakeDamage` delegates via RPC to master, which is correct, but the
  master migration path (when MC leaves) is untested and likely breaks.
- Hard to add anti-cheat or lag compensation without a central authority.

### Proposed Solution
1. Verify and harden `OnMasterClientSwitched` in `GameSceneManager` so the new
   master immediately re-subscribes all damage listeners.
2. Add a short "authority transfer" coroutine that re-initializes `EnemyManager`
   and re-registers alive players after a master switch.
3. Write an Editor test that simulates master disconnect and verifies damage still
   fires correctly from the new master.

### Acceptance Criteria
- [ ] Killing an enemy after master switch works without console errors.
- [ ] `PlayerHealth` continues to receive damage after master switch.
- [ ] Unit / integration test covers the authority transfer path.

---

## 3 — [Spec] Security: Remove hardcoded database credentials

**Labels:** `spec` `security` `data`

### Summary
`DataManager.ConnectDataBase()` contains a plain-text MySQL connection string
(server IP, uid, password, port) committed directly to source code.

### Problem / Motivation
- Credentials are visible to anyone with repository access.
- Rotating credentials requires a code change and a new build.
- Connection failure has no retry or fallback — the game continues silently broken.

### Proposed Solution
1. Create a `Resources/DatabaseConfig.asset` (ScriptableObject, git-ignored) that
   holds connection parameters.
2. Fall back to environment variables (`DB_HOST`, `DB_USER`, `DB_PASS`) when the
   asset is absent, to support CI/CD.
3. Add a `try/catch` with exponential backoff (max 3 retries) around `con.Open()`.
4. Add `.gitignore` entry for `DatabaseConfig.asset` and `DatabaseConfig.asset.meta`.
5. Provide a `DatabaseConfig.example.asset` with placeholder values that IS committed.

### Acceptance Criteria
- [ ] No credentials present in any committed file.
- [ ] Game runs without DB access (graceful degradation — leaderboard features
   disabled, not crash).
- [ ] README documents how to set up the config asset locally.

---

## 4 — [Spec] Gameplay: Player respawn system

**Labels:** `spec` `gameplay`

### Summary
Dead players are `SetActive(false)` permanently for the rest of the round with no
respawn mechanic, making early deaths frustrating and unbalanced.

### Problem / Motivation
`PlayerHealth.DeathRoutine` disables the `GameObject` after 3 s. There is no
revive or respawn path. `DataManager.DeathCount` fires `GameOver` only when
**all** players are simultaneously dead, meaning a solo survivor carrying the
team is the only win path.

### Proposed Solution
1. Add a `RespawnTimer` component with a configurable delay (default 30 s).
2. On death, start the timer and show a "Respawning in X s" overlay for the
   local player.
3. After the delay, call `PhotonNetwork.Instantiate("PlayerHolder", ...)` from
   the respawning client (master client validates).
4. Mark the `PlayerData` entry `isAlive = true` and call
   `EnemyManager.RenewalTargetPlayer`.
5. Optional: limit respawns to N per round (configurable in `GameSettings`).

### Acceptance Criteria
- [ ] Dead player re-enters the game after the configured delay.
- [ ] `EnemyManager` target list is updated after respawn.
- [ ] `GameOver` only fires when all players are dead and no pending respawns remain.
- [ ] Respawn UI shows countdown correctly on the local client.

---

## 5 — [Spec] Refactor: Standardise namespaces to match module layout

**Labels:** `spec` `refactor` `tech-debt`

### Summary
Scripts were moved to feature modules (`Assets/Scripts/...`) but still use
contributor-scoped namespaces (`Darik`, `ildoo`, `LDW`, `Park_Woo_Young`).
This creates confusion and makes the codebase hard to navigate.

### Problem / Motivation
- `using Darik;` in player code (conceptually unrelated to "Darik").
- Namespaces do not reflect module purpose (e.g. `ildoo` for player systems).
- New contributors must learn contributor aliases before contributing.

### Proposed Solution
Rename namespaces to match module paths. Suggested mapping:

| Old Namespace | New Namespace |
|---|---|
| `Darik` | `ProjectJammer.Gameplay.Enemy`, `ProjectJammer.Infrastructure`, etc. |
| `ildoo` | `ProjectJammer.Gameplay.Player`, `ProjectJammer.Gameplay.Weapons`, etc. |
| `LDW` | `ProjectJammer.UI`, `ProjectJammer.Data`, `ProjectJammer.Lobby` |
| `Park_Woo_Young` | `ProjectJammer.Gameplay.Objective` |

Use global find-and-replace, update all `using` directives, and verify compile.

### Acceptance Criteria
- [ ] No contributor-named namespaces remain in any `.cs` file.
- [ ] All `using` directives reference the new namespace.
- [ ] Unity compiles with 0 errors and 0 namespace-related warnings.
- [ ] Existing scene/prefab GUID references still resolve (no missing scripts).

---

## 6 — [Spec] Performance: Object pooling for networked enemy & item spawns

**Labels:** `spec` `performance` `networking`

### Summary
Enemies and items are spawned via `PhotonNetwork.InstantiateRoomObject` /
`PhotonNetwork.Instantiate` on every spawn interval with no pooling,
creating GC pressure and per-wave instantiation lag.

### Problem / Motivation
- Three enemy coroutines each call `PhotonNetwork.InstantiateRoomObject` on
  their timers — no reuse.
- `ItemSpawner.AttemptForRespawn` calls `PhotonNetwork.Instantiate` on every
  spawn, regardless of whether a pooled instance is available.
- Photon's own view ID pool has a hard limit; excessive instantiations risk
  running out in long sessions.

### Proposed Solution
1. Implement a Photon-aware pool: on `OnPhotonInstantiate`, place the object in a
   per-type pool rather than destroying it on death.
2. On "spawn" request, check the pool first; only call
   `PhotonNetwork.InstantiateRoomObject` when the pool is empty.
3. On "destroy", disable the object and return it to the pool rather than
   `PhotonNetwork.Destroy`.
4. Benchmark: target <1 ms spawn cost per enemy on a mid-tier mobile device.

### Acceptance Criteria
- [ ] Enemy/item spawn does not allocate a new Photon ViewID when a pooled
  instance is available.
- [ ] No observable frame spike (>2 ms) on spawn in a 4-player session.
- [ ] Pool size is configurable per prefab in `EnemyManager` / `ItemSpawner`.

---

## 7 — [Spec] Audio: Integrate AudioManager into GameManager and wire SFX

**Labels:** `spec` `audio` `tech-debt`

### Summary
`AudioManager` exists but is never registered in `GameManager`, and most game
events (gunshots, hits, footsteps) use direct `AudioSource.PlayOneShot` calls
rather than routing through a central system.

### Problem / Motivation
- `GameManager.InitManagers()` does not create an `AudioManager` child — it is
  unused infrastructure.
- Volume controls in `AudioManager` (`SetMasterVolume`, `SetBGMVolume`,
  `SetSFXVolume`) cannot be reached from the pause menu without a direct
  reference.
- Duplicate `AudioSource` components on multiple GameObjects make mixing
  and ducking impossible.

### Proposed Solution
1. Add `AudioManager` to `GameManager.InitManagers()` (alongside
   `ResourceManager`, `PoolManager`, etc.) and expose it as
   `GameManager.Audio`.
2. Replace inline `AudioSource.PlayOneShot` calls in `Gun`, `PlayerHealth`, etc.
   with `GameManager.Audio.PlayEffect(sound)`.
3. Wire the pause menu volume sliders to `GameManager.Audio.Set*Volume`.
4. Add `AudioMixer` snapshot support (e.g. muffle SFX during pause).

### Acceptance Criteria
- [ ] `GameManager.Audio` is non-null after scene load.
- [ ] Master / BGM / SFX volume sliders in the pause menu update the mixer.
- [ ] No `NullReferenceException` from audio calls during gameplay.
- [ ] Gunshot, hit, reload, and item pickup sounds play via the central system.

---

## 8 — [Spec] Data: JSON-driven enemy stat balancing (replace magic numbers)

**Labels:** `spec` `data` `gameplay`

### Summary
Enemy HP, damage, and speed are hardcoded as `[SerializeField]` floats scattered
across `Enemy.cs` and its subclasses. `DataManager` already loads `StatData.json`
into `MonsterStatDict` but it is never applied to enemy instances.

### Problem / Motivation
- Balancing requires an engineer to open scenes and tweak component values.
- `MonsterStatDict` is populated but never read after `DataInit()`.
- `JammerStatDict` exists in `DataManager` but is never populated (the JSON
  loader call for it is absent).

### Proposed Solution
1. Define a `Stat` struct that covers HP, move speed, attack damage, and
   attack range.
2. In `Enemy.Awake` (or `OnEnable`), look up the enemy's ID in
   `GameManager.Data.MonsterStatDict` and apply the stats.
3. Populate `JammerStatDict` from `JammerData.json` (file already present in
   `Resources`).
4. Add a runtime debug overlay that shows current enemy stats (editor-only).

### Acceptance Criteria
- [ ] Enemy HP/speed/damage values are sourced from `StatData.json`.
- [ ] Changing JSON values changes in-game behaviour without a code recompile.
- [ ] `JammerStatDict` is loaded and referenced by `DisruptorState`.
- [ ] `MonsterStatDict` is verified non-empty in an Editor unit test.
