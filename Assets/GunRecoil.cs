using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float recoilRotationAmount = 15f;
    public float recoverySpeed = 8f;

    private float _recoilRotation = 0f;
    private float _prevRecoilRotation = 0f;

    void LateUpdate()
    {
        // Decay toward 0
        _recoilRotation = Mathf.Lerp(_recoilRotation, 0f, Time.deltaTime * recoverySpeed);

        // Only apply the CHANGE since last frame, not the full value
        float delta = _recoilRotation - _prevRecoilRotation;
        _prevRecoilRotation = _recoilRotation;

        transform.localRotation *= Quaternion.Euler(0f, 0f, delta);
    }

    public void TriggerRecoil()
    {
        _recoilRotation += recoilRotationAmount;
    }
}