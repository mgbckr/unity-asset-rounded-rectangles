# TODO

## Next

### Animation / controls

- [x] Make vertical pill (efficiency difference?)
- [x] Make fraction of rounded rectangle fraction in a single shader graph
- [x] Add rounded corners to rounded rectangle fraction
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
- [ ] clean up and organize shader graph files
    - [x] add simple optimized sub graphs where it is easy and takes no time
        - [x] Round Corner Gradient Overall Borders
        - [x] Round Corner Step
        - [x] Round Rectangle Gradient Individual Corners Uniform Borders Composite
        - [x] Round Rectangle Step Individual Corners Composite
    - [x] add shader gallery
        - [x] basic elements
            - [x] first draft with animations
            - [x] make sure all sub graphs are represented
        - [x] composite elements
            - [x] implement composite elements
            - [x] document elements
- [x] translate FrameFractionUtils code to shader code
    - [x] shader code
    - [x] see whether that is slower/faster
        - [x] implement benchmarking tools
            - [x] setup scenes
            - [x] write scripts
        - [x] benchmark on vision pro
            - no difference ... WTF? Well, we will see in practice

- [x] shadows with different border sizes; THIS WAS MATH HEAVY
    - [x] implement with some linear algebra :)
    - [x] check corner cases
        - [x] corner radius == 0
        - [x] border width == 0 (this is an issue ... how to solve?)
    - [x] check smoothness at border
        - maybe solve by using this gradient thing (see Rectangle node code)

- [x] implement production shaders
    - [x] implement using most flexible/convenient shaders first
        - [x] animated colored dynamic frame
            - [x] direct control
            - [x] indirect
        - [x] colored triple frame with shadows
            - [x] content: color
            - [x] content: texture

### Image with content

- [x] Make simple, single border image
    - [ ] add controls
        - [ ] test close button animation with looking at it
        - [x] fix issue with dynamic border
        - [x] add relative start/end offset (in hindsight probably useless)
        - [ ] animate corner
- [ ] Load images from URL?
- [ ] Make three or four windows
- [ ] Account for texture size when scaling
- [ ] Test many borders with images ... oh oh!!
- [ ] allow non 1x1 texture size
- [ ] allow non 1x1 objects

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
- [ ] run benchmarks on Meta Quest
- [ ] benchmark shaders
    - [x] implement benchmarks
    - [x] run on vision pro
        - there is nearly NO difference ... WTF ... we will see in practice
    - [ ] OPTIONAL: run on Meta Quest?
        - [ ] make shaders look nice on meta quest
    - [ ] deprecate/archive where no performance difference is observed
    - [ ] update shader gallery and move deprecated ones to extra galleries