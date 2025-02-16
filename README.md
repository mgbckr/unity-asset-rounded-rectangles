# Rounded Rectangles

The "Rounded Rectangles" asset allows the crafting of rounded rectangles using shader graphs solely using built-in nodes. This makes it compatible with PolySpatial and Apple Vision Pro.

## Description

The "Rounded Rectangles" asset allows the crafting of rounded rectangles using shader graphs solely using built-in nodes. This makes it compatible with PolySpatial and Apple Vision Pro.

Rounded Rectangles supports

- individual corner radii
- borders with individual border sizes
- padding
- color and texture

## Quickstart

There are a couple of examples in the demo scene (`Rounded Rectangles/Samples/Demo Scene`). If you want to start using `Rounded Rectangles` yourself, follow the following steps.

- Create a new material from the `Rounded Rectangle` shader graph (right-click -> Create -> Material).
- For using the material with a `Canvas`:
    - Create a `RawImage` (within a `Canvas`).
    - Set the `Render Mode` to `World Space`.
    - Assign the new material to the `Material` property of the `RawImage`. Make sure to set some default texture or you might get some errors.
    - Color and texture can be set via the corresponding properties (`Color` and `Texture`) of the `RawImage`.
    - Consider setting up a `Canvas Order Coordinator` for an ordering issue where `Canvas`es don't draw in `World Space` order but rely on the `Hierarchy`. See the "Usage" section for details.
- For using the material with `Plane`:
    - Create a `Plane`.
    - Assign the new material to the `Material` property of the `Plane`.
    - Since a `Plane` does not let you set texture and color, add the `Set Content` script to the `Plane` and set texture or color as you like.
- The border color can be set via the Shader's' `Surface Inputs`. Additionally you can switch between using the texture or the color as main content.

## Usage

### Canvas Order Coordinator

There is an ordering issue where `Canvas`es don't draw in `World Space` order but rely on the `Hierarchy`.
To fix this, we can update the `Order in Layer` for each `Canvas` on `Update` for every frame.
For this, add the `Canvas Order Coordinator` to any `GameObject` and set it up as you need.

- `Find Canvases Every N Frames`: Find all `Canvas`es on `Update` at every `N` frame. This is the most convenient because you don't have to do anything even if you programmatically add `Canvas`es at runtime. However it may be inefficient and cause performance issues.

- `Find Canvases on Start` finds all `Canvas`es on `Start` once.

- `Update Sorting Order Every N Frames`: Update sorting orders every `N` frames. `0` means no automatic updates.

- `Update Sorting Order On Start`: Update sorting order on start.

- You can update the `CanvasOrderCoordinator.canvases` to keep `Canvas`es up to date by hand or run `CanvasOrderCoordinator.FindCanvases` for a general reset.

- Finally you can call `CanvasOrderCoordinator.UpdateSortingOrder` to set a sorting order for a limited set of `Canvas`es.


## Technical details

For usage instructions, see included `README.md`.

**Notes**:

- The "Rounded Rectangles" asset is built solely using built-in shader graph nodes. This makes it compatible with PolySpatial and Apple Vision Pro.
- While there are a couple of implementations out there that make beautiful rounded rectangles, none of them worked for me on PolySpatial for the Apple Vision Pro. That's why I created these shader graphs which solely use the corresponding shader nodes. No scripting is needed.
- Currently tested for `RawImage` and `Plane` objects.

**Known issues**:

- **Canvas drawing order**: Apparently PolySpatial draws elements of `Canvas`es according to their order in the `Hierarchy` if they are on the same `Layer` and have the same `Order in Layer` even if the `Canvas`es are placed in front of each other in `World Space`. One solution for this is to come up with a script that dynamically updates the `Order in Layer`, the other is using `Plane`s as outlined above.

**Resources**:

- Source: https://github.com/mgbckr/unity-asset-rounded-rectangles
- Issue tracker: https://github.com/mgbckr/unity-asset-rounded-rectangles/issues
