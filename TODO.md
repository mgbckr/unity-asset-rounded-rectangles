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
        - [x] test close button animation with looking at it
        - [x] fix issue with dynamic border
        - [x] add relative start/end offset (in hindsight probably useless)
        - [x] animate corner
        - [x] allow moving
        - [x] add right resize control
        - [x] resizing
            - [x] implement controls
            - [x] fix resizing issues
                - [x] out of control corners and window sizes
                - [x] border width when change in ratio
                - [x] corner size when change ratio
                - [x] test with goggles on
            - [x] scale image when resizing (for now; see "Interactive" section)
        - [x] keep window oriented towards gaze
            - [x] try VisionOSBillboard: 
                I can turn this on and off but when I turn it off the rotation resets
                which is not what we want ... we will have to see what immersive mode means
            - [x] use (unbounded mode)[https://docs.unity3d.com/Packages/com.unity.polyspatial.visionos@2.3/manual/FAQ.html#q-objects-that-are-supposed-to-face-the-camera-transformlookat-are-not-working]
                - [x] initial spike
                - [x] fix tracking hand instead of head ... wtf
                    - had to add an XR Origin and use the Main Camera as a tracking target
                    - probably simply could have used any object with a `TrackedPoseDriver (Input System)` component on it (with the right configuration), but did not test this
                    - note I had to reset the `Camera Offset`'s position as well as the `Camera Y Offset` on the `XR Origin` to zero, otherwise there was an offset in what was tracked
                - [x] fix turn around of billboard!?
                    - turns out Quads are just facing away from the user with their front UV .. fixed via rotation
        - [ ] make controls stay the same size
            - [x] move window behavior implementation from `Input Manager` to `Window`
            - [x] test `Window`
            - [x] placement on scaling
            - [x] fix corner position / alignment with center
            - [x] change size based on distance
                - [x] initial version
                - [x] fix corners
                    - [x] make `Dynamic Frame Fraction Control` relative
            - [x] change size of corners when resizing
                - [x] allow relative setting of bottom start/end
                - [x] implement resizing
            - [x] fix minimum/maximum (?) size (see what Vision OS Windows do)
                - just fixing minimum for now
            - [x] lock control when selected
            - [x] disable non-active controls
            - [x] scale Z movement based on distance from avatar for fast movement further away
            - [ ] test and optimize on vision pro
        - [ ] OPTIONAL: window control optimizations
            - [ ] fix movement distortions when starting very far to the left 
                ... need to scale movement distance based on distance from hand
            - [ ] maybe also improve resizing feeling?
            - [ ] make control size change similar to Vision OS (there seems to be some scaling)
                - mhm, this is kind of difficult because they scale the frame funnily
- [ ] Load images from URL?
    - [x] allow loading texture from URL
    - [x] set image ratio automatically
    - [x] add loading and error placeholders / animations
    - [ ] OPTIONAL: smoothly animated transitions between loading, error, and main image
    - [ ] OPTIONAL: animate resizing when image is loaded
- [x] allow non 1x1 texture size (should be covered by auto ratio above)
- [x] allow non 1x1 objects (quads)
- [x] Make three or four windows
- [ ] Test many borders with images ... oh oh!!
    - [x] tried on laptop and it works ... very worried about Vision Pro
    - [ ] try on vision pro

### Interactive

- [ ] Pan / Zooming (this is a bit tricky because I don't know how controls should work)


### Functionality (browser)

- [ ] sync windows with browser (this is going to be a complicated one)
- [ ] sync tab groups
- [ ] implement closing window
- [ ] allow undo of closing window

### MVP 1 (transitional browser)

- [ ] add browser window
- [ ] switch between image and browser window seamlessly
- [ ] celebrate!!!

### MVP 1.1 (visuals)

- [ ] add antialiasing to image border 
- [ ] make moving smoother 
- [ ] make resizing smoother

### MVP 1.2 (user management)

- [ ] login etc :( not fun
- [ ] THIS IS A FINAL PRODUCT!!!


### MVP 2 (mind mapping)

- [ ] Crop and copy!!!
- [ ] allow cropping from browser window
- [ ] attach URL / source to windows
- [ ] allow text nodes
- [ ] add voice controls?
- [ ] figure out how to distinguish between crops and actual browser windows

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

### Interaction

- [ ] experiment with different interaction framework
    - [ ] move as default not with extra control?

### Optimization

- [ ] Test whether reusing shader based controls between images is more efficient
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