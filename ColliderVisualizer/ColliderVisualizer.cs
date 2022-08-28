using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections;
using UnityEngine;

namespace ModTemplate
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
                RenderShapes(layer._array, layer.Count, DetectorShapeColor);
            }
            if (DrawShapeVolume)
            {
                ShapeManager.Layer[] layers = ShapeManager._volumes;
                for (int i = 0; i < layers.Length; i++)
                {
                    RenderShapes(layers[i]._array, layers[i].Count, VolumeShapeColor);
                }
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
                        GLHelpers.DrawSimpleWireframeSphere(sphere.radius, sphere.center, colorToUse, 12);
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
            for (int i = 0; i < count; i++)
            {
                if (shapes[i] != null)
                {
                    Color colorToUse = ShapeBoundColor;
                    ShapeManager.ShapeData bounds = shapes[i];
                    GLHelpers.DrawSimpleWireframeSphere(bounds.worldBoundsRadius, bounds.worldBoundsCenter, colorToUse, 12);
                }
            }
        }
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
                    ShapeManager.ShapeData.SphereShapeData sphereData = shape.sphere;
                    GLHelpers.DrawSimpleWireframeSphere(sphereData.worldRadius, sphereData.worldCenter, colorToUse, 12);
                    break;

                case ShapeManager.ShapeData.Type.Hemisphere:
                    break;

                case ShapeManager.ShapeData.Type.Capsule:
                    ShapeManager.ShapeData.CapsuleShapeData capsuleData = shape.capsule;
                    GLHelpers.DrawWireframeCapsule(capsuleData.worldRadius, capsuleData.worldStartPoint, capsuleData.worldEndPoint, colorToUse, 12);
                    break;

                case ShapeManager.ShapeData.Type.Hemicapsule:
                    break;

                case ShapeManager.ShapeData.Type.Cylinder:
                    ShapeManager.ShapeData.CapsuleShapeData cylinderData = shape.capsule;
                    GLHelpers.DrawWireframeCone(cylinderData.worldRadius, cylinderData.worldRadius, cylinderData.worldStartPoint, cylinderData.worldEndPoint, colorToUse, 12);
                    break;

                case ShapeManager.ShapeData.Type.Box:
                    break;
                
                case ShapeManager.ShapeData.Type.Cone:
                    ShapeManager.ShapeData.ConeShapeData coneData = shape.cone;
                    GLHelpers.DrawWireframeCone(coneData.worldStartRadius, coneData.worldEndRadius, coneData.worldStartPoint, coneData.worldEndPoint, colorToUse, 12);
                    break;
            }
        }
    }
}
