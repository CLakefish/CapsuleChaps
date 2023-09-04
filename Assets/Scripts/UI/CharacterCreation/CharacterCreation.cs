using System.Collections;
using System.Collections.Generic; using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterCreation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject capsule;
    [SerializeField] private Transform[] positions;
    [SerializeField] private GameObject[] menus;
    [SerializeField] private int position = 0;
    private Vector3 velocity;

    [Header("Sprite Objects")]
    [SerializeField] private SpriteRenderer mouth;
    [SerializeField] private SpriteRenderer glasses, moustache;
    [SerializeField] private SpriteRenderer leftEye, rightEye;

    [Header("Sprites")]
    [SerializeField] private List<Sprite> eyes = new();
    [SerializeField] private List<Sprite> mouths = new(), glass = new(), moustaches = new();
    [SerializeField] private List<GameObject> hats = new();
    [SerializeField] private List<UnityEngine.UI.Button> hatButtons = new();
    private GameObject hatObject;

    [Header("Sliders")]
    [SerializeField] private Slider eyeScale;
    [SerializeField] private Slider eyePosition, eyeSeperation, eyeRotation;
    [SerializeField] private Slider moustacheScale, moustachePosition;
    [SerializeField] private Slider mouthScale, mouthPosition, mouthThickness;
    [SerializeField] private Slider glassesScale, glassesPosition;
    private int eyeIndex;
    private float previousTimeBlinked;

    private void Start()
    {
        //Camera.main.backgroundColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1, 0.1f, 0.8f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        string[] mouthValues = PlayerPrefs.GetString("mouthValues").Split("//"), 
                 eyeValues = PlayerPrefs.GetString("eyeValues").Split("//"), 
                 glassesValues = PlayerPrefs.GetString("glassesValues").Split("//"), 
                 moustacheValues = PlayerPrefs.GetString("moustacheValues").Split("//");

        if (eyeValues.Length > 0)
        {
            eyeScale.value = (float)Convert.ToDouble(eyeValues[0]);
            eyePosition.value = (float)Convert.ToDouble(eyeValues[1]);
            eyeSeperation.value = (float)Convert.ToDouble(eyeValues[2]);
            eyeRotation.value = (float)Convert.ToDouble(eyeValues[3]);

            leftEye.sprite = rightEye.sprite = eyes[PlayerPrefs.GetInt("eyeIndex")];
        }
        if (mouthValues.Length > 0)
        {
            mouthScale.value = (float)Convert.ToDouble(mouthValues[0]);
            mouthThickness.value = (float)Convert.ToDouble(mouthValues[1]);
            mouthPosition.value = (float)Convert.ToDouble(mouthValues[2]);

            mouth.sprite = mouths[PlayerPrefs.GetInt("mouthIndex")];
        }
        if (glassesValues.Length > 0)
        {
            glassesPosition.value = (float)Convert.ToDouble(glassesValues[0]);
            glassesScale.value = (float)Convert.ToDouble(glassesValues[1]);

            glasses.sprite = glass[PlayerPrefs.GetInt("glassesIndex")];
        }
        if (moustacheValues.Length > 0)
        {
            moustacheScale.value = (float)Convert.ToDouble(moustacheValues[0]);
            moustachePosition.value = (float)Convert.ToDouble(moustacheValues[1]);

            moustache.sprite = moustaches[PlayerPrefs.GetInt("moustacheIndex")];
        }

        if (PlayerPrefs.GetString("glassesColorR").Length > 0)
        {
            string spriteToAlter = "glassesColor";

            glasses.color = new Color(
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "R")),
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "G")),
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "B")), 1);
        }

        if (PlayerPrefs.GetString("moustacheColorR").Length > 0)
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

            GetComponent<MeshRenderer>().sharedMaterial.color = new Color(
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "R")),
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "G")),
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "B")), 1);
        }

        int hat = PlayerPrefs.GetInt("hatIndex");

        if (hat != 0)
        {
            hatObject = Instantiate(hats[hat], capsule.transform.position + new Vector3(0, 0.35f, 0), Quaternion.identity);
            hatObject.transform.parent = transform;
        }
        else if (hatObject != null) Destroy(gameObject);

        capsule.transform.rotation = Quaternion.Euler(new Vector3(0, -25, 0));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Horizontal") * 150 * Time.deltaTime);

        capsule.transform.position = Vector3.SmoothDamp(capsule.transform.position, positions[position].position, ref velocity, 0.2f);

        #region Blinking 

        if (Time.time >= 4f + UnityEngine.Random.Range(-0.4f, 2f) + previousTimeBlinked)
        {
            leftEye.sprite = rightEye.sprite = eyes[6];
            previousTimeBlinked = Time.time;
        }
        else if (Time.time >= 0.2f + UnityEngine.Random.Range(-0.05f, 0.1f) + previousTimeBlinked)
        {
            leftEye.sprite = rightEye.sprite = eyes[PlayerPrefs.GetInt("eyeIndex")];
        }

        #endregion
    }

    private void FixedUpdate()
    {
        for (int i = 1; i < hatButtons.Count; i++)
        {
            if (PlayerPrefs.GetInt("hatIndex" + i.ToString()) == 0)
            {
                hatButtons[i].enabled = false;
                hatButtons[i].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }

        #region Eyes

        rightEye.transform.localScale = new Vector3(eyeScale.value, eyeScale.value, 0);
        leftEye.transform.localScale = new Vector3(-eyeScale.value, eyeScale.value, 0);

        rightEye.transform.localPosition = new Vector3(rightEye.transform.localPosition.x, eyePosition.value, 0);
        leftEye.transform.localPosition = new Vector3(leftEye.transform.localPosition.x, eyePosition.value, 0);

        rightEye.transform.localPosition = new Vector3(0.15f - eyeSeperation.value, rightEye.transform.localPosition.y, 0);
        leftEye.transform.localPosition = new Vector3(-0.15f + eyeSeperation.value, leftEye.transform.localPosition.y, 0);

        rightEye.transform.localEulerAngles = new Vector3(0, 0, 0 - eyeRotation.value);
        leftEye.transform.localEulerAngles = new Vector3(0, 0, 0 + eyeRotation.value);

        #endregion

        #region Mouth

        mouth.transform.localScale = new Vector3(mouthScale.value, mouthThickness.value, 0);

        mouth.transform.localPosition = new Vector3(0, -0.2f + mouthPosition.value, 0);

        #endregion

        #region Glasses

        glasses.transform.localPosition = new Vector3(0, glassesPosition.value, -0.05f);
        glasses.transform.localScale = new Vector3(glassesScale.value, glassesScale.value, 0);

        #endregion

        #region Moustache

        moustache.transform.localPosition = new Vector3(0, moustachePosition.value, -0.04f);
        moustache.transform.localScale = new Vector3(moustacheScale.value, moustacheScale.value, 0);

        #endregion

        int amount = 0;

        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].activeSelf) amount++;
        }

        if (amount <= 0 && position != 3) position = 0;
    }

    public void ApplyChanges()
    {
        PlayerPrefs.SetString("eyeValues", eyeScale.value.ToString() + "//" + eyePosition.value.ToString() + "//" + eyeSeperation.value.ToString() + "//" + eyeRotation.value.ToString());
        PlayerPrefs.SetString("mouthValues", mouthScale.value.ToString() + "//" + mouthThickness.value.ToString() + "//" + mouthPosition.value.ToString());
        PlayerPrefs.SetString("glassesValues", glassesPosition.value.ToString() + "//" + glassesScale.value.ToString());
        PlayerPrefs.SetString("moustacheValues", moustacheScale.value.ToString() + "//" + moustachePosition.value.ToString());

        if (position == 0) position = 3;
        StartCoroutine(ChangeScene());
    }

    public IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(1);
    }

    public void OpenMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
        position = 1;
    }

    public void OpenSpecialMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
        position = 0;
    }

    public void OpenHatMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
        position = 2;
    }

    public void SetEyes(int index)
    {
        leftEye.sprite = rightEye.sprite = eyes[index];
        eyeIndex = index;
        PlayerPrefs.SetInt("eyeIndex", index);
    }

    public void SetMouth(int index)
    {
        mouth.sprite = mouths[index];
        PlayerPrefs.SetInt("mouthIndex", index);
    }

    public void SetGlasses(int index)
    {
        glasses.sprite = glass[index];
        PlayerPrefs.SetInt("glassesIndex", index);
    }

    public void SetMoustache(int index)
    {
        moustache.sprite = moustaches[index];
        PlayerPrefs.SetInt("moustacheIndex", index);
    }

    public void SetHat(int index)
    {
        if (hatObject != null)
        {
            Destroy(hatObject);
        }

        if (index == 0)
        {
            PlayerPrefs.SetInt("hatIndex", index);
        }
        else
        {
            hatObject = Instantiate(hats[index], capsule.transform.position + new Vector3(0, 0.35f, 0), capsule.transform.rotation);
            hatObject.transform.localRotation *= Quaternion.Euler(new Vector3(0, 26.75f, 0));
            hatObject.transform.parent = transform;

            PlayerPrefs.SetInt("hatIndex", index);
        }
    }
}
