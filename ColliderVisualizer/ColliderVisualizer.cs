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

        public const int MAX_COLLIDERS_TO_DRAW = 50;
        private Collider[] collidersToDraw = new Collider[MAX_COLLIDERS_TO_DRAW];
        private int amountToDraw = 0;

        public bool IsToDraw = false;

        public float CheckPeriod = 0.25f;

        public bool DrawBoundingBoxes = true;
        public bool DrawTriggers = true;
        public bool DrawColliders = true;

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

            //Collider Bounds
            if (DrawBoundingBoxes)
            {
                GL.PushMatrix();
                GL.MultMatrix(Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one));

                for (int i = 0; i < amountToDraw; i++)
                {
                    if (collidersToDraw[i] != null)
                        GLHelpers.DrawWireframeCube(collidersToDraw[i].bounds.size, collidersToDraw[i].bounds.center - collidersToDraw[i].bounds.extents, BoundsColor);
                }
                GL.PopMatrix();
            }
            //Collider 
            for (int i = 0; i < amountToDraw; i++)
            {
                if (collidersToDraw[i] != null 
                    && (collidersToDraw[i].isTrigger && DrawTriggers || !collidersToDraw[i].isTrigger && DrawColliders))
                {
                    Color colorToUse = collidersToDraw[i].isTrigger ? TriggerVolumeColor : ColliderColor;
                    Type colliderType = collidersToDraw[i].GetType();
                    if (colliderType == typeof(BoxCollider))
                    {
                        BoxCollider box = (BoxCollider)collidersToDraw[i];
                        GL.PushMatrix();
                        GL.MultMatrix(collidersToDraw[i].transform.localToWorldMatrix);
                        GLHelpers.DrawWireframeCube(box.size, box.center - box.size / 2f, colorToUse);
                        GL.PopMatrix();
                    }
                    else if (colliderType == typeof(SphereCollider))
                    {
                        SphereCollider sphere = (SphereCollider)collidersToDraw[i];
                        GL.PushMatrix();
                        GL.MultMatrix(collidersToDraw[i].transform.localToWorldMatrix);
                        GLHelpers.DrawSimpleWireframeSphere(sphere.radius, sphere.center, colorToUse, 12);
                        GL.PopMatrix();
                    }
                    else if (colliderType == typeof(CapsuleCollider))
                    {
                        CapsuleCollider capsule = (CapsuleCollider)collidersToDraw[i];
                        GL.PushMatrix();
                        GL.MultMatrix(collidersToDraw[i].transform.localToWorldMatrix);
                        GLHelpers.DrawSimpleWireframeCapsule(capsule.radius, capsule.height, capsule.center, colorToUse, 12);
                        GL.PopMatrix();
                    }
                }
            }
        }
    }
}
