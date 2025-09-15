# Devoid Engine

Devoid Engine is a custom-built game engine developed as a personal learning project to explore modern game engine architecture, rendering systems, and editor integration. It is written in C# and powered by DevoidGPU, a Render Hardware Interface (RHI) that supports multiple graphics backends. While not intended for production use, the engine implements many modern features and provides a foundation for experimenting with engine design and rendering techniques.

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