# Devoid Engine

Devoid Engine is a custom-built game engine designed as a personal learning project to better understand modern game engine architecture and rendering systems. It primarily uses OpenGL for rendering and is written in C# using OpenTK. While it is not intended for production use, it includes many modern engine features and supports editor integration.

## Features



## Renderer Features



## Screenshots


## Installation
You need .NET 8.0 to run this project.
Currently, to work with the editor you will **require** a editor project to launch via the engine starter project.

## Usage

### Creating a GameObject

```
GameObject gameObject = this.gameObject.scene.addGameObject(); // Optionally specify a name or set gameObject.Name
```

### Adding components

```
gameObject.AddComponent<MeshRenderer>();
// or
MeshRenderer meshRenderer = new MeshRenderer();
gameObject.AddComponent(meshRenderer);
```


## Contributing
👋 Thanks for your interest! However, this project isn't accepting outside contributions. Feel free to fork it or open issues for feedback.

## License

GPL-3.0 License. See [LICENSE](LICENSE) for more info.