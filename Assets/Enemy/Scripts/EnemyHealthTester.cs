using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyHealthTester : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.hKey.wasPressedThisFrame)
        {
            health.TakeDamage(25f);
        }
    }
}