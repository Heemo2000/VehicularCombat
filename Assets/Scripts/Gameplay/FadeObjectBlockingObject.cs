using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Game.Gameplay
{
    public class FadeObjectBlockingObject : MonoBehaviour
    {
        [SerializeField]
        private LayerMask layerMask;
        [SerializeField]
        private Transform target;
        [SerializeField]
        private Camera lookCamera;
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float fadedAlpha = 0.33f;
        [SerializeField]
        private bool retainShadows = true;
        [SerializeField]
        private Vector3 targetPositionOffset = Vector3.up;
        [Min(0.1f)]
        [SerializeField]
        private float fadeSpeed = 1;

        [Header("Read Only Data")]
        [SerializeField]
        private List<FadingObject> objectsBlockingView = new List<FadingObject>();
        private Dictionary<FadingObject, Coroutine> runningCoroutines = new Dictionary<FadingObject, Coroutine>();

        private RaycastHit[] hits = new RaycastHit[10];

        private void OnEnable()
        {
            StartCoroutine(CheckForObjects());
        }

        private IEnumerator CheckForObjects()
        {
            while (true)
            {
                int hitsCount = Physics.RaycastNonAlloc(
                    lookCamera.transform.position,
                    (target.transform.position + targetPositionOffset - lookCamera.transform.position).normalized,
                    this.hits,
                    Vector3.Distance(lookCamera.transform.position, target.transform.position + targetPositionOffset),
                    layerMask
                );

                if (hitsCount > 0)
                {
                    for (int i = 0; i < hitsCount; i++)
                    {
                        FadingObject fadingObject = GetFadingObjectFromHit(this.hits[i]);

                        if (fadingObject != null && !objectsBlockingView.Contains(fadingObject))
                        {
                            if (runningCoroutines.ContainsKey(fadingObject))
                            {
                                if (runningCoroutines[fadingObject] != null)
                                {
                                    StopCoroutine(runningCoroutines[fadingObject]);
                                }

                                runningCoroutines.Remove(fadingObject);
                            }

                            runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                            objectsBlockingView.Add(fadingObject);
                        }
                    }
                }

                FadeObjectsNoLongerBeingHit();

                ClearHits();

                yield return null;
            }
        }

        private void FadeObjectsNoLongerBeingHit()
        {
            List<FadingObject> objectsToRemove = new List<FadingObject>(objectsBlockingView.Count);

            foreach (FadingObject fadingObject in objectsBlockingView)
            {
                bool objectIsBeingHit = false;
                for (int i = 0; i < hits.Length; i++)
                {
                    FadingObject hitFadingObject = GetFadingObjectFromHit(hits[i]);
                    if (hitFadingObject != null && fadingObject == hitFadingObject)
                    {
                        objectIsBeingHit = true;
                        break;
                    }
                }

                if (!objectIsBeingHit)
                {
                    if (runningCoroutines.ContainsKey(fadingObject))
                    {
                        if (runningCoroutines[fadingObject] != null)
                        {
                            StopCoroutine(runningCoroutines[fadingObject]);
                        }
                        runningCoroutines.Remove(fadingObject);
                    }

                    runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectIn(fadingObject)));
                    objectsToRemove.Add(fadingObject);
                }
            }

            foreach (FadingObject removeObject in objectsToRemove)
            {
                objectsBlockingView.Remove(removeObject);
            }
        }

        private IEnumerator FadeObjectOut(FadingObject FadingObject)
        {
            foreach (Material material in FadingObject.Materials)
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.SetInt("_Surface", 1);

                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                material.SetShaderPassEnabled("DepthOnly", false);
                material.SetShaderPassEnabled("SHADOWCASTER", retainShadows);

                material.SetOverrideTag("RenderType", "Transparent");

                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            }

            float time = 0;

            while (FadingObject.Materials[0].color.a > fadedAlpha)
            {
                foreach (Material material in FadingObject.Materials)
                {
                    if (material.HasProperty("_Color"))
                    {
                        material.color = new Color(
                            material.color.r,
                            material.color.g,
                            material.color.b,
                            Mathf.Lerp(FadingObject.InitialAlpha, fadedAlpha, time * fadeSpeed)
                        );
                    }
                }

                time += Time.deltaTime;
                yield return null;
            }

            if (runningCoroutines.ContainsKey(FadingObject))
            {
                StopCoroutine(runningCoroutines[FadingObject]);
                runningCoroutines.Remove(FadingObject);
            }
        }

        private IEnumerator FadeObjectIn(FadingObject FadingObject)
        {
            float time = 0;

            while (FadingObject.Materials[0].color.a < FadingObject.InitialAlpha)
            {
                foreach (Material material in FadingObject.Materials)
                {
                    if (material.HasProperty("_Color"))
                    {
                        material.color = new Color(
                            material.color.r,
                            material.color.g,
                            material.color.b,
                            Mathf.Lerp(fadedAlpha, FadingObject.InitialAlpha, time * fadeSpeed)
                        );
                    }
                }

                time += Time.deltaTime;
                yield return null;
            }

            foreach (Material material in FadingObject.Materials)
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.SetInt("_Surface", 0);

                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

                material.SetShaderPassEnabled("DepthOnly", true);
                material.SetShaderPassEnabled("SHADOWCASTER", true);

                material.SetOverrideTag("RenderType", "Opaque");

                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            }

            if (runningCoroutines.ContainsKey(FadingObject))
            {
                StopCoroutine(runningCoroutines[FadingObject]);
                runningCoroutines.Remove(FadingObject);
            }
        }

        private void ClearHits()
        {
            System.Array.Clear(hits, 0, hits.Length);
        }

        private FadingObject GetFadingObjectFromHit(RaycastHit Hit)
        {
            return Hit.collider != null ? Hit.collider.GetComponent<FadingObject>() : null;
        }
    }
}
