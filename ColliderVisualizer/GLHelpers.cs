using UnityEngine;
namespace ModTemplate
{
    public static class GLHelpers
    {
        public static void DrawSimpleWireframeCube(Vector3 size, Vector3 offset, Color color)
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
        //public static void DrawWireframeCube(Vector3[] corners, Vector3 offset, Color color)
        //{
        //    GL.Begin(GL.LINES);

        //    //From the 0,0,0 corner
        //    GL.Color(color);
        //    GL.Vertex3(offset[0], offset[1], offset[2]);
        //    GL.Vertex3(size[0] + offset[0], offset[1], offset[2]);

        //    GL.Color(color);
        //    GL.Vertex3(offset[0], offset[1], offset[2]);
        //    GL.Vertex3(offset[0], size[1] + offset[1], offset[2]);

        //    GL.Color(color);
        //    GL.Vertex3(offset[0], offset[1], offset[2]);
        //    GL.Vertex3(offset[0], offset[1], size[2] + offset[2]);

        //    //From the size corner

        //    GL.Color(color);
        //    GL.Vertex3(size[0] + offset[0], size[1] + offset[1], size[2] + offset[2]);
        //    GL.Vertex3(offset[0], size[1] + offset[1], size[2] + offset[2]);

        //    GL.Color(color);
        //    GL.Vertex3(size[0] + offset[0], size[1] + offset[1], size[2] + offset[2]);
        //    GL.Vertex3(size[0] + offset[0], offset[1], size[2] + offset[2]);

        //    GL.Color(color);
        //    GL.Vertex3(size[0] + offset[0], size[1] + offset[1], size[2] + offset[2]);
        //    GL.Vertex3(size[0] + offset[0], size[1] + offset[1], offset[2]);

        //    //Lines That are Left

        //    GL.Color(color);
        //    GL.Vertex3(size[0] + offset[0], offset[1], offset[2]);
        //    GL.Vertex3(size[0] + offset[0], size[1] + offset[1], offset[2]);

        //    GL.Color(color);
        //    GL.Vertex3(size[0] + offset[0], offset[1], offset[2]);
        //    GL.Vertex3(size[0] + offset[0], offset[1], size[2] + offset[2]);


        //    GL.Color(color);
        //    GL.Vertex3(offset[0], size[1] + offset[1], offset[2]);
        //    GL.Vertex3(offset[0], size[1] + offset[1], size[2] + offset[2]);

        //    GL.Color(color);
        //    GL.Vertex3(offset[0], size[1] + offset[1], offset[2]);
        //    GL.Vertex3(size[0] + offset[0], size[1] + offset[1], offset[2]);


        //    GL.Color(color);
        //    GL.Vertex3(offset[0], offset[1], size[2] + offset[2]);
        //    GL.Vertex3(offset[0], size[1] + offset[1], size[2] + offset[2]);

        //    GL.Color(color);
        //    GL.Vertex3(offset[0], offset[1], size[2] + offset[2]);
        //    GL.Vertex3(size[0] + offset[0], offset[1], size[2] + offset[2]);

        //    GL.End();
        //}

        public static void DrawWireframeCircle(float radius, Vector3 normal, Vector3 offset, Color color, int resolution = 3)
        {
            if (resolution < 3 && radius <= 0f)
                return;

            GL.Begin(GL.LINES);

            float angleStep = 2f * Mathf.PI / resolution;

            Vector3 radiusVector = Quaternion.AngleAxis(0f, normal) * Vector3.forward * radius;

            Vector3 vertex1 = radiusVector + offset;
            radiusVector = Quaternion.AngleAxis(angleStep, normal) * Vector3.forward * radius;
            Vector3 vertex2 = radiusVector + offset;

            for (int i = 0; i <= resolution + 1; i++)
            {
                GL.Color(color);

                GL.Vertex3(vertex1.x, vertex1.y, vertex1.z);
                GL.Vertex3(vertex2.x, vertex2.y, vertex2.z);

                vertex1 = vertex2;
                radiusVector = Quaternion.AngleAxis(angleStep, normal) * Vector3.forward * radius;
                vertex2 = radiusVector + offset;
            }
            GL.End();
        }
        public static void DrawSimpleWireframeSphere(float radius, Vector3 offset, Color color, int resolution = 3)
        {
            if (resolution < 3 && radius <= 0f)
                return;

            GL.Begin(GL.LINES);

            float angleStep = 2f * Mathf.PI / resolution;
            float cosOfFirstVertex = 1f;
            float sinOfFirstVertex = 0f;
            float cosOfSecondVertex = Mathf.Cos(angleStep) * radius;
            float sinOfSecondVertex = Mathf.Sin(angleStep) * radius;

            for (int i = 0; i <= resolution + 1; i++)
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
        public static void DrawWireframeCapsule(float radius, Vector3 startPoint, Vector3 endPoint, Color color, int resolution = 3)
        {
            DrawWireframeCircle(radius, startPoint - endPoint, (startPoint + endPoint)/2, color, resolution);
            //Top and bottom Spheres
            DrawSimpleWireframeSphere(radius, startPoint, color, resolution);
            DrawSimpleWireframeSphere(radius, endPoint, color, resolution);
        }
            public static void DrawSimpleWireframeCapsule(float radius, float height, Vector3 up, Vector3 offset, Color color, int resolution = 3)
        {
            //Middle Circle
            DrawWireframeCircle(radius, up, offset, color, resolution);
            //Top and bottom Spheres
            DrawSimpleWireframeSphere(radius, new Vector3(0f, 0f, height / 2f) + offset, color, resolution);
            DrawSimpleWireframeSphere(radius, new Vector3(0f, 0f, height / -2f) + offset, color, resolution);
        }
        //Can be also used as a wireframe cylinder!
        //TODO this isn't drawing correctly, the lines from the circle aren't following the circles
        public static void DrawWireframeCone(float coneRadiusStart, float coneRadiusEnd, Vector3 coneStart, Vector3 coneEnd, Color color, int resolution = 3)
        {
            Vector3 direction = coneEnd - coneStart;
            //Start Circle
            DrawWireframeCircle(coneRadiusStart, direction, coneStart, color, resolution);
            //End Circle
            DrawWireframeCircle(coneRadiusEnd, direction, coneEnd, color, resolution);
            //Connecting Lines
            GL.Begin(GL.LINES);

            float angleStep = 2f * Mathf.PI / resolution;
            for (int i = 0; i <= resolution; i++)
            {
                Vector3 radiusVector = Quaternion.AngleAxis(angleStep*i, direction) * Vector3.forward;
                Vector3 vertex1 = radiusVector*coneRadiusStart + coneStart;
                Vector3 vertex2 = radiusVector*coneRadiusEnd + coneEnd;

                GL.Color(color);
                GL.Vertex3(vertex1.x, vertex1.y, vertex1.z);
                GL.Vertex3(vertex2.x, vertex2.y, vertex2.z);
            }
            GL.End();

        }
    }
}
