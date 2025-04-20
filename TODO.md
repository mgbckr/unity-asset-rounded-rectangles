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
- [x] fix offset for segment based positioning
- [x] ensure corner radius is at least the size of the border
- [x] fix segment scaling
- [x] some clean-up and renaming please
- [x] move things into a utility class
- [x] move segment based positioning into extra function not into a class per se
    - so we can animate things later
- [x] shadows with different border sizes; THIS WAS MATH HEAVY
    - can we do this in a simpler way?
- [ ] translate FrameFractionUtils code to shader code
    - [x] shader code
    - [ ] see whether that is slower/faster
- [ ] clean up and organize shader graph files
    - [ ] add simple optimized sub graphs where it is easy and takes no time

### Image with content

- [ ] Make simple, single border image
- [ ] Load images from URL?
- [ ] Make three or four windows
- [ ] Account for texture size when scaling
- [ ] Test many borders with images ... oh oh!!

### Interactive

- [ ] Add dummy controls
- [ ] See how things look and optimize!

- [ ] set up interaction framework
- [ ] make controls react to focus/hover (resize, color, etc.)
    - [ ] simple hover effect
    - [ ] animate corners
- [ ] Move / Close
- [ ] Resize
- [ ] Pan / Zooming
- [ ] Test whether reusing shader based controls between images is more efficient
- [ ] Crop and copy

## Backlog

### Image content

- [ ] Allow border overlay in some shaders (so image can shine through border)

### Fancy stuff

I'll add those when I need them.

- [ ] Shadows
    - [ ] Try simple inverse lerp / smoothstep shadow (no conditions)
    - [ ] Enable some inner and outer shadows
    - [ ] can this be more efficient if we use same border radius? (see optimization)

- [ ] Bloom / glow for neon effect?
- [ ] Border gradients
- [ ] Dashed border
- [ ] Support text somehow?
- [ ] Support some icons?

### Optimization

- [ ] For stacked border test whether chaining makes a difference
- [ ] Can we optimize the Rounded Rectangle Fraction ... I think it is too inefficient
    - [x] optimize by moving code out of shader ... helps a lot
    - [ ] maybe try something with equal corner radius?
    - [ ] another thing could be to try to get rid of the subtraction for corners ... mh
    - mhm, think about different border widths, too, though
- [ ] generally, can we make borders without subtraction? ... probably not actually
- [ ] I think, with the optimizations below we can make shadow fractions easily
- [ ] for corners, I could use the same approach as the built in rounded rectangle uses!
    - [x] can we use this for shadow corners as well? YES THAT WORKS!!! NICE!!! :D
- [ ] for full frames maybe use simple rounded rectangle for further optimization
- [ ] for shadows can we build more efficient shadows if we use equal corner radii?
    - see simple rounded rectangle (generated shader code)
- [ ] check if [MaterialPropertyBlock](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) does anything to help us
- [ ] optimize rounded corner shadow (variables inside custom nodes can be reused)

