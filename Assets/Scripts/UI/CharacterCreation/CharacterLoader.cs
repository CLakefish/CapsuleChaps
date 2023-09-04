using System;
using System.Collections;
using System.Collections.Generic; using UnityEngine.SceneManagement;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject capsuleObject;
    [SerializeField] private SpriteRenderer mouth;
    [SerializeField] private SpriteRenderer leftEye, rightEye;
    [SerializeField] private SpriteRenderer glasses, moustache;
    [SerializeField] private List<Sprite> eyes = new(), mouths = new(), glass = new(), moustaches = new();
    [SerializeField] private List<GameObject> hats = new();
    private GameObject hatObject;
    private float previousTimeBlinked;

    // Start is called before the first frame update
    void Start()
    {
        Load();
    }

    private void Update()
    {
        if (Time.time >= 4f + UnityEngine.Random.Range(-0.4f, 2f) + previousTimeBlinked)
        {
            leftEye.sprite = rightEye.sprite = eyes[6];
            previousTimeBlinked = Time.time;
        }
        else if (Time.time >= 0.2f + UnityEngine.Random.Range(-0.05f, 0.1f) + previousTimeBlinked)
        {
            leftEye.sprite = rightEye.sprite = eyes[PlayerPrefs.GetInt("eyeIndex")];
        }

        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void FixedUpdate()
    {
        if (hatObject == null && PlayerPrefs.GetInt("hatIndex") != 0)
        {
            MovementController m = FindObjectOfType<MovementController>();

            hatObject = Instantiate(hats[PlayerPrefs.GetInt("hatIndex")], m.transform.position + new Vector3(0, 0.35f, 0), transform.rotation);
            hatObject.transform.parent = m.transform;
        }
    }

    private void Load()
    {
        mouth.sprite = mouths[PlayerPrefs.GetInt("mouthIndex")];
        leftEye.sprite = rightEye.sprite = eyes[PlayerPrefs.GetInt("eyeIndex")];
        glasses.sprite = glass[PlayerPrefs.GetInt("glassesIndex")];
        moustache.sprite = moustaches[PlayerPrefs.GetInt("moustacheIndex")];

        string[] mouthValues = PlayerPrefs.GetString("mouthValues").Split("//"),
         eyeValues = PlayerPrefs.GetString("eyeValues").Split("//"),
         glassesValues = PlayerPrefs.GetString("glassesValues").Split("//"),
         moustacheValues = PlayerPrefs.GetString("moustacheValues").Split("//");

        if (mouthValues.Length > 0)
        {
            mouth.transform.localScale = new Vector3((float)Convert.ToDouble(mouthValues[0]), (float)Convert.ToDouble(mouthValues[1]), 0);
            mouth.transform.localPosition = new Vector3(0, -0.2f + (float)Convert.ToDouble(mouthValues[2]), 0);
        }

        if (eyeValues.Length > 0)
        {
            rightEye.transform.localScale = new Vector3((float)Convert.ToDouble(eyeValues[0]), (float)Convert.ToDouble(eyeValues[0]), 0);
            leftEye.transform.localScale = new Vector3(-(float)Convert.ToDouble(eyeValues[0]), (float)Convert.ToDouble(eyeValues[0]), 0);
            rightEye.transform.localPosition = new Vector3(rightEye.transform.localPosition.x, (float)Convert.ToDouble(eyeValues[1]), 0);
            leftEye.transform.localPosition = new Vector3(leftEye.transform.localPosition.x, (float)Convert.ToDouble(eyeValues[1]), 0);
            rightEye.transform.localPosition = new Vector3(0.15f - (float)Convert.ToDouble(eyeValues[2]), rightEye.transform.localPosition.y, 0);
            leftEye.transform.localPosition = new Vector3(-0.15f + (float)Convert.ToDouble(eyeValues[2]), leftEye.transform.localPosition.y, 0);
            rightEye.transform.localEulerAngles = new Vector3(0, 0, 0 - (float)Convert.ToDouble(eyeValues[3]));
            leftEye.transform.localEulerAngles = new Vector3(0, 0, 0 + (float)Convert.ToDouble(eyeValues[3]));
        }

        if (glassesValues.Length > 0)
        {
            glasses.transform.localPosition = new Vector3(0, (float)Convert.ToDouble(glassesValues[0]), 0);
            glasses.transform.localScale = new Vector3((float)Convert.ToDouble(glassesValues[1]), (float)Convert.ToDouble(glassesValues[1]), -0.05f);
        }

        if (moustacheValues.Length > 0)
        {
            moustache.transform.localPosition = new Vector3(0, (float)Convert.ToDouble(moustacheValues[1]), 0);
            moustache.transform.localScale = new Vector3((float)Convert.ToDouble(moustacheValues[0]), (float)Convert.ToDouble(moustacheValues[0]), -0.04f);
        }

        if ("glassesColorR".Length > 0)
        {
            string spriteToAlter = "glassesColor";

            glasses.color = new Color(
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "R")),
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "G")),
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "B")), 1);
        }

        if ("moustacheColorR".Length > 0)
        {
            string spriteToAlter = "moustacheColor";

            moustache.color = new Color(
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "R")),
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "G")),
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "B")), 1);
        }

        if (PlayerPrefs.GetString("bodyColorR").Length > 0)
        {
            string spriteToAlter = "bodyColor";

            FindObjectOfType<MovementController>().GetComponent<MeshRenderer>().sharedMaterial.color = new Color(
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "R")),
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "G")),
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "B")), 1);
        }
    }
}
