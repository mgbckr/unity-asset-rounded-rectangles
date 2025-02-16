# Rounded Rectangles

The "Rounded Rectangles" asset allows the crafting of rounded rectangles using shader graphs solely using built-in nodes. This makes it compatible with PolySpatial and Apple Vision Pro.

Rounded Rectangles supports

- individual corner radii
- borders with individual border sizes
- padding
- color and texture

## Quickstart

There are a couple of examples in the demo scene (`Rounded Rectangles/Samples/Demo Scene`). If you want to start using `Rounded Rectangles` yourself, follow the following steps.

- Create a new material from the `Rounded Rectangle` shader graph (right-click -> Create -> Material).
- Assign the new material to a `RawImage`. Make sure to set some default texture.
- Have fun playing with the options found under the Shader's' `Surface Inputs`.

## Notes

While there are a couple of implementations out there that make beautiful rounded rectangles, none of them worked for me on PolySpatial for the Apple Vision Pro. That's why I created these shader graphs which solely use the corresponding shader nodes. No scripting is needed.
