using System;
using System.Collections;
using UnityEngine;

namespace ColliderVisualizer
{
    public class ColliderVisualizer : MonoBehaviour
    {
        public float Check_radius = 10f;

        static Material lineMaterial;
        static Color TriggerVolumeColor = Color.green;
        static Color ColliderColor = Color.blue;
        static Color BoundsColor = Color.red;
        static Color ShapeBoundColor = Color.magenta;
        static Color DetectorShapeColor = Color.white;
        static Color VolumeShapeColor = Color.cyan;

        public const int MAX_COLLIDERS_TO_DRAW = 50;
        private Collider[] collidersToDraw = new Collider[MAX_COLLIDERS_TO_DRAW];
        private int amountToDraw = 0;

        public bool IsToDraw = false;

        public float CheckPeriod = 0.25f;

        public bool DrawBoundingBoxes = true;
        public bool DrawTriggers = true;
        public bool DrawColliders = true;
        public bool DrawShapeBounds = true;
        public bool DrawShapeDetector = true;
        public bool DrawShapeVolume = true;

        private void Start()
        {
            StartCoroutine("UpdateCollidersListWithDelay");
        }

        //TODO fazer um patch e atualizar essa lista com a destruição e construção do Collider
        private void UpdateCollidersList(float radius)
        {
            Transform reference;
            if (Locator.GetActiveCamera() != null)
                reference = Locator.GetActiveCamera().transform;
            else
                reference = Camera.main.transform;

            amountToDraw = Physics.OverlapSphereNonAlloc(reference.position, radius, collidersToDraw, ~0, QueryTriggerInteraction.Collide);
        }
        public void ChangeColliderDrawAmount(int amount = MAX_COLLIDERS_TO_DRAW)
        {
            if (amount > 0)
                collidersToDraw = new Collider[amount];
        }
        //From unity GL docs
        static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }
        private IEnumerator UpdateCollidersListWithDelay()
        {
            while (true)
            {
                UpdateCollidersList(Check_radius);
                yield return new WaitForSeconds(CheckPeriod);
            }
        }

