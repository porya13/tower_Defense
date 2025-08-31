# QUT Tower Defense - Unity Game Project

## Project Overview
This is a **Tower Defense** game developed in Unity 2022.3.53f1, created as part of the QUT (Queensland University of Technology) course curriculum. The game follows classic tower defense mechanics where players must strategically place defensive towers to prevent enemy waves from reaching and destroying their castle.

## Core Game Mechanics
- **Enemy System**: Multiple enemy types (Basic, Fast) with health points and waypoint-based navigation using Unity's NavMesh system
- **Tower Defense**: Strategic placement of defensive structures to eliminate enemies before they reach the castle
- **Castle Protection**: Players lose health points when enemies successfully reach the castle
- **Wave Management**: Enemies spawn from portals and follow predetermined paths

## Technical Architecture
- **Unity Version**: 2022.3.53f1 LTS
- **Scripting**: C# with object-oriented design patterns
- **AI Navigation**: Unity NavMesh for enemy pathfinding
- **Modular Design**: Separated systems for UI, Tower management, Build system, Level system, and Tile system
- **Interface Implementation**: Uses interfaces like IDamagable for consistent damage handling

## Project Structure
- **Scripts/**: Core game logic organized into logical modules
  - Managers/: Game state and system management
  - Tower/: Tower behavior and mechanics
  - Enemy/: Enemy AI and behavior
  - UI/: User interface components
  - BuildSystem/: Tower placement and construction
  - LevelSystem/: Level progression and management
  - TileSystem/: Grid-based tile management

## Target Platform
- **Primary**: Windows (Standalone)
- **Resolution**: 1024x768 default, supports various screen orientations
- **Graphics**: Uses Unity's URP (Universal Render Pipeline) with advanced shader support

## Development Status
This appears to be a complete, playable tower defense game with all core systems implemented, including enemy AI, tower mechanics, level progression, and user interface. The project demonstrates solid Unity development practices and game design principles suitable for educational purposes.
