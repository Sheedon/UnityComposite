# Architecture

## Frozen version-one decisions

- The library provides mechanisms; consuming game modules provide terrain, movement, and gameplay rules.
- Only flat-top hexes are supported.
- Axial `Q/R` is canonical and cube `S` is calculated.
- Direction values are frozen as `NE = 0`, `E = 1`, `SE = 2`, `SW = 3`, `W = 4`, and `NW = 5`.
- Runtime code is one `Sheedon.Hex` assembly with logical `Core`, `Map`, and `Algorithms` areas.
- Tests are one `Sheedon.Hex.Tests` assembly.
- Runtime code must not reference `UnityEngine`.
- Finite-map membership and traversability are separate concepts.
- Costs are positive integers whose business unit belongs to consumers.

## Dependency direction

`Map` and `Algorithms` may depend on `Core`. Algorithms must operate through graph and traversal contracts rather than concrete map types.

## Explicitly outside the package

Terrain, elevation, resources, ownership, rendering, meshes, shaders, GameObjects, picking, selection, chunk streaming, map saves, asynchronous scheduling, and game-specific movement rules are not responsibilities of this package.

## Milestones

- `0.1.0`: Core.
- `0.2.0`: finite regions, shapes, and generic layers.
- `0.3.0`: graph contracts and search algorithms.
- `0.4.0`: complete tests and public API review.
- `0.5.0`: validation through the first real `Dao.Map` consumer.
