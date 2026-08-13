# Sheedon Hex

`Sheedon.Hex` is a Unity-independent foundation for flat-top hex grids. Version `0.1.0` contains axial coordinates, six-direction topology, ranges, rings, spirals, lines, rounding, and layout conversion.

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
```

The package intentionally has no dependency on `UnityEngine`. Conversion from `HexPoint` to `Vector2` or `Vector3` belongs in the consuming Unity integration layer.

## Current scope

- Flat-top orientation only.
- Axial `Q/R` coordinates with calculated cube `S`.
- Direction order is frozen as `NE/E/SE/SW/W/NW`.
- Map storage and search algorithms are planned for later `0.x` milestones.

See [Documentation/Architecture.md](Documentation/Architecture.md) for the frozen boundaries.
