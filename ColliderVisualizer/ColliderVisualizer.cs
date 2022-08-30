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
        public string ToggleDrawKBCommand = "f9";

        public float CheckPeriod = 0.25f;

        public bool DrawBoundingBoxes = true;
        public bool DrawTriggers = true;
        public bool DrawColliders = true;

        public bool DrawShapeBounds = true;
        public bool DrawShapeDetector = true;
        public bool DrawShapeVolume = true;
        public int[] ShapeLayersToDraw = new int[] { 1, 2, 3, 4 };

        private void Start()
        {
            StartCoroutine("UpdateCollidersListWithDelay");
        }
        private void Update() 
        {
            if (Event.current != null)
            {
                if (Event.current.Equals(Event.KeyboardEvent(ToggleDrawKBCommand)))
                {
                    IsToDraw = !IsToDraw;
                }
            }
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
                RenderShapes(layer._array, layer.Count, DetectorShapeColor);
            }
            if (DrawShapeVolume)
            {
                ShapeManager.Layer[] layers = ShapeManager._volumes;
                for (int i = 0; i < ShapeLayersToDraw.Length; i++)
                {
                    int layer = ShapeLayersToDraw[i] - 1;
                    if (layer >= 0 && layer < layers.Length)
                    {
                        RenderShapes(layers[layer]._array, layers[layer].Count, VolumeShapeColor);
                    }
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
                    HemisphereShape hemisphereShape = shape.hemisphere.hemisphereShape;
                    Vector3 hemisphereCenter = ShapeUtil.Sphere.CalcWorldSpaceCenter(hemisphereShape);
                    float hemisphereRadius = ShapeUtil.Sphere.CalcWorldSpaceRadius(hemisphereShape);
                    Vector3 hemisphereUp = ShapeUtil.Hemisphere.CalcWorldSpaceAxis(hemisphereShape);
                    GLHelpers.DrawWireframeHemisphere(hemisphereRadius, hemisphereCenter,hemisphereShape.transform.up, hemisphereUp, Color.Lerp(colorToUse, Color.white, 0.5f), 12);
                    break;

                case ShapeManager.ShapeData.Type.Capsule:
                    CapsuleShape capsuleShape = shape.capsule.capsuleShape;
                    ShapeUtil.Capsule.CalcWorldSpaceEndpoints(capsuleShape, out float capsuleRadius, out Vector3 capsuleStart, out Vector3 capsuleEnd);
                    GLHelpers.DrawWireframeCapsule(capsuleRadius, capsuleStart, capsuleEnd, Color.Lerp(colorToUse, Color.green, 0.5f), 12);
                    break;

                case ShapeManager.ShapeData.Type.Hemicapsule:
                    HemicapsuleShape hemicapsuleShape = shape.hemicapsule.hemicapsuleShape;
                    ShapeUtil.Capsule.CalcWorldSpaceEndpoints(hemicapsuleShape, out float hemicapsuleRadius, out Vector3 hemicapsuleStart, out Vector3 hemicapsuleEnd);
                    GLHelpers.DrawWireframeCapsule(hemicapsuleRadius, hemicapsuleStart, hemicapsuleEnd, Color.Lerp(colorToUse, Color.grey, 0.5f), 12);
                    break;

                case ShapeManager.ShapeData.Type.Cylinder:
                    CylinderShape cylinderShape = shape.cylinder.cylinderShape;
                    ShapeUtil.Cylinder.CalcWorldSpaceEndpoints(cylinderShape, out float cylinderRadius, out Vector3 cylinderStart, out Vector3 cylinderEnd);
                    GLHelpers.DrawWireframeCone(cylinderRadius, cylinderRadius, cylinderStart, cylinderEnd, Color.Lerp(colorToUse, Color.magenta, 0.5f), 12);
                    break;

                case ShapeManager.ShapeData.Type.Box:
                    BoxShape boxShape = shape.box.boxShape;
                    Vector3[] boxAxes = new Vector3[3];
                    Vector3[] verts = new Vector3[8];
                    ShapeUtil.Box.CalcWorldSpaceData(boxShape, out Vector3 boxCenter, out Vector3 boxSize, ref boxAxes,ref verts);
                    GLHelpers.DrawWireframeCube(boxAxes[2] * boxSize.z, boxAxes[1]*boxSize.y,boxAxes[0] * boxSize.x , boxCenter, Color.Lerp(colorToUse, Color.black, 0.5f));
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
