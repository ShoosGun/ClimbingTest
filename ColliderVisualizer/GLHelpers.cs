using UnityEngine;
namespace ColliderVisualizer
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
        public static void DrawWireframeCube(Vector3 foward, Vector3 up, Vector3 right, Vector3 offset, Color color)
        {
            Vector3[] vertex = new Vector3[4];
            vertex[0] = (foward + right)/2f;
            vertex[1] = (-foward + right) / 2f;
            vertex[2] = (-foward - right) / 2f;
            vertex[3] = (foward - right) / 2f;

            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            for(int i = 0; i < 4; i++) 
            {
                GL.Vertex(vertex[i] + offset);
            }
            GL.Vertex(vertex[0] + offset);
            GL.End();

            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            for (int i = 0; i < 4; i++)
            {
                GL.Vertex(vertex[i] + offset + up);
            }
            GL.Vertex(vertex[0] + offset + up);
            GL.End();

            GL.Begin(GL.LINES);
            for (int i = 0; i < 4; i++)
            {
                GL.Color(color);
                GL.Vertex(vertex[i] + offset);
                GL.Vertex(vertex[i] + offset + up);
            }
            GL.End();
        }

        public static void DrawWireframeCircle(float radius, Vector3 normal, Vector3 up, Vector3 offset, Color color, int resolution = 3, float startAngle =0f, float endAngle = 2f*Mathf.PI, bool isWholeCircle = true)
        {
            if (resolution < 3 || radius <= 0f)
                return;
            normal = normal.normalized;
            up = up.normalized;

            GL.Begin(GL.LINE_STRIP);

            float angleStep = (endAngle - startAngle) / resolution;
            int aditionalSteps = isWholeCircle ? 1 : 0;

            GL.Color(color);
            Vector3 rotationVector = Vector3MathUtils.GetRotationVector(normal, up);
            for (int i = 0; i <= resolution + aditionalSteps; i++)
            {
                Vector3 radiusVector = Vector3MathUtils.GetRotatedVectorComponent(rotationVector, up, angleStep * i + startAngle);
                GL.Vertex(radiusVector * radius + offset);
            }
            GL.End();
        }
        public static void DrawWireframeSphere(float radius, Vector3 offset,Vector3 foward, Vector3 up, Color color, int resolution = 3)
        {
            Vector3 right = Vector3.Cross(foward, up);
            DrawWireframeCircle(radius, up, foward, offset, color, resolution);
            DrawWireframeCircle(radius, foward, right, offset, color, resolution);
            DrawWireframeCircle(radius, right, up, offset, color, resolution);
        }
        public static void DrawWireframeHemisphere(float radius, Vector3 offset, Vector3 foward, Vector3 up, Color color, int resolution = 3)
        {
            Vector3 right = Vector3.Cross(foward, up);
            DrawWireframeCircle(radius, up, foward, offset, color, resolution, -Mathf.PI / 2f, Mathf.PI / 2f, false);
            DrawWireframeCircle(radius, foward, right, offset, color, resolution);
            DrawWireframeCircle(radius, right, foward, offset, color, resolution, -Mathf.PI / 2f, Mathf.PI / 2f, false);
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
            Vector3 direction = startPoint - endPoint;
            Vector3 randomUpVector = Vector3MathUtils.GetArbitraryPerpendicularVector(direction);

            //Top and bottom Spheres
            DrawWireframeHemisphere(radius, startPoint, direction, randomUpVector, color, resolution);
            DrawWireframeHemisphere(radius, endPoint, -direction, -randomUpVector, color, resolution);

            GL.Begin(GL.LINES);
            float angleStep = 2f * Mathf.PI / resolution;
            Vector3 rotationVector = Vector3MathUtils.GetRotationVector(direction.normalized, randomUpVector);
            for (int i = 0; i <= resolution; i++)
            {
                Vector3 radiusVector = Vector3MathUtils.GetRotatedVectorComponent(rotationVector,randomUpVector, angleStep * i);
                Vector3 vertex1 = radiusVector * radius + startPoint;
                Vector3 vertex2 = radiusVector * radius + endPoint;

                GL.Color(color);
                GL.Vertex(vertex1);
                GL.Vertex(vertex2);
            }
            GL.End();
        }
        public static void DrawSimpleWireframeCapsule(float radius, float height, Vector3 up, Vector3 offset, Color color, int resolution = 3)
        {
            Vector3 randomFowardVector = Vector3MathUtils.GetArbitraryPerpendicularVector(up);
            //Top and bottom Spheres
            DrawWireframeSphere(radius, offset - up*height/2f, randomFowardVector, up, color, resolution);
            DrawWireframeSphere(radius, offset + up * height / 2f, randomFowardVector, up, color, resolution);

            GL.Begin(GL.LINES); 
            float angleStep = 2f * Mathf.PI / resolution;
            Vector3 rotationVector = Vector3MathUtils.GetRotationVector(up.normalized, randomFowardVector);
            for (int i = 0; i <= resolution; i++)
            {
                Vector3 radiusVector = Vector3MathUtils.GetRotatedVectorComponent(rotationVector, randomFowardVector, angleStep * i);
                Vector3 vertex1 = radiusVector * radius + offset - up * height / 2f;
                Vector3 vertex2 = radiusVector * radius + offset + up * height / 2f;

                GL.Color(color);
                GL.Vertex(vertex1);
                GL.Vertex(vertex2);
            }
            GL.End();
        }
        //Can be also used as a wireframe cylinder!
        public static void DrawWireframeCone(float coneRadiusStart, float coneRadiusEnd, Vector3 coneStart, Vector3 coneEnd, Color color, int resolution = 3)
        {
            Vector3 direction = coneEnd - coneStart;
            Vector3 randomFowardVector = Vector3MathUtils.GetArbitraryPerpendicularVector(direction);
            //Start Circle
            DrawWireframeCircle(coneRadiusStart, direction, randomFowardVector, coneStart, color, resolution);
            //End Circle
            DrawWireframeCircle(coneRadiusEnd, direction, randomFowardVector, coneEnd, color, resolution);
            //Connecting Lines
            GL.Begin(GL.LINES);
            float angleStep = 2f * Mathf.PI / resolution;
            Vector3 rotationVector = Vector3MathUtils.GetRotationVector(direction.normalized, randomFowardVector);
            for (int i = 0; i <= resolution; i++)
            {
                Vector3 radiusVector = Vector3MathUtils.GetRotatedVectorComponent(rotationVector, randomFowardVector, angleStep * i);
                Vector3 vertex1 = radiusVector * coneRadiusStart + coneStart;
                Vector3 vertex2 = radiusVector * coneRadiusEnd + coneEnd;

                GL.Color(color);
                GL.Vertex(vertex1);
                GL.Vertex(vertex2);
            }
            GL.End();

        }
    }
}
