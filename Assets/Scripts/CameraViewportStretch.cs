using UnityEngine;
using System.Collections;

public class CameraViewportStretch : MonoBehaviour {

    [SerializeField]
    private Vector2 ViewportSize = new Vector2(960f, 540f);

    [SerializeField, ReadOnly]
    private float AspectRatio = 1f;

    [SerializeField, ReadOnly]
    private float CurrentAspectRatio;

    private bool isNormalized = false;

    private Camera BackgroundCamera;

	public Color BackgroundColor = Color.black;

    void Awake()
    {
        AspectRatio = ViewportSize.x/ViewportSize.y;
    }

    void LateUpdate()
    {
        CurrentAspectRatio = (float) Screen.width/Screen.height;
        if (!isNormalized && Mathf.Abs(CurrentAspectRatio - AspectRatio) < Mathf.Epsilon) // Equals
        {
            isNormalized = true;
            Camera.main.rect = new Rect(0, 0, 1, 1);
            if (BackgroundCamera)
            {
                BackgroundCamera.gameObject.SetActive(false);
            }
            return;
        }

        if (!BackgroundCamera) {
            // Make a new camera behind the normal camera which displays black; otherwise the unused space is undefined
            BackgroundCamera = new GameObject("_BackgroundCam", typeof(Camera)).GetComponent<Camera>();
            BackgroundCamera.depth = int.MinValue;
            BackgroundCamera.clearFlags = CameraClearFlags.SolidColor;
            BackgroundCamera.backgroundColor = BackgroundColor;
            BackgroundCamera.cullingMask = 0;
        }
        // Pillarbox
		if (CurrentAspectRatio > AspectRatio) {
			float inset = 1.0f - AspectRatio/CurrentAspectRatio;
			Camera.main.rect = new Rect(inset/2, 0.0f, 1.0f-inset, 1.0f);
            BackgroundCamera.gameObject.SetActive(true);
		}
		// Letterbox
		else {
			float inset = 1.0f - CurrentAspectRatio/AspectRatio;
			Camera.main.rect = new Rect(0.0f, inset/2, 1.0f, 1.0f-inset);
            BackgroundCamera.gameObject.SetActive(true);
		}
    }
}
