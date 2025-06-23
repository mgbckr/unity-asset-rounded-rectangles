# Unified controls FAILED

I wanted nice controls like in Apple Vision OS where the hover animation depends on where the user looked before. However, PolySpatial only gives hover information within a shader. Additionally, Unity shaders do not allow statefullness. Which means we never know where the user looked previously.

So ChatGPT suggested storing information in the pixels rendered by the shader and then reading them via C# and then sending that back into the shader in the next frame. However, this kind of failed while reading the pixel from a `RenderedTexture`. This may be because we need a camera and the pixel actually needs to be rendered, but I am not sure. I could not make this approach work. 