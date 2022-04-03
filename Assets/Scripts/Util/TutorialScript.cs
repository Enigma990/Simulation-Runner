using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text = null;
    [SerializeField] private string dialogue = null;
    private float waitTime = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Type()
    {
        foreach (char word in dialogue)
        {
            text.text += word.ToString();

            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            text.gameObject.SetActive(true);
            StartCoroutine(Type());
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            text.text = "System: ";
            text.gameObject.SetActive(false);
            StopAllCoroutines();
            Destroy(this.gameObject);
        }
    }
}
