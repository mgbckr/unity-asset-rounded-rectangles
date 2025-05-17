# Development Notes

## 2025-05-17 Atomic Rounded Rectangles with Individual Corners

OOOOOK, so I tried to get rounded rectangles with individual corners running using an atomic approach (gradient and step version). That is, instead of creating each corner separately and then merging them, I tried a quadrant approach where the corner radius is set in each quadrant and then everything is processed together. This has a couple of issues:

- The corner radius can maximally have a radius of 0.5 (relative to the rectangle size).
- For simplicity I focused on the step version (_Development - Rounded Rectangle - Style Step - Corners Individual - Implementation Atomic): 
    - There I had some initial working versions however those had a grease in the middle. This was caused by my approach of corner smoothing where I use `fwidth` which has artifacts because I applied it to the SDF that had non-smooth transitions between quadrants.
    - Then I got rid of that grease but that caused a weird offset pixel when the corner radius was set to 0.
    - I fixed this with a branch, however, now when rotating the whole thing I am getting greases outside the rectangle. 
- CONCLUSION: Abandon! I will come back to this if I actually run into performance issues.
