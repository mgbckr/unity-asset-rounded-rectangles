# TODO

## Next

### Animation / controls

- [x] Make vertical pill (efficiency difference?)
- [x] Make fraction of rounded rectangle fraction in a single shader graph
- [ ] Add rounded corners to rounded rectangle fraction
    - [x] give out coordinates
        - [x] corner fraction
        - [x] coordinate rectangle
    - [x] output step function for parts from cascade to switch between coordinates
    - [x] combine and animate (a tiny bit laggy, damn)
    - [x] fix random dots
- [x] Fix gaps between corners and edges 
    - tricky I think
    - nah, just used step functions instead of built-in rectangle
- [x] Create script to find corner fraction points based on scale, border width, corner radius, etc.
- [x] move lots of logic outside of shader and see whether that makes a difference
    - it does! About twice as fast ...
- [ ] fix offset for segment based positioning
- [ ] move segment based positioning into extra function not into a class per se
- [ ] some clean-up and renaming please!!!
- [ ] translate code to shader code and see whether that is faster

### Image with content

- [ ] Make simple, single border image
- [ ] Load images from URL?
- [ ] Make three or four windows
- [ ] Account for texture size when scaling
- [ ] Test many borders with images ... oh oh!!

### Interactive

- [ ] Add dummy controls
- [ ] Move / Close
- [ ] Resize
    - [ ] Make controls usable
    - [ ] Animate controls
- [ ] Pan / Zooming
- [ ] Test whether reusing controls between images is more efficient

## Backlog

### Interactive

- [ ] Crop and copy
- [ ] Hover animation

### Image content

- [ ] Allow border overlay in some shaders (so image can shine through border)

### Fancy stuff

I'll add those when I need them.

- [ ] Shadows
    - [ ] Try simple inverse lerp / smoothstep shadow (no conditions)
    - [ ] Enable some inner and outer shadows

- [ ] Bloom / glow for neon effect?
- [ ] Border gradients
- [ ] Dashed border
- [ ] Support text somehow?
- [ ] Support some icons?

### Optimization

- [ ] For stacked border test whether chaining makes a difference
- [ ] Can we optimize the Rounded Rectangle Fraction ... I think it is too inefficient

