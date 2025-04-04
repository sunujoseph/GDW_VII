using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBlock : MonoBehaviour
{
    [SerializeField] public GameObject objectToWatch;

    public Material mat;
    [SerializeField] Material dissolveMaterial;
    public float fade;
    public bool isDissolving;

    private void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
        fade = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (objectToWatch == null)
        {
            gameObject.GetComponent<SpriteRenderer>().material = dissolveMaterial;
            mat = dissolveMaterial;

            StartCoroutine(DestroyAfterDelay(1f));

            //Destroy(gameObject); // Destroy this block
        }

        DeathDissolve();
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //Destroy(gameObject);
        isDissolving = true;
    }

    void DeathDissolve()
    {
        //dissolve effect on death
        if (isDissolving)
        {
            fade -= Time.deltaTime;
            //avoid negatives
            if (fade < 0)
            {
                fade = 1;
                isDissolving = false;
                Destroy(this.gameObject);
            }

            //update attached material
            mat.SetFloat("_Fade", fade);
        }
    }

}
