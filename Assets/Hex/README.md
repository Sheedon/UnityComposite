# Sheedon Hex

`Sheedon.Hex` is a Unity-independent foundation for pointy-top hex grids. Version `0.3.0` contains Core geometry, finite map structures, graph contracts, and reusable search algorithms.

## Install locally

In Unity Package Manager, choose **Add package from disk** and select this directory's `package.json`. The package can also remain under a project's `Assets` folder while it is being developed.

## Quick start

```csharp
using Sheedon.Hex;

var origin = HexCoord.Zero;
var east = HexTopology.GetNeighbor(origin, HexDirection.E);
var distance = HexTopology.Distance(origin, new HexCoord(3, -2));

var layout = new HexLayout(1d);
var worldPlanePoint = layout.HexToPoint(east);
var coord = layout.PointToHex(worldPlanePoint);

var region = HexShapes.CreateHexagon(origin, 3);
var terrain = new HexLayer<int>(region);
terrain.Set(origin, 1);

var graph = new RegionHexGraph(region);
```

The package intentionally has no dependency on `UnityEngine`. Conversion from `HexPoint` to `Vector2` or `Vector3` belongs in the consuming Unity integration layer.

The consuming game implements `IHexTraversalRule` for each movement mode, then passes that rule together with the graph to `BreadthFirstSearch.FindPath`, `Dijkstra.FindPath`, `AStar.FindPath`, `CostRange.Find`, or `FloodFill.FindConnected`.

## Current scope

- Pointy-top orientation only.
- Axial `Q/R` coordinates with calculated cube `S`.
- Direction order is frozen as `NE/E/SE/SW/W/NW`.
- Finite maps are represented by `HexRegion` plus one or more `HexLayer<T>` instances.
- Shapes are region construction methods and do not create map subclasses.
- `IHexGraph` describes valid nodes and immediate adjacency; `RegionHexGraph` adapts a live region.
- `IHexTraversalRule` keeps game-specific passability and positive integer Cost outside the package.
- BFS, Dijkstra, A*, CostRange, and FloodFill operate through those contracts.
- A* uses Hex Distance by default and accepts an optional non-negative admissible estimate callback.
