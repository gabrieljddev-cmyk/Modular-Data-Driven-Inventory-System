# Modular Data-Driven Inventory System

A modular, optimized, and data-driven inventory system built from scratch in Unity. This project was engineered with a strict focus on software architecture best practices, clean code principles, decoupling, and data persistence.

---

## 🚀 Gameplay Demonstration

*coming soon*

---

## ✨ Core Features

- **Data-Driven Architecture:** Items are treated as immutable data assets created entirely through ScriptableObjects. This allows game designers to configure new items directly in the Unity Editor without touching a single line of code.
- **Advanced Drag & Drop Mechanic:** Fluid, real-time visual translation of items across the user interface with dynamic raycast detection and slot validation.
- **Smart Item Stacking:** Automatic grouping of identical items upon pickup or dropping, respecting the maximum stack boundaries defined per item asset.
- **Safety Position Snapback:** A fail-safe mechanism that automatically snaps an item back to its original slot if it is accidentally dropped outside a valid UI container.
- **Robust State Persistence (Save/Load):** Automated serialization of the inventory's layout and data into a structured JSON format, safely written to disco via Application.persistentDataPath.
- **Decoupled Event-Driven Communication:** Complete separation between backend data logic and the frontend presentation layer (UI) using C# Actions / UnityEvents, eliminating rigid dependencies and reducing the risk of null reference exceptions.

---

## 🛠️ Tech Stack & Patterns Applied

- **Engine:** Unity 6.3 LTS
- **Language:** C#
- **Architectural Patterns & Mechanics:**
  - **ScriptableObjects** acting as the item database.
  - **DTO (Data Transfer Object)** pattern utilized to clean and prepare data models for lightweight disk serialization.
  - **Unity's JsonUtility** for fast and memory-efficient parsing.
  - **SOLID Principles** applied specifically to achieve high cohesion and low coupling between components.
