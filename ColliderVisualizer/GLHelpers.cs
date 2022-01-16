using UnityEngine;
namespace ModTemplate
{
    public static class GLHelpers
    {
        public static void DrawWireframeCube(Vector3 size, Vector3 offset, Color color)
        {
            GL.Begin(GL.LINES);

            //From the 0,0,0 corner
            GL.Color(color);
            GL.Vertex3(offset[0], offset[1], offset[2]);
            GL.Vertex3(size[0] + offset[0], offset[1], offset[2]);

            GL.Color(color);
            GL.Vertex3(offset[0], offset[1], offset[2]);
            GL.Vertex3(offset[0], size[1] + offset[1], offset[2]);

            GL.Color(color);
            GL.Vertex3(offset[0], offset[1], offset[2]);
            GL.Vertex3(offset[0], offset[1], size[2] + offset[2]);

            //From the size corner

            GL.Color(color);
            GL.Vertex3(size[0] + offset[0], size[1] + offset[1], size[2] + offset[2]);
            GL.Vertex3(offset[0], size[1] + offset[1], size[2] + offset[2]);

            GL.Color(color);
            GL.Vertex3(size[0] + offset[0], size[1] + offset[1], size[2] + offset[2]);
            GL.Vertex3(size[0] + offset[0], offset[1], size[2] + offset[2]);

            GL.Color(color);
            GL.Vertex3(size[0] + offset[0], size[1] + offset[1], size[2] + offset[2]);
            GL.Vertex3(size[0] + offset[0], size[1] + offset[1], offset[2]);

            //Lines That are Left

            GL.Color(color);
            GL.Vertex3(size[0] + offset[0], offset[1], offset[2]);
            GL.Vertex3(size[0] + offset[0], size[1] + offset[1], offset[2]);

            GL.Color(color);
            GL.Vertex3(size[0] + offset[0], offset[1], offset[2]);
            GL.Vertex3(size[0] + offset[0], offset[1], size[2] + offset[2]);


            GL.Color(color);
            GL.Vertex3(offset[0], size[1] + offset[1], offset[2]);
            GL.Vertex3(offset[0], size[1] + offset[1], size[2] + offset[2]);

            GL.Color(color);
            GL.Vertex3(offset[0], size[1] + offset[1], offset[2]);
            GL.Vertex3(size[0] + offset[0], size[1] + offset[1], offset[2]);


            GL.Color(color);
            GL.Vertex3(offset[0], offset[1], size[2] + offset[2]);
            GL.Vertex3(offset[0], size[1] + offset[1], size[2] + offset[2]);

            GL.Color(color);
            GL.Vertex3(offset[0], offset[1], size[2] + offset[2]);
            GL.Vertex3(size[0] + offset[0], offset[1], size[2] + offset[2]);

            GL.End();
        }

        public static void DrawWireframeCircle(float radius, Vector3 offset, Color color, int resolution = 3)
        {
            if (resolution < 3)
                return;

            GL.Begin(GL.LINES);

            float angleStep = 2f * Mathf.PI / resolution;
            float cosOfFirstVertex = 1f;
            float sinOfFirstVertex = 0f;
            float cosOfSecondVertex = Mathf.Cos(angleStep) * radius;
            float sinOfSecondVertex = Mathf.Sin(angleStep) * radius;

            for (int i = 0; i < resolution; i++)
            {
                GL.Color(color);
                GL.Vertex3(cosOfFirstVertex + offset[0], sinOfFirstVertex + offset[1], offset[2]);
                GL.Vertex3(cosOfSecondVertex + offset[0], sinOfSecondVertex + offset[1], offset[2]);

                cosOfFirstVertex = cosOfSecondVertex;
                sinOfFirstVertex = sinOfSecondVertex;
                sinOfSecondVertex = Mathf.Cos((i + 1) * angleStep) * radius;
                cosOfSecondVertex = Mathf.Sin((i + 1) * angleStep) * radius;
            }
            GL.End();
        }
        public static void DrawSimpleWireframeSphere(float radius, Vector3 offset, Color color, int resolution = 3)
        {
            if (resolution < 3)
                return;

            GL.Begin(GL.LINES);

            float angleStep = 2f * Mathf.PI / resolution;
            float cosOfFirstVertex = 1f;
            float sinOfFirstVertex = 0f;
            float cosOfSecondVertex = Mathf.Cos(angleStep) * radius;
            float sinOfSecondVertex = Mathf.Sin(angleStep) * radius;

            for (int i = 0; i < resolution; i++)
            {
                GL.Color(color);
                GL.Vertex3(cosOfFirstVertex + offset[0], sinOfFirstVertex + offset[1], offset[2]);
                GL.Vertex3(cosOfSecondVertex + offset[0], sinOfSecondVertex + offset[1], offset[2]);

                //Um circulo perpendicular ao anterior
                GL.Color(color);
                GL.Vertex3(offset[0], cosOfFirstVertex + offset[1], sinOfFirstVertex + offset[2]);
                GL.Vertex3(offset[0], cosOfSecondVertex + offset[1], sinOfSecondVertex + offset[2]);

                //Um outro circulo perpendicular ao anterior
                GL.Color(color);
                GL.Vertex3(cosOfFirstVertex + offset[0], offset[1], sinOfFirstVertex + offset[2]);
                GL.Vertex3(cosOfSecondVertex + offset[0], offset[1], sinOfSecondVertex + offset[2]);


                cosOfFirstVertex = cosOfSecondVertex;
                sinOfFirstVertex = sinOfSecondVertex;
                sinOfSecondVertex = Mathf.Cos((i + 1) * angleStep) * radius;
                cosOfSecondVertex = Mathf.Sin((i + 1) * angleStep) * radius;
            }
            GL.End();
        }
        public static void DrawSimpleWireframeCapsule(float radius, float height, Vector3 offset, Color color, int resolution = 3)
        {
            //Middle Circle
            DrawWireframeCircle(radius, offset, color, resolution);
            //Top and bottom Spheres
            DrawSimpleWireframeSphere(radius, new Vector3(0f, 0f, height / 2f) + offset, color, resolution);
            DrawSimpleWireframeSphere(radius, new Vector3(0f, 0f, height / -2f) + offset, color, resolution);
        }
    }
}
