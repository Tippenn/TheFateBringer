using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpitterBulletScript : MonoBehaviour
{
    public IEnumerator ShootTo(Vector3 position)
    {
        float elapsedTime = 0f;
        float time = 0.25f;
        while (elapsedTime < time)
        {
            // Lerp between the start position and target position
            transform.position = Vector3.Lerp(transform.position, position, elapsedTime / time);
            elapsedTime += Time.deltaTime;

            // Wait for the next frame before continuing the loop
            yield return null;
        }
        Destroy(gameObject);
    }
}
