# The FateBringer

<p align="center">
  <!--<img src="https://github.com/MicksS1/SideScroll-GameProg/assets/158981991/84f156fe-552a-47bd-acdc-a8668b1820b1">-->
</p>

## 🔴 About This Project
<p align="justify">This game was parts of team competing in the IGI Competition. In this project, i learn how to create a generic grid system and placing game data inside json so that i can have a generic gameplay scene.</p>

<br>

## 📋 Project Info

<b> Developed with Unity 2022 </b>

| **Role** | **Name** | **Development Time** 
| - | - | - |
| Game Designer | Jeremy Edward | 1 week |
| Game Programmer | Steven Putra A | 1 week |
| Game 2D Artist | Karen Cresentia | 1 week |

<details>
  <summary> <b>My Contribution (Game programmer)</b> </summary>
  
- Save System for progression
- Inventory System
- Enemy Behaviour
- All 8 unit (4 ally, 4 enemy)
- Turn Base Mechanic
- Generic Tilemap
  
</details>

<br>

## ♦️About Game
<p align="justify">The FateBringer is a puzzle 2D turn based strategy game. It was parts of team competing in the IGI Competition. We as a general of an army were task to defeat all of the evil lurking in the unknown island. We were given a set amount of troops and we need to find sequence of move to win fighting all of the enemy.</p>

<br>

## 🎮 Gameplay
<p align="justify">This game is a puzzle game! Do your to figure out the sequence of move to win the level. There 35 levels in this game!</p>

<br>

## ⚙️ Game Mechanics I Created
### Generic Grid System
- Logic is located within the `grid.cs` script
- the script is the base of all the grid system inside the game.
- the grid which will then be used for the tilemap and its visualizer.

### Generic Saving System
- Logic is located within the `SaveSytem.cs` script
- JSON file format is used to store essential level data such as the grid layout and the inventory system.
- using other script such as 'UnitSpawner.cs' , 'TileMap.cs' , and 'InventorySystem.cs' the JSON data will be translated to their respected needs.
- this allowed me to only 1 scene for the whole 45 level.
- the save is located in the folder 'Asset/Resources'

### Inheritance of 8 unit
- the base unit is located in 'UnitGridCombat.cs' script.
- the base unit is then used for 2 other script which is 'RobotBaseScript.cs' script and 'GiantBaseScript.cs' script.
- from the base unit, it was used to create 8 unit with different ability.
<br>

## 📜 Scripts

|  Script       | Description                                                  |
| ------------------- | ------------------------------------------------------------ |
| `GameManager.cs` | Manages the game data |
| `TilemapTesting.cs`  | Responsible for all the visual intializer in the game. |
| `GridCombatSystem.cs`  | Responsible for the turnbase and grid combat system. |
| `InventorySystem.cs`  | Manages Inventory of all the robot a player can drop to the battlefield. |
| `UnitGridCombat.cs`  | The base for all unit inside the game |
| `UnitSpawner.cs`  | Responsible for getting the data needed to spawn the unit inside a level |
| `etc`  |

<br>

## 🕹️ Controls
We use mouse for all the control which in the gameplay is used to drop and command the army we have.

<br>
