using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class DynamicCameraControl : MonoBehaviour
{
    public static DynamicCameraControl Instance { get; private set; }

    public CinemachineFreeLook freeLookCamera;
    public AnimationCurve transitionSpeedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float initialTopRigRadius;
    private float initialTopRigHeight;

    private Tween currentTween;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            freeLookCamera = GetComponent<CinemachineFreeLook>();
            initialTopRigRadius = freeLookCamera.m_Orbits[0].m_Radius;
            initialTopRigHeight = freeLookCamera.m_Orbits[0].m_Height;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public void ChangeTopRigHeight(float newTopRigHeight, float transitionTime = 1f)
    {
        KillCurrentTween();
        currentTween = DOTween.To(() => freeLookCamera.m_Orbits[0].m_Height, x => freeLookCamera.m_Orbits[0].m_Height = x, newTopRigHeight, transitionTime)
            .SetEase(transitionSpeedCurve);
    }

    public void ChangeTopRigRadius(float newTopRigRadius, float transitionTime = 1f)
    {
        KillCurrentTween();
        currentTween = DOTween.To(() => freeLookCamera.m_Orbits[0].m_Radius, x => freeLookCamera.m_Orbits[0].m_Radius = x, newTopRigRadius, transitionTime)
            .SetEase(transitionSpeedCurve);
    }

    public void ChangeTopRigHeightAndRadius(float newTopRigRadius, float newTopRigHeight, float transitionTime = 1f)
    {
        KillCurrentTween();
        currentTween = DOTween.Sequence()
            .Append(DOTween.To(() => freeLookCamera.m_Orbits[0].m_Radius, x => freeLookCamera.m_Orbits[0].m_Radius = x, newTopRigRadius, transitionTime))
            .Insert(0, DOTween.To(() => freeLookCamera.m_Orbits[0].m_Height, x => freeLookCamera.m_Orbits[0].m_Height = x, newTopRigHeight, transitionTime))
            .SetEase(transitionSpeedCurve);
    }

    public void ResetCameraHeight(float resetTime = 1f)
    {
        KillCurrentTween();
        currentTween = DOTween.To(() => freeLookCamera.m_Orbits[0].m_Height, x => freeLookCamera.m_Orbits[0].m_Height = x, initialTopRigHeight, resetTime)
            .SetEase(transitionSpeedCurve);
    }

    public void ResetCameraRadius(float resetTime = 1f)
    {
        KillCurrentTween();
        currentTween = DOTween.To(() => freeLookCamera.m_Orbits[0].m_Radius, x => freeLookCamera.m_Orbits[0].m_Radius = x, initialTopRigRadius, resetTime)
            .SetEase(transitionSpeedCurve);
    }

    public void ChangeTopRigHeightAndReset(float newTopRigHeight, float resetDelay = 2f, float transitionTime = 1f)
    {
        ChangeTopRigHeight(newTopRigHeight, transitionTime);

        DOTween.Sequence()
            .AppendInterval(transitionTime + resetDelay)
            .AppendCallback(() => ResetCameraHeight());
    }

    public void ChangeTopRigRadiusAndReset(float newTopRigRadius, float resetDelay = 2f, float transitionTime = 1f)
    {
        ChangeTopRigRadius(newTopRigRadius, transitionTime);

        DOTween.Sequence()
            .AppendInterval(transitionTime + resetDelay)
            .AppendCallback(() => ResetCameraRadius());
    }

    public void ChangeTopRigHeightAndRadiusAndReset(float newTopRigRadius, float newTopRigHeight, float resetDelay = 2f, float transitionTime = 1f)
    {
        ChangeTopRigHeightAndRadius(newTopRigRadius, newTopRigHeight, transitionTime);

        DOTween.Sequence()
            .AppendInterval(transitionTime + resetDelay)
            .AppendCallback(() => ResetCameraHeightAndRadius());
    }

    public void ResetCameraHeightAndRadius(float resetTime = 1f)
    {
        ResetCameraHeight(resetTime);
        ResetCameraRadius(resetTime);
    }

    void Update()
    {
        
    }

    private void KillCurrentTween()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
    }
}
