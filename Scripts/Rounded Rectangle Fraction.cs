// using UnityEngine;

// /** TODO: Could actually be moved to the shader or a variant of it. */
// public class RoundedRectangleFraction : MonoBehaviour
// {

//     public Vector4 padding;
//     public Vector4 cornerRadius;
//     public float borderWidth;
//     public int index;
//     public float relativeOrigin;
//     public float absoluteStartOffset;
//     public float absoluteEndOffset;
//     public Color color;

//     void Start()
//     {


//     }

//     void Update()
//     {

//         // Get the material from the renderer
//         material = GetComponent<Renderer>().material;

//         // Set the properties of the material
//         material.SetVector("_Padding_Left_Right_Bottom_Top", padding);
//         material.SetVector("_CornerRadius_lb_lt_rb_rt", cornerRadius);
//         material.SetFloat("_Border_Width", borderWidth);
//         material.SetColor("_Border_Color", color);

//         float settings = Settings(
//             transform.localScale.x,
//             transform.localScale.y,
//             padding,
//             cornerRadius,
//             borderWidth,
//             index,
//             relativeOrigin,
//             absoluteStartOffset,
//             absoluteEndOffset
//         );

//         material.SetFloat("_Origin", settings.x);
//         material.SetFloat("_Start", settings.y);
//         material.SetFloat("_End", settings.z);
//     }

//     /**
//         * @param padding The padding around the rectangle (left, right, bottom, top).
//         * @param cornerRadius The radius of the corners (left-bottom, left-top, right-bottom, right-top).
//         * @param borderWidth The width of the border.
//         */
//     Vector3 Settings(
//         float width,
//         float height,
//         Vector4 padding, 
//         Vector4 cornerRadius, 
//         float borderWidth,
//         int index,
//         float relativeOrigin,
//         float absoluteStartOffset,
//         float absoluteEndOffset)
//     {
//         // left, right, bottom, top edge
//         Vector4 edgeLengths = 
//             padding.zzxx + padding.wwyy 
//             + cornerRadius.xzxy + cornerRadius.ywzw;
//         edgeLengths = new Vector4(height, height, width, width) - edgeLengths;

//         // left-bottom, left-top, right-bottom, right-top corner (quarter circumference)
//         // For the radii we subtract half the border width.
//         // Formula: 2 * pi * r / 4
//         float half = borderWidth / 2f;
//         Vector4 halfBorder = new Vector4(half, half, half, half);
//         Vector4 cornerLengths = (cornerRadius - half) * Mathf.PI / 2f;

//         float[] orderedLengths = new float[8] {
//             edgeLengths.x,
//             cornerLengths.x,
//             edgeLengths.z,
//             cornerLengths.z,
//             edgeLengths.y,
//             cornerLengths.w,
//             edgeLengths.w,
//             cornerLengths.y
//         };

//         float[] cumsum = new float[9];
//         cumsum[0] = 0;
//         for (int i = 0; i < 8; i++)
//         {
//             cumsum[i + 1] = cumsum[i] + orderedLengths[i];
//         }
//         float circumference = cumsum[8];

//         float startLength = cumsum[index];
//         float referenceLength = orderedLengths[index];
//         float originLength = startLength + relativeOrigin * referenceLength;

//         var originOut = (originLength - absoluteStartOffset) / circumference;
//         var startOut = 0f;
//         var endOut = (originLength + absoluteStartOffset) / circumference;

//         return new Vector3(originOut, startOut, endOut);
//     }
// }
