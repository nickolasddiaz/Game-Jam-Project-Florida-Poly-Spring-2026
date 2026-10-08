# Game Jam Project Florida Poly Spring 2026

## Tool Used

Godot using C#
AsepriteWizard: Asesprite is needed to import assets



## Diagram of the Game

```mermaid
graph TD;
    A["Instructions/credits screen"] <--> B["Start Game Screen"]
    B --> C["Game Screen Manager"]
    C --> D["Intro For the Story"]
    D --> E["Basic Tutorial (Text)"]
    E --> F["Show the next task for the day"]
    F --> G["Edit Screen for the map"]
    G --> H["Gameplay"]
    H --> I["End of Day"]
    I --> F
    I --> K["End of Game"]
    K --> B
```

## Team

- Alexander Ramirez
- Mateus Ramos
- Nicolas Diaz
- Nickolas Diaz