        public void OnRenderObject()
        {
            if (!IsToDraw)
                return;

            CreateLineMaterial();
            lineMaterial.SetPass(0);
            if (DrawBoundingBoxes)
            {
                RenderBoundingBoxes(collidersToDraw, amountToDraw);
            }
            RenderColliders(collidersToDraw, amountToDraw, DrawTriggers, DrawColliders);
            if (DrawShapeBounds)
            {
                ShapeManager.Layer layer = ShapeManager._detectors;
                RenderShapeBounds(layer._array, layer.Count);

                ShapeManager.Layer[] layers = ShapeManager._volumes;
                for(int i = 0; i < layers.Length; i++) 
                {
                    RenderShapeBounds(layers[i]._array, layers[i].Count);
                }
            }
            if (DrawShapeDetector)
            {
                ShapeManager.Layer layer = ShapeManager._detectors;
                //GL.PushMatrix();
                //GL.MultMatrix(Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one));
                RenderShapes(layer._array, layer.Count, DetectorShapeColor);
                //GL.PopMatrix();
            }
            if (DrawShapeVolume)
            {
                ShapeManager.Layer[] layers = ShapeManager._volumes;
                //GL.PushMatrix();
                //GL.MultMatrix(Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one));
                for (int i = 0; i < layers.Length; i++)
                {
                    RenderShapes(layers[i]._array, layers[i].Count, VolumeShapeColor);
                }
                //GL.PopMatrix();
            }
        }
        private void RenderBoundingBoxes(Collider[] colliders, int amountToDraw) 
        {
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one));

            for (int i = 0; i < amountToDraw; i++)
            {
                if (colliders[i] != null)
                    GLHelpers.DrawSimpleWireframeCube(colliders[i].bounds.size, colliders[i].bounds.center - colliders[i].bounds.extents, BoundsColor);
            }
            GL.PopMatrix();
        }
    
        private void RenderColliders(Collider[] colliders, int amountToDraw, bool drawTriggers, bool drawColliders) 
        {
            for (int i = 0; i < amountToDraw; i++)
            {
                if (colliders[i] != null
                    && (colliders[i].isTrigger && drawTriggers || !colliders[i].isTrigger && drawColliders))
                {
                    Color colorToUse = colliders[i].isTrigger ? TriggerVolumeColor : ColliderColor;
                    Type colliderType = colliders[i].GetType();
                    if (colliderType == typeof(BoxCollider))
                    {
                        BoxCollider box = (BoxCollider)colliders[i];
                        GL.PushMatrix();
                        GL.MultMatrix(colliders[i].transform.localToWorldMatrix);
                        GLHelpers.DrawSimpleWireframeCube(box.size, box.center - box.size / 2f, colorToUse);
                        GL.PopMatrix();
                    }
                    else if (colliderType == typeof(SphereCollider))
                    {
                        SphereCollider sphere = (SphereCollider)colliders[i];
                        GL.PushMatrix();
                        GL.MultMatrix(colliders[i].transform.localToWorldMatrix);
                        GLHelpers.DrawWireframeSphere(sphere.radius, sphere.center, colliders[i].transform.forward, colliders[i].transform.up, colorToUse, 12);
                        GL.PopMatrix();
                    }
                    else if (colliderType == typeof(CapsuleCollider))
                    {
                        CapsuleCollider capsule = (CapsuleCollider)colliders[i];
                        GL.PushMatrix();
                        GL.MultMatrix(colliders[i].transform.localToWorldMatrix);
                        GLHelpers.DrawSimpleWireframeCapsule(capsule.radius, capsule.height, colliders[i].transform.up, capsule.center, colorToUse, 12);
                        GL.PopMatrix();
                    }
                }
            }
        }
        
        private void RenderShapeBounds(ShapeManager.ShapeData[] shapes, int count)
        {
            Color colorToUse = ShapeBoundColor;
            for (int i = 0; i < count; i++)
            {
                if (shapes[i] != null)
                {
                    ShapeManager.ShapeData bounds = shapes[i];
                    Transform t = bounds.shape.transform;
                    GLHelpers.DrawWireframeSphere(bounds.worldBoundsRadius, bounds.worldBoundsCenter, t.forward, t.up,  colorToUse, 12);
                }
            }
        }
        //TODO, esses dados apenas são atualizados quando necessarios, ou seja, temos que pegar essas informações do Shape mesmo
        private void RenderShapes(ShapeManager.ShapeData[] shapes, int count, Color colorToUse)
        {
            for (int i = 0; i < count; i++)
            {
                if (shapes[i] != null)
                {
                    RenderShape(shapes[i], colorToUse);
                }
            }
        }
        private void RenderShape(ShapeManager.ShapeData shape, Color colorToUse) 
        {
            switch (shape.type)
            {
                case ShapeManager.ShapeData.Type.Sphere:
                    SphereShape sphereShape = shape.sphere.sphereShape;
                    Vector3 sphereCenter = ShapeUtil.Sphere.CalcWorldSpaceCenter(sphereShape);
                    float sphereRadius = ShapeUtil.Sphere.CalcWorldSpaceRadius(sphereShape);
                    GLHelpers.DrawWireframeSphere(sphereRadius, sphereCenter, sphereShape.transform.forward, sphereShape.transform.up, Color.Lerp(colorToUse, Color.blue, 0.5f), 12);
                    break;

                case ShapeManager.ShapeData.Type.Hemisphere:
                    break;

                case ShapeManager.ShapeData.Type.Capsule:
                    CapsuleShape capsuleShape = shape.capsule.capsuleShape;
                    ShapeUtil.Capsule.CalcWorldSpaceEndpoints(capsuleShape, out float capsuleRadius, out Vector3 capsuleStart, out Vector3 capsuleEnd);
                    GLHelpers.DrawWireframeCapsule(capsuleRadius, capsuleStart, capsuleEnd, Color.Lerp(colorToUse, Color.green, 0.5f), 12);
                    break;

                case ShapeManager.ShapeData.Type.Hemicapsule:
                    break;

                case ShapeManager.ShapeData.Type.Cylinder:
                    CylinderShape cylinderShape = shape.cylinder.cylinderShape;
                    ShapeUtil.Cylinder.CalcWorldSpaceEndpoints(cylinderShape, out float cylinderRadius, out Vector3 cylinderStart, out Vector3 cylinderEnd);
                    GLHelpers.DrawWireframeCone(cylinderRadius, cylinderRadius, cylinderStart, cylinderEnd, Color.Lerp(colorToUse, Color.magenta, 0.5f), 12);
                    break;

                case ShapeManager.ShapeData.Type.Box:
                    break;
                
                case ShapeManager.ShapeData.Type.Cone:
                    ConeShape coneShape = shape.cone.coneShape;
                    ShapeUtil.Cone.CalcWorldSpaceEndpoints(coneShape, out float coneRadiusStart,out float coneRadiusEnd, out Vector3 coneStart, out Vector3 coneEnd);
                    GLHelpers.DrawWireframeCone(coneRadiusStart, coneRadiusEnd, coneStart, coneEnd,Color.Lerp(colorToUse,Color.red,0.5f), 12);
                    break;
            }
        }
    }
}
